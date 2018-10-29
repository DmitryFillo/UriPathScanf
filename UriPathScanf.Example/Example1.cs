using System.Collections.Generic;
using System.Diagnostics;
using UriPathScanf.Attributes;

namespace UriPathScanf.Example
{
    internal static class Example1
    {
        /// <summary>
        /// Example using scan and then casting or pattern matching
        /// </summary>
        public static void Go()
        {
            var descriptors = new[]
            {
                new UriPathDescriptor("/path/some/{varOne}", "varOneLink"),
                new UriPathDescriptor("/path/some/{someVar}/{someVar2}", "varTwoLink", typeof(Meta)),
            };

            var uriPathScanf = new UriPathScanf(descriptors);

            // typed and not typed matches
            foreach (var u in new []
            {
                "/path/some/12314?x=123",
                "/path/some/12314/xxx/?x=123"
            })
            {
                var result = uriPathScanf.Scan(u);

                // pattern matching
                switch (result.Meta)
                {
                    case IDictionary<string, string> m:
                        Assert(result, m);
                        break;
                    case Meta m:
                        Assert(result, m);
                        break;
                }

                // casting
                if (result.TryCast<Meta>(out var resultMeta)) Assert(result, resultMeta);
                if (result.TryCast(out var resultDict)) Assert(result, resultDict);
            }
        }

        private static void Assert(UriMetadata result, IDictionary<string, string> metaResult)
        {
            Debug.Assert(metaResult["varOne"] == "12314");
            Debug.Assert(metaResult["qs__x"] == "123");
            Debug.Assert(result.UriType == "varOneLink");
        }

        private static void Assert(UriMetadata result, Meta m)
        {
            Debug.Assert(m.SomeVar == "12314");
            Debug.Assert(m.SomeVarQueryString == "123");
            Debug.Assert(m.SomeVar2 == "xxx");
            Debug.Assert(result.UriType == "varTwoLink");
        }

        internal class Meta : IUriPathMetaModel
        {
            [UriMeta("someVar")]
            public string SomeVar { get; set; }

            [UriMeta("someVar2")]
            public string SomeVar2 { get; set; }

            [UriMetaQuery("x")]
            public string SomeVarQueryString { get; set; }
        }
    }
}
