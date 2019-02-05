using System;
using System.Collections.Generic;
using System.Reflection;
using UriPathScanf.Exceptions;

namespace UriPathScanf
{
    /// <summary>
    /// Factories configuration
    /// </summary>
    public class UriPathConfiguration
    {
        public UriPathConfiguration()
        {
        }

        public UriPathConfiguration(Action<UriPathConfiguration> cfg)
        {
            cfg(this);
        }

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

            _attrs.Add((attr, type, DescriptorFactory.GetFactory<T>()));

            return this;
        }

        /// <summary>
        /// Add format that can be resolved as dictionary.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public UriPathConfiguration Add(string format)
        {
            _dynamicDeclaredFormats.Add(format);
            return this;
        }
        
        public IEnumerable<(UriPathAttribute, Type, Func<IDictionary<string, string>, object>)> Attributes => _attrs;
        public IEnumerable<string> DynamicDeclaredFormats => _dynamicDeclaredFormats;

        private readonly ICollection<(UriPathAttribute, Type, Func<IDictionary<string, string>, object>)> _attrs =
            new List<(UriPathAttribute, Type, Func<IDictionary<string, string>, object>)>();
        private readonly ICollection<string> _dynamicDeclaredFormats = new List<string>();
    }
}
