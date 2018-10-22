using System;

namespace UriPathScanf.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Populates property from URI path
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UriMetaAttribute : Attribute
    {
        /// <summary>
        /// Specifies name to bind
        /// </summary>
        protected internal string BindName { get; }

        /// <inheritdoc />
        /// <summary>
        /// Populates property from URI path
        /// </summary>
        /// <param name="bindName">Name in the URI path, e.g. /{bindName}/...</param>
        public UriMetaAttribute(string bindName)
        {
            BindName = bindName;
        }
    }
}
