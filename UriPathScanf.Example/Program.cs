using System.Collections.Generic;
using System.Diagnostics;
using UriPathScanf.Attributes;

namespace UriPathScanf.Example
{
    internal class Meta
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

            // NOTE: dict result
            var meta = uriPathScanf.Scan("/path/some/12314?x=123");
            var metaResult = (IDictionary<string, string>) meta.Meta;

            Debug.Assert(metaResult["varOne"] == "12314");
            Debug.Assert(metaResult["qs__x"] == "123");
            Debug.Assert(meta.UriType == "varOneLink");

            // NOTE: meta model result
            var metaModel = uriPathScanf.Scan("/path/some/123/321/?x=123&someVar=1&someVar2=2");
            var metaResultModel = (Meta) metaModel.Meta;

            Debug.Assert(metaResultModel.SomeVar == "123");
            Debug.Assert(metaResultModel.SomeVarQueryString == "1");
            Debug.Assert(metaResultModel.SomeVar2 == "321");
            Debug.Assert(metaModel.UriType == "varTwoLink");
        }
    }
}
