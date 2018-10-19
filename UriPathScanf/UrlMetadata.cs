using System;

namespace UriPathScanf
{
    public class UrlMetadata : IEquatable<UrlMetadata>
    {
        public bool Equals(UrlMetadata other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(UrlType, other.UrlType) && Equals(Meta, other.Meta);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UrlMetadata) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((UrlType != null ? UrlType.GetHashCode() : 0) * 397) ^ (Meta != null ? Meta.GetHashCode() : 0);
            }
        }

        public static bool operator ==(UrlMetadata left, UrlMetadata right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UrlMetadata left, UrlMetadata right)
        {
            return !Equals(left, right);
        }

        public string UrlType { get; set; }

        public object Meta { get; set; }
    }
}
