namespace UriPathScanf
{
    /// <summary>
    /// URL parser.
    /// Provides API to get metadata by URL itself.
    /// </summary>
    public interface IUriPathScanf
    {
        /// <summary>
        /// Gets Metadata by URL.
        /// </summary>
        /// <param name="link">URL</param>
        /// <returns></returns>
        UrlMetadata GetMetadata(string link);
    }
}
