using System;
using System.Collections.Generic;
using System.Reflection;
using UriPathScanf.Exceptions;
using UriPathScanf.Internal;

namespace UriPathScanf
{
    /// <summary>
    /// Factories configuration
    /// </summary>
    public class UriPathConfiguration
    {
        /// <summary>
        /// Add model to the configuration
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <returns></returns>
        /// <exception cref="ProvideUriPathAttributeException">Occurs when no attribute with URI path format is declared on the given model</exception>
        public UriPathConfiguration Add<T>() where T: class, new()
        {
            var type = typeof(T);
          
            var attr = type.GetTypeInfo().GetCustomAttribute<UriPathAttribute>();

            if (attr == null)
            {
                throw new ProvideUriPathAttributeException(
                    $"Your type {type.FullName} should have {nameof(UriPathAttribute)} to be allowed as model for {nameof(UriPathScanf)}");
            }

            _attrs.Add((attr, type));
            _factories.Add(type, DescriptorFactory.GetFactory<T>());

            return this;
        }

        /// <summary>
        /// Set delimiter for URI paths. Default is "/"
        /// </summary>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public UriPathConfiguration SetDelimiter(char delimiter)
        {
            Delimiter = delimiter;

            return this;
        }

        /// <summary>
        /// Delimiter of the URI path
        /// </summary>
        public char Delimiter { get; private set; } = '/';

        internal IReadOnlyDictionary<Type, Func<IDictionary<string, string>, object>> Factories => _factories;

        internal IEnumerable<(UriPathAttribute, Type)> Attributes => _attrs;

        private readonly IList<(UriPathAttribute, Type)> _attrs = new List<(UriPathAttribute, Type)>();
        private readonly Dictionary<Type, Func<IDictionary<string, string>, object>> _factories = 
            new Dictionary<Type, Func<IDictionary<string, string>, object>>();
    }
}
