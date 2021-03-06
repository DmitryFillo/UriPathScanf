﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.WebUtilities;
using UriPathScanf.Attributes;

namespace UriPathScanf
{
    /// <inheritdoc />
    /// <summary>
    /// URI path parser
    /// </summary>
    public class UriPathScanf : IUriPathScanf
    {
        private readonly Regex _placeholderRegex = new Regex(@"\{(\w+)\}");
        private readonly Regex _slashRegex = new Regex(@"/+");

        private readonly UriPathDescriptor[] _descriptors;
        private readonly Dictionary<UriPathDescriptor, Dictionary<string, PropertyInfo>> _methods
            = new Dictionary<UriPathDescriptor, Dictionary<string, PropertyInfo>>();

        /// <summary>
        /// Creates instances of <see cref="T:UriPathScanf"/>
        /// </summary>
        /// <param name="descriptors">URI paths descriptors</param>
        public UriPathScanf(IEnumerable<UriPathDescriptor> descriptors)
        {
            // NOTE: to search longest uriPath format first
            _descriptors = descriptors.OrderByDescending(d => d.Format.ToCharArray().Aggregate(0, (acc, next) => next == '/' ? acc + 1 : acc)).ToArray();

            // NOTE: only for case when we need result model
            foreach (var d in _descriptors.Where(d => d.Meta != null))
            {
                var result = new Dictionary<string, PropertyInfo>();

                var assignableProps = d.Meta
                    .GetTypeInfo()
                    .DeclaredProperties
                    .Where(x => x.PropertyType.GetTypeInfo().IsAssignableFrom(typeof(string).GetTypeInfo()));

                foreach (var m in assignableProps)
                {
                    var attrs = m.GetCustomAttributes();

                    var attr = attrs.OfType<UriMetaAttribute>().FirstOrDefault();

                    if (attr == null) continue;

                    var name = attr.IsQueryString ? GetQueryStringBindingName(attr.BindName) : attr.BindName;

                    try
                    {
                        result.Add(name, m);
                    }
                    catch (ArgumentException)
                    {
                        // NOTE: do not fail on duplicates
                    }
                }

                try
                {
                    _methods.Add(d, result);
                }
                catch (ArgumentException)
                {
                    // NOTE: do not fail on duplicates
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Get URI path metadata (all descriptors)
        /// </summary>
        /// <param name="uriPath">URI path</param>
        /// <returns></returns>
        public UriMetadata ScanAll(string uriPath)
        {
            var match = FindMatch(uriPath);

            if (!match.HasValue) return null;

            var (descriptor, _, _) = match.Value;

            return descriptor.Meta == null ? GetDictMeta(match.Value) : GetTypedMeta(match.Value);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get URI path metadata (only typed descriptors)
        /// </summary>
        /// <param name="uriPath">URI path</param>
        /// <returns></returns>
        public UriMetadata<T> Scan<T>(string uriPath) where T: class, IUriPathMetaModel
        {
            var match = FindMatch(uriPath);

            if (!match.HasValue) return null;

            var (descriptor, _, _) = match.Value;

            return descriptor.Meta != typeof(T) ? null : (UriMetadata<T>) GetTypedMeta(match.Value);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get URI path metadata (only non-typed descriptors)
        /// </summary>
        /// <param name="uriPath">URI path</param>
        /// <returns></returns>
        public UriMetadata<IDictionary<string, string>> Scan(string uriPath)
        {
            var match = FindMatch(uriPath);

            if (!match.HasValue) return null;

            var (descriptor, _, _) = match.Value;

            return descriptor.Meta == null ? (UriMetadata<IDictionary<string, string>>) GetDictMeta(match.Value) : null;
        }

        /// <summary>
        /// Finds match in descriptors
        /// </summary>
        /// <param name="uriPath">URI path</param>
        /// <returns></returns>
        protected (UriPathDescriptor, IEnumerable<(string, string)>, string)? FindMatch(string uriPath)
        {
            foreach (var descr in _descriptors)
            {
                var format = descr.Format;

                var regexString = _placeholderRegex
                    .Replace(format, m => "(?<" + m.Groups[1].Value + ">.+)")
                    .TrimEnd('/');

                // NOTE: right to left search regexp, so it starts with ^
                regexString = _slashRegex.Replace(regexString, m => "/+");
                regexString = $@"^{regexString}/*(\?+.+)*";
                var formatRegex = new Regex(regexString, RegexOptions.RightToLeft | RegexOptions.IgnoreCase);

                var matches = formatRegex.Match(uriPath);

                if (!matches.Success)
                    continue;

                // NOTE: 1 is query string group (because of "right to left" regex)   
                var urlMatches = formatRegex.GetGroupNames().Where(g => g != "1" && g != "0").Select(m => (m, matches.Groups[m].Value));
                var queryString = matches.Groups[1].Value;

                return (descr, urlMatches, queryString);
            }

            return null;
        }

        /// <summary>
        /// Gets bind name for query string
        /// </summary>
        /// <param name="qsParamName">Query string param name</param>
        /// <returns></returns>
        protected string GetQueryStringBindingName(string qsParamName) => "qs__" + qsParamName;

        private UriMetadata GetDictMeta((UriPathDescriptor, IEnumerable<(string, string)>, string) match)
        {
            var (descriptor, urlMatches, queryString) = match;

            var re = new Dictionary<string, string>();

            foreach (var (name, value) in urlMatches)
            {
                re.Add(name, value);
            }

            foreach (var s in QueryHelpers.ParseQuery(queryString))
            {
                re.Add(GetQueryStringBindingName(s.Key), s.Value);
            }

            return new UriMetadata(descriptor.Type, re) { Type = typeof(Dictionary<string, string>) };
        }

        private UriMetadata GetTypedMeta((UriPathDescriptor, IEnumerable<(string, string)>, string) match)
        {
            var (descriptor, urlMatches, queryString) = match;

            var metaType = descriptor.Meta;

            var metaResult = Activator.CreateInstance(metaType);

            foreach (var (name, value) in urlMatches)
            {
                AddToMeta(name, value);
            }

            foreach (var s in QueryHelpers.ParseQuery(queryString))
            {
                AddToMeta(GetQueryStringBindingName(s.Key), s.Value);
            }

            return new UriMetadata(descriptor.Type, metaResult) { Type = metaType };

            void AddToMeta(string name, string value)
            {
                if (!_methods[descriptor].TryGetValue(name, out var prop)) return;
                prop.SetMethod.Invoke(metaResult, new object[] { value });
            }
        }
    }
}
