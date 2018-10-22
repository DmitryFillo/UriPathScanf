using System;

namespace UriPathScanf
{
    /// <summary>
    /// Describes URI path
    /// </summary>
    public class UriPathDescriptor
    {
        /// <summary>
        /// Creates instances of URI path descriptors
        /// </summary>
        /// <param name="type">Type of given URI path</param>
        /// <param name="format">Format (<see cref="M:string.Format"/> reversed)</param>
        /// <param name="meta">Model that will be populated from given URI paths</param>
        public UriPathDescriptor(string type, string format, Type meta)
        {
            Type = type;
            Format = format;
            Meta = meta;
        }

        /// <summary>
        /// Creates instance of URI path descriptor
        /// </summary>
        /// <param name="type">Type of given URI path</param>
        /// <param name="format">Format (<see cref="M:string.Format"/> reversed)</param>
        public UriPathDescriptor(string type, string format)
        {
            Type = type;
            Format = format;
            Meta = null;
        }

        /// <summary>
        /// Type of descriptor (user defined)
        /// </summary>
        protected internal string Type { get; }

        /// <summary>
        /// Descriptor format (<see cref="M:string.Format"/> reversed)
        /// </summary>
        protected internal string Format { get; }

        /// <summary>
        /// <see cref="T:Type"/> of model to be populated
        /// </summary>
        protected internal Type Meta { get; }
    }
}
