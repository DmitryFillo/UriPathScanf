using System;

namespace UriPathScanf
{
    /// <inheritdoc />
    /// <summary>
    /// Describes URI path
    /// </summary>
    public class UriPathDescriptor : IEquatable<UriPathDescriptor>
    {
        /// <summary>
        /// Creates instances of URI path descriptors
        /// </summary>
        /// <param name="type">Type of given URI path</param>
        /// <param name="format">Format (<see cref="M:string.Format"/> reversed)</param>
        /// <param name="meta">Model that will be populated from given URI paths</param>
        public UriPathDescriptor(string format, string type, Type meta)
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
        public UriPathDescriptor(string format, string type)
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

        /// <inheritdoc />
        public bool Equals(UriPathDescriptor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Format, other.Format);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((UriPathDescriptor) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() => (Format != null ? Format.GetHashCode() : 0);

        /// <summary>
        /// Equals operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(UriPathDescriptor left, UriPathDescriptor right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Not equals operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(UriPathDescriptor left, UriPathDescriptor right)
        {
            return !Equals(left, right);
        }
    }
}
