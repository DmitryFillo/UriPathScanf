using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UriPathScanf.Utils
{
    public static class QueryStringParser
    {
        public static IDictionary<string, IEnumerable<string>> Parse(string qs)
        {
            // TODO: support percent encoding
            var lookup = qs
                .TrimStart('?')
                .Split(new [] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(kv => kv.Split('='))
                .ToLookup(kv => kv[0], kv => kv[1])
                .ToDictionary(g => g.Key, g => g.AsEnumerable());

            return lookup;
        }
    }
}
