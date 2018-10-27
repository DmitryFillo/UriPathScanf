using System;

namespace UriPathScanf.Attributes
{
    /// <inheritdoc cref="Attribute" />
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

        /// <summary>
        /// If true it's query string
        /// </summary>
        protected internal bool IsQueryString { get; }

        /// <inheritdoc />
        /// <summary>
        /// Populates property from URI path
        /// </summary>
        /// <param name="bindName">Name in the URI path, e.g. /{bindName}/...</param>
        public UriMetaAttribute(string bindName)
        {
            BindName = bindName;
        }

        /// <inheritdoc />
        /// <summary>
        /// Populates property from URI path
        /// </summary>
        /// <param name="bindName">Name in the URI path, e.g. /{bindName}/...</param>
        /// <param name="isQueryString">If true then match should be for query string</param>
        internal UriMetaAttribute(string bindName, bool isQueryString)
        {
            BindName = bindName;
            IsQueryString = isQueryString;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Populates property from URI query string
    /// </summary>
    public class UriMetaQueryAttribute : UriMetaAttribute
    {
        /// <inheritdoc />
        /// <summary>
        /// Populates property from URI query string
        /// </summary>
        /// <param name="bindName">Name in the URI path, e.g. /{bindName}/...</param>
        public UriMetaQueryAttribute(string bindName) : base(bindName, true)
        {

        }
    }
}
