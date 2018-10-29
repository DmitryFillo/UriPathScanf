using System;
using System.Collections.Generic;

namespace UriPathScanf
{
    /// <inheritdoc />
    /// <summary>
    /// Describes URI path
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UriMetadata<T> : IEquatable<UriMetadata<T>>
    {
        /// <summary>
        /// Creates URI metadata instance
        /// </summary>
        /// <param name="uriType">Type of URI path (user defined)</param>
        /// <param name="meta">URI path metadata model</param>
        public UriMetadata(string uriType, T meta)
        {
            UriType = uriType;
            Meta = meta;
        }

        /// <summary>
        /// Type of URI path (user defined)
        /// </summary>
        public string UriType { get; }

        /// <summary>
        /// URI path metadata model
        /// </summary>
        public T Meta { get; }


        /// <inheritdoc />
        public virtual bool Equals(UriMetadata<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(UriType, other.UriType) && EqualityComparer<T>.Default.Equals(Meta, other.Meta);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((UriMetadata<T>) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((UriType != null ? UriType.GetHashCode() : 0) * 397) ^ EqualityComparer<T>.Default.GetHashCode(Meta);
            }
        }

        /// <summary>
        /// Equals operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(UriMetadata<T> left, UriMetadata<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Not equals operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(UriMetadata<T> left, UriMetadata<T> right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Cast to <see cref="UriMetadata{T}"/>
        /// </summary>
        /// <param name="v"></param>
        public static explicit operator UriMetadata<T>(UriMetadata v)
        {
            return new UriMetadata<T>(v.UriType, (T)v.Meta);
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Describes URI path
    /// </summary>
    public class UriMetadata : UriMetadata<object>
    {
        /// <summary>
        /// Type of <see cref="UriMetadata{T}.Meta"/>
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Converts <see cref="UriMetadata{T}.Meta"/> to <see cref="IDictionary{TKey,TValue}"/>.
        /// If it's typed meta then null will be returned
        /// </summary>
        /// <returns></returns>
        public bool TryCastToDict(out IDictionary<string, string> result)
        {
            if (!(Meta is IDictionary<string, string> casted))
            {
                result = null;
                return false;
            }

            result = casted;
            return true;
        } 

        /// <summary>
        /// Cast <see cref="UriMetadata{T}.Meta"/> to type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool TryCast<T>(out T result) where T : class, IUriPathMetaModel
        {
            if (!(Meta is T casted))
            {
                result = null;
                return false;
            }

            result = casted;
            return true;
        }

        /// <inheritdoc />
        public UriMetadata(string uriType, object meta) : base(uriType, meta)
        {

        }
    }
}
