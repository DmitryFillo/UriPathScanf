using System;

namespace UriPathScanf
{
    /// <inheritdoc />
    /// <summary>
    /// Describes URI path
    /// </summary>
    public class UriMetadata : IEquatable<UriMetadata>
    {
        /// <inheritdoc />
        public bool Equals(UriMetadata other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(UriType, other.UriType) && Equals(Meta, other.Meta);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UriMetadata) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((UriType != null ? UriType.GetHashCode() : 0) * 397) ^ (Meta != null ? Meta.GetHashCode() : 0);
            }
        }
         
        public static bool operator ==(UriMetadata left, UriMetadata right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UriMetadata left, UriMetadata right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Type of URI path (user defined)
        /// </summary>
        public string UriType { get; set; }

        /// <summary>
        /// URI path metadata model
        /// </summary>
        public object Meta { get; set; }
    }
}
