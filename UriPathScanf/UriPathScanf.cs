﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
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
        /// Query string prefix
        /// </summary>
        protected readonly string QsPrefix = "qs__";

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
                    if (attr != null)
                    {
                        result.Add(attr.IsQueryString ? QsPrefix + attr.BindName : attr.BindName, m);
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
        /// Get URI path metadata
        /// </summary>
        /// <param name="uriPath">URI path</param>
        /// <returns></returns>
        public UriMetadata GetMeta(string uriPath)
        {
            var result = new UriMetadata();

            var match = FindMatch(uriPath);

            if (!match.HasValue)
            {
                return null;
            }

            var (descriptor, urlMatches, queryString) = match.Value;

            var linkType = descriptor.Type;
            var metaType = descriptor.Meta;

            // NOTE: if dictionary instead of user defined type
            if (metaType == null)
            {
                var re = new Dictionary<string, string>();

                PrepareResult(urlMatches, queryString, re.Add);

                result.UriType = linkType;
                result.Meta = re;

                return result;
            }

            var metaResult = Activator.CreateInstance(metaType);

            PrepareResult(urlMatches, queryString, AddToMeta);

            result.UriType = linkType;
            result.Meta = metaResult;

            return result;

            void AddToMeta(string name, string value)
            {
                if (!_methods[descriptor].TryGetValue(name, out var prop)) return;
                prop.SetMethod.Invoke(metaResult, new object[] { value });
            }

            void PrepareResult(
                IEnumerable<(string, string)> matches,
                string qs,
                Action<string, string> add)
            {
                foreach (var (name, value) in matches)
                {
                    add(name, value);
                }

                var qsParsed = HttpUtility.ParseQueryString(qs);

                foreach (var s in qsParsed.AllKeys)
                {
                    add(QsPrefix + s, qsParsed[s]);
                }
            }
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
    }
}
