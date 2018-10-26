namespace UriPathScanf
{
    /// <summary>
    /// URI parser, provides API to get metadata from URI paths
    /// </summary>
    public interface IUriPathScanf
    {
        /// <summary>
        /// Gets meta by URI path
        /// </summary>
        /// <param name="uriPath">URI path (w/o domain and proto)</param>
        /// <returns></returns>
        UriMetadata Scan(string uriPath);
    }
}
