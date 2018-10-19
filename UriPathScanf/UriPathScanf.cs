using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using UriPathScanf.Attributes;

namespace UriPathScanf
{
    public class UriPathScanf : IUriPathScanf
    {
        private readonly Regex _placeholderRegex = new Regex(@"\{(\w+)\}");
        private readonly Regex _slashRegex = new Regex(@"/+");

        private readonly LinkDescriptor[] _descriptors;
        private readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _methods
            = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        public UriPathScanf(IEnumerable<LinkDescriptor> descriptors)
        {
            // NOTE: to search longest link format first
            _descriptors = descriptors.OrderByDescending(d => d.Format.Split('/').Length).ToArray();

            // NOTE: only for case when we need result model
            foreach (var d in _descriptors.Where(d => d.Meta != null))
            {
                var result = new Dictionary<string, PropertyInfo>();

                var stringProps = d.Meta
                    .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.PropertyType == typeof(string));

                foreach (var m in stringProps)
                {
                    var attrs = m.GetCustomAttributes(true);

                    var attr = attrs.OfType<UrlMetaAttribute>().FirstOrDefault();
                    if (attr != null)
                    {
                        result.Add(attr.BindName, m);
                    }
                }

                _methods.Add(d.Meta, result);
            }
        }

        public UrlMetadata GetMetadata(string link)
        {
            var result = new UrlMetadata();

            var match = FindMatch(link);

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

                result.UrlType = linkType;
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
            
            result.UrlType = linkType;
            result.Meta = metaResult;

            return result;
        }

        protected (LinkDescriptor, IEnumerable<(string, string)>, string)? FindMatch(string link)
        {
            var qsName = "__qs__";

            foreach (var descr in _descriptors)
            {
                var format = descr.Format;

                var groups = new HashSet<string>();

                var regexString = _placeholderRegex
                    .Replace(format, m =>
                    {
                        var g = m.Groups[1].Value;
                        groups.Add(g);
                        return "(?<" + g + ">.+)";
                    }).TrimEnd('/');

                if (groups.Contains(qsName))
                {
                    qsName += Math.Abs(qsName.GetHashCode());
                }

                // NOTE: right to left search regexp, so it starts with ^
                regexString = _slashRegex.Replace(regexString, m => "/+");
                regexString = $"^{regexString}";
                regexString += $@"/*(?<{qsName}>\?+.+)*";
                var formatRegex = new Regex(regexString, RegexOptions.RightToLeft);

                var matches = formatRegex.Match(link);

                if (!matches.Success)
                    continue;

                var urlMatches = formatRegex.GetGroupNames().Where(g => g != qsName && g != "0").Select(m => (m, matches.Groups[m].Value));
                var queryString = matches.Groups[qsName].Value;

                return (descr, urlMatches, queryString);
            }

            return null;
        }

        protected static void PrepareResult(
            IEnumerable<(string, string)> urlMatches,
            string queryString,
            Action<string, string> adder,
            string qsPrefix = "qs__"
        )
        {
            foreach (var (name, value) in urlMatches)
            {
                adder(name, value);
            }

            var qsParsed = HttpUtility.ParseQueryString(queryString);

            if (qsParsed.Count <= 0)
                return;

            foreach (var s in qsParsed.AllKeys)
            {
                adder($"{qsPrefix}{s}", qsParsed[s]);
            }
        }
    }
}
