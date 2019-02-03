using System;
using System.Collections.Generic;
using System.Linq;
using UriPathScanf.Internal;

namespace UriPathScanf
{
    /// <summary>
    /// URI path scanf utility
    /// </summary>
    public class UriPathScanf : IUriPathScanf
    {
        private readonly UriPathConfiguration _uriPathConfiguration = new UriPathConfiguration();

        /// <summary>
        /// Create URI path scanf instance.
        /// </summary>
        /// <param name="configurationFactory">Configuration factory</param>
        /// <exception cref="ArgumentException">If no configuration factory provided</exception>
        public UriPathScanf(Action<UriPathConfiguration> configurationFactory)
        {
            if (configurationFactory == null)
            {
                throw new ArgumentException("You should provide configuration factory delegate");
            }

            configurationFactory(_uriPathConfiguration);
        }

        public object Scan(string uriPath)
        {
            var matchResult = Match(uriPath);

            if (matchResult == null) return null;

            var (type, attrs) = matchResult.Value;

            return Factories[type](attrs);
        }

        public T Scan<T>(string uriPath) where T: class
        {
            var matchResult = Match(uriPath);

            if (matchResult == null) return null;

            var (type, attrs) = matchResult.Value;

            return typeof(T) == type ? (T)Factories[type](attrs) : null;
        }

        // TODO: add query string
        /// <summary>
        /// Scan given URI path across registered descriptors
        /// </summary>
        /// <param name="uriPath"></param>
        /// <returns></returns>
        protected virtual (Type, IDictionary<string, string>)? Match(string uriPath)
        {
            // TODO: support relative paths
            var uri = new Uri(uriPath, UriKind.RelativeOrAbsolute);

            var segments = uri
                .Segments
                .Skip(1)
                .Select(s => s.TrimEnd(_uriPathConfiguration.Delimiter))
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

           
            foreach (var (attr, type) in Attributes)
            {
                var scheme = GetUriPathFormat(attr).ToArray();

                if (segments.Length != scheme.Length)
                {
                    return null;
                }

                var placeholderValues = scheme
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

                if (placeholderValues.Any())
                {
                    return (type, placeholderValues);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets URI path format from given attribute using format and names
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        protected virtual IEnumerable<string> GetUriPathFormat(UriPathAttribute attr)
        {
            using (var enumerator = attr.Names.GetEnumerator())
            {
                foreach (var v in attr.Format.Split(_uriPathConfiguration.Delimiter).Skip(1).Select((p, i) =>
                {
                    if (!p.IsPlaceholder()) return p;

                    enumerator.MoveNext();

                    return enumerator.Current.ToPlaceholderVariable();

                }))
                {
                    yield return v;
                }
            }
        }

        /// <summary>
        /// Gets attributes from <see cref="UriPathConfiguration"/>
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<(UriPathAttribute, Type)> Attributes => _uriPathConfiguration.Attributes;

        /// <summary>
        /// Gets factories from <see cref="UriPathConfiguration"/>
        /// </summary>
        /// <returns></returns>
        protected IReadOnlyDictionary<Type, Func<IDictionary<string, string>, object>> Factories => _uriPathConfiguration.Factories;
    }
}
