using System;

namespace UriPathScanf.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    /// Occurs when no <see cref="T:UriPathScanf.UriPathAttribute" /> defined on model
    /// </summary>
    public class ProvideUriPathAttributeException : Exception
    {
        /// <inheritdoc />
        public ProvideUriPathAttributeException()
        {
        }

        /// <inheritdoc />
        public ProvideUriPathAttributeException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        public ProvideUriPathAttributeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
