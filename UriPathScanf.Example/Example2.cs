using System.Diagnostics;
using UriPathScanf.Attributes;

namespace UriPathScanf.Example
{
    internal static class Example2
    {
        /// <summary>
        /// Example using <see cref="UriPathScanf.Scan"/>, <see cref="UriPathScanf.Scan{T}"/> and <see cref="UriPathScanf.ScanDict"/>
        /// </summary>
        public static void Go()
        {
            var descriptors = new[]
            {
                new UriPathDescriptor("/path/some/{varOne}", "varOneLink"),
                new UriPathDescriptor("/path/some/{someVar}/{someVar2}", "varTwoLink", typeof(Meta)),
                new UriPathDescriptor("/path/some/{someVar}/{someVar2}/{someVar3}", "varTwoLink", typeof(Meta)),
            };

            var uriPathScanf = new UriPathScanf(descriptors);

            // typed scan
            foreach (var u in new []
            {
                "/path/some/12314/xxx/",
                "/path/some/1/2/3x/",
            })
            {
                var result = uriPathScanf.Scan<Meta>(u);

                Debug.Assert(!string.IsNullOrEmpty(result.Meta.SomeVar));
                Debug.Assert(!string.IsNullOrEmpty(result.Meta.SomeVar2));
                Debug.Assert(string.IsNullOrEmpty(result.Meta.SomeVarQueryString));
            }

            // typed scan and not found
            var resultNonMeta = uriPathScanf.Scan<Meta>("/path/some/3");
            Debug.Assert(resultNonMeta == null);

            // dict scan and found
            var resultMetaDict = uriPathScanf.ScanDict("/path/some/3");
            Debug.Assert(resultMetaDict.Meta["varOne"] == "3");

            // dict scan and not found
            var resultNotDict = uriPathScanf.ScanDict("/path/some/3/4");
            Debug.Assert(resultNotDict == null);
        }

        internal class Meta : IUriPathMetaModel
        {
            [UriMeta("someVar")]
            public string SomeVar { get; set; }

            [UriMeta("someVar2")]
            public string SomeVar2 { get; set; }

            [UriMetaQuery("someVar")]
            public string SomeVarQueryString { get; set; }
        }
    }
}
