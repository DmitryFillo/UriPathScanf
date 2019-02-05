using System;
using System.Collections.Generic;
using System.Text;

namespace UriPathScanf
{
    public interface IUriPathScanf
    {
        object Scan(string uriPath);
        T Scan<T>(string uriPath) where T : class;
        IDictionary<string, string> ScanDyn(string uriPath);
    }
}
