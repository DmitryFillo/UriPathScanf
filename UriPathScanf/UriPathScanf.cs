using System;
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
        private readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _methods
            = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        /// <summary>
        /// Query string prefix
        /// </summary>
        protected const string QsPrefix = "qs__";

        /// <summary>
        /// Creates instances of <see cref="T:UriPathScanf"/>
        /// </summary>
        /// <param name="descriptors">URI paths descriptors</param>
        public UriPathScanf(IEnumerable<UriPathDescriptor> descriptors)
        {
            // NOTE: to search longest uriPath format first
            _descriptors = descriptors.OrderByDescending(d => d.Format.Aggregate(0, (acc, next) => next == '/' ? acc + 1 : acc)).ToArray();

            // NOTE: only for case when we need result model
            foreach (var d in _descriptors.Where(d => d.Meta != null))
            {
                var result = new Dictionary<string, PropertyInfo>();

                var assignableProps = d.Meta
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.PropertyType.IsAssignableFrom(typeof(string)));

                foreach (var m in assignableProps)
                {
                    var attrs = m.GetCustomAttributes(true);

                    var attr = attrs.OfType<UriMetaAttribute>().FirstOrDefault();
                    if (attr != null)
                    {
                        result.Add(attr.BindName, m);
                    }
                }

                _methods.Add(d.Meta, result);
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

            var (linkFormat, urlMatches, queryString) = match.Value;

            var linkType = linkFormat.Type;
            var metaType = linkFormat.Meta;

            if (metaType == null)
            {
                var re = new Dictionary<string, string>();

                PrepareResult(urlMatches, queryString, (name, value) =>
                {
                    re.Add(name, value);
                });

                result.UriType = linkType;
                result.Meta = re;

                return result;
            }

            var metaResult = Activator.CreateInstance(metaType);
            var methods = _methods[metaType];

            PrepareResult(urlMatches, queryString, (name, value) =>
            {
                if (!methods.TryGetValue(name, out var prop)) return;

                prop.SetMethod.Invoke(metaResult, new object[] { value });
            });
            
            result.UriType = linkType;
            result.Meta = metaResult;

            return result;

            void PrepareResult(
                IEnumerable<(string, string)> matches,
                string qs,
                Action<string, string> adder,
                string qsPrefix = QsPrefix
            )
            {
                foreach (var (name, value) in matches)
                {
                    adder(name, value);
                }

                var qsParsed = HttpUtility.ParseQueryString(qs);

                if (qsParsed.Count <= 0)
                    return;

                foreach (var s in qsParsed.AllKeys)
                {
                    adder($"{qsPrefix}{s}", qsParsed[s]);
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
