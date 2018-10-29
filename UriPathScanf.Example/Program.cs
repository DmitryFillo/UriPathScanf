using System.Collections.Generic;
using System.Diagnostics;
using UriPathScanf.Attributes;

namespace UriPathScanf.Example
{
    internal class Meta : IUriPathMetaModel
    {
        [UriMeta("someVar")]
        public string SomeVar { get; set; }

        [UriMeta("someVar2")]
        public string SomeVar2 { get; set; }

        [UriMetaQuery("someVar")]
        public string SomeVarQueryString { get; set; }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var descriptors = new []
            {
                new UriPathDescriptor("/path/some/{varOne}", "varOneLink"),
                new UriPathDescriptor("/path/some/{someVar}/{someVar2}", "varTwoLink", typeof(Meta)), 
            };

            var uriPathScanf = new UriPathScanf(descriptors);

            var result = uriPathScanf.Scan("/path/some/12314?x=123");

            // pattern matching
            switch (result.Meta)
            {
                case IDictionary<string, string> m:
                    ProcessDictionary(result, m);
                    break;
                case Meta m:
                    ProcessDictionary(result, m);
                    break;
            }
        }

        private static void ProcessDictionary(UriMetadata result, IDictionary<string, string> metaResult)
        {
            Debug.Assert(metaResult["varOne"] == "12314");
            Debug.Assert(metaResult["qs__x"] == "123");
            Debug.Assert(result.UriType == "varOneLink");
        }

        private static void ProcessDictionary(UriMetadata result, Meta m)
        {
            Debug.Assert(m.SomeVar == "123");
            Debug.Assert(m.SomeVarQueryString == "1");
            Debug.Assert(m.SomeVar2 == "321");
            Debug.Assert(result.UriType == "varTwoLink");
        }
    }
}
