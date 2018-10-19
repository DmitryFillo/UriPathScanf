using System;

namespace UriPathScanf
{
    /// <summary>
    /// 
    /// </summary>
    public class LinkDescriptor
    {
        public LinkDescriptor(string linkType, string linkFormat, Type meta)
        {
            Type = linkType;
            Format = linkFormat;
            Meta = meta;
        }

        public LinkDescriptor(string linkType, string linkFormat)
        {
            Type = linkType;
            Format = linkFormat;
            Meta = null;
        }

        public string Type { get; }
        public string Format { get; }
        public Type Meta { get; }
    }
}
