using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace UriPathScanf.Example
{
    [UriPath("/some/path/{}/x", nameof(PropOne))]
    internal class ExampleDescriptor
    {
        public string PropOne { get; set; }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var u = new UriPathScanf(new UriPathConfiguration(cfg =>
            {
                cfg.Add<ExampleDescriptor>();
                cfg.Add("/some/path2/{test}/x");
            }));

            //var r = u.Scan<ExampleDescriptor>("https://xxx.com/some/path/55/x////");
            var r2 = u.ScanDyn("/////some///path2/55/x////");
        }
    }
}
