using System.Collections.Generic;

namespace UriPathScanf
{
    /// <summary>
    /// URI parser, provides API to get metadata from URI paths
    /// </summary>
    public interface IUriPathScanf
    {
        /// <summary>
        /// Gets meta by URI path (all descriptors)
        /// </summary>
        /// <param name="uriPath">URI path (w/o domain and proto)</param>
        /// <returns></returns>
        UriMetadata ScanAll(string uriPath);

        /// <summary>
        /// Gets meta by URI path (only typed descriptors)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uriPath"></param>
        /// <returns></returns>
        UriMetadata<T> Scan<T>(string uriPath) where T : class, IUriPathMetaModel;

        /// <summary>
        /// Gets meta by URI path (only non-typed descriptors)
        /// </summary>
        /// <param name="uriPath"></param>
        /// <returns></returns>
        UriMetadata<IDictionary<string, string>> Scan(string uriPath);
    }
}
