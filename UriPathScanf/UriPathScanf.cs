using System;
using System.Collections.Generic;
using System.Linq;

namespace UriPathScanf
{
    /// <summary>
    /// URI path scanf utility
    /// </summary>
    public class UriPathScanf : IUriPathScanf
    {

        /// <summary>
        /// Gets URI path configuration
        /// </summary>
        private readonly UriPathConfiguration _uriPathConfiguration;

        /// <summary>
        /// Create URI path scanf instance.
        /// </summary>
        /// <param name="configurationFactory">Configuration factory</param>
        /// <exception cref="ArgumentException">If no configuration factory provided</exception>
        public UriPathScanf(UriPathConfiguration uriPathConfiguration)
        {
            _uriPathConfiguration = uriPathConfiguration;
        }

        public object Scan(string uriPath)
        {
            var checker = GetMatchChecker(uriPath);

            foreach (var (attr, _, fac) in _uriPathConfiguration.Attributes)
            {
                var scheme = attr.GetUriPathFormat();

                var result = checker(scheme);

                return fac(result);
            }

            return null;
        }

        public T Scan<T>(string uriPath) where T: class
        {
            var checker = GetMatchChecker(uriPath);

            foreach (var (attr, type, fac) in _uriPathConfiguration.Attributes)
            {
                var scheme = attr.GetUriPathFormat();

                var result = checker(scheme);

                return typeof(T) == type ? (T)fac(result) : null;
            }

            return null;
        }

        public IDictionary<string, string> ScanDyn(string uriPath)
        {
            var checker = GetMatchChecker(uriPath);

            return _uriPathConfiguration.DynamicDeclaredFormats.Select(format => format.Split('/').Skip(1))
                .Select(scheme => checker(scheme))
                .FirstOrDefault();
        }

        // TODO: query string
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uriPath"></param>
        /// <returns></returns>
        private static Func<IEnumerable<string>, IDictionary<string, string>> GetMatchChecker(string uriPath)
        {
            var uri = new Uri(uriPath, UriKind.RelativeOrAbsolute);

            if (!uri.IsAbsoluteUri)
            {
                uri = new Uri("http://_/" + uriPath);
            }

            // TODO: add query string support
            var query = uri.Query;

            var segments = uri
                .Segments
                .Skip(1)
                .Select(s => s.TrimEnd('/'))
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            return scheme =>
            {
                var schemeArray = scheme as string[] ?? scheme.ToArray();

                if (segments.Length != schemeArray.Length)
                {
                    return null;
                }

                var placeholderValues = schemeArray
                    .Zip(segments, (s, s1) =>
                    {
                        if (s == s1)
                        {
                            return null;
                        }

                        if (s.IsPlaceholderVariable())
                        {
                            return ((string PropName, string ExtractedValue)?)(s, s1);
                        }

                        return null;
                    })
                    .Where(x => x != null)
                    .ToDictionary(x => x.Value.PropName.GetNameOfPlaceholderVariable(), x => x.Value.ExtractedValue);

                return placeholderValues.Any() ? placeholderValues : null;
            };
        }
    }
}
