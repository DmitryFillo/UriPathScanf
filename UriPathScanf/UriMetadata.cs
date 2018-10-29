using System;
using System.Collections;
using System.Collections.Generic;

namespace UriPathScanf
{
    /// <inheritdoc />
    /// <summary>
    /// Describes URI path
    /// </summary>
    public class UriMetadata : IEquatable<UriMetadata>
    {
        /// <summary>
        /// Type of URI path (user defined)
        /// </summary>
        public string UriType { get; }

        /// <summary>
        /// URI path metadata model
        /// </summary>
        public object Meta { get; }

        /// <summary>
        /// Type of <see cref="Meta"/>
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Converts <see cref="Meta"/> to dictionary. If it's typed meta then null will be returned
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> AsDictionary => Meta as IDictionary<string, string>;

        /// <summary>
        /// Cast <see cref="Meta"/> to type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T As<T>() where T: class, IUriPathMetaModel => Meta as T;

        /// <summary>
        /// Creates URI metadata instance
        /// </summary>
        /// <param name="uriType">Type of URI path (user defined)</param>
        /// <param name="meta">URI path metadata model</param>
        public UriMetadata(string uriType, object meta)
        {
            UriType = uriType;
            Meta = meta;
        }

        /// <inheritdoc />
        public virtual bool Equals(UriMetadata other)
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
            return obj.GetType() == GetType() && Equals((UriMetadata) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((UriType != null ? UriType.GetHashCode() : 0) * 397) ^ (Meta != null ? Meta.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Equals operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(UriMetadata left, UriMetadata right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Not equals operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(UriMetadata left, UriMetadata right)
        {
            return !Equals(left, right);
        }
    }
}
