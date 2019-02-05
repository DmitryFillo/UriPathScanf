using System;
using System.Collections.Generic;
using System.Linq;

namespace UriPathScanf
{
    /// <summary>
    /// Marks model as fetchable from URI path
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UriPathAttribute : Attribute
    {
        /// <summary>
        /// URI path format
        /// </summary>
        protected readonly string Format;

        /// <summary>
        /// Names of variable, should be the same as prop names in the model
        /// </summary>
        protected IEnumerable<string> Names { get; }

        /// <summary>
        /// <inheritdoc cref="UriPathAttribute"/>
        /// </summary>
        /// <param name="uriPathFormat">Format string, e.g. "/some/path/{}/xxx"</param>
        /// <param name="names">Names params in the same order as in the format string</param>
        public UriPathAttribute(string uriPathFormat, params string[] names)
        {
            Format = uriPathFormat;
            Names = names;
        }

        public virtual IEnumerable<string> GetUriPathFormat()
        {
            using (var enumerator = Names.GetEnumerator())
            {
                foreach (var v in Format.Split('/').Skip(1).Select((p, i) =>
                {
                    if (!p.IsUnboundPlaceholder()) return p;

                    enumerator.MoveNext();

                    return enumerator.Current.ToPlaceholderVariable();

                }))
                {
                    yield return v;
                }
            }
        }
    }
}
