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

            var d = new UriPathConfiguration();
            d.Add<ExampleDescriptor>();

            var u = new UriPathScanf(i =>
            {
                i.Add<ExampleDescriptor>();
               
            });

            var r = u.Scan<ExampleDescriptor>("https://xxx.com/some/path/55/x////");
        }
    }
}
