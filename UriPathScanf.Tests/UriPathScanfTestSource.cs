using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UriPathScanf.Attributes;

namespace UriPathScanf.Tests
{
    [ExcludeFromCodeCoverage]
    internal static class UriPathScanfTestSource
    {
        public static IEnumerable NonTypedMetaTestCases
        {
            get
            {
                // Arrange
                var descr = new UriPathDescriptor("/shop/{varOne}/{varTwo}/", "testLinkTwo");
                var descriptors = new[]
                {
                    new UriPathDescriptor("/shop/sales/{varOne}/{varTwo}/", "testLinkOne"),
                    new UriPathDescriptor("/sales/{varOne}/{varTwo}/", "testLinkOne"),

                    // NOTE: test for duplicates, it should be OK
                    descr,
                    descr,

                    // NOTE: should be escaped, because key is format URI path
                    new UriPathDescriptor("/shop/sales/{varOne}/{varTwo}/", "testLinkOne2"),
                    new UriPathDescriptor("/sales/{varOne}/{varTwo}/", "testLinkOne2"),
                };

                // Test case data
                yield return new TestCaseData(
                    descriptors,
                    "/shop/sales/some-ident/second-ident",
                    new UriMetadata()
                    {
                        UriType = "testLinkOne",
                        Meta = new Dictionary<string, string>
                        {
                            { "varOne", "some-ident" },
                            { "varTwo", "second-ident" }
                        }
                    }
                ).SetName("Check URI path without trailing slash for case when double match can occur");

                yield return new TestCaseData(
                    descriptors,
                    "/sales/some-ident/second-ident/",
                    new UriMetadata
                    {
                        UriType = "testLinkOne",
                        Meta = new Dictionary<string, string>
                        {
                            { "varOne", "some-ident" },
                            { "varTwo", "second-ident" }
                        }
                    }
                ).SetName("Check URI path with trailing slash for case when only one match can occur #1");

                yield return new TestCaseData(
                    descriptors,
                    "/shop/some-ident/second-ident/",
                    new UriMetadata
                    {
                        UriType = "testLinkTwo",
                        Meta = new Dictionary<string, string>
                        {
                            { "varOne", "some-ident" },
                            { "varTwo", "second-ident" }
                        }
                    }
                ).SetName("Check URI path with trailing slash for case when only one match can occur #2");

                yield return new TestCaseData(
                    descriptors,
                    "xx/shop/some-ident/second-ident/",
                    null
                ).SetName("Check URI path with trailing slash for case when no such URL");

                yield return new TestCaseData(
                    descriptors,
                    "/shop/some-ident/second-ident/?a=second-ident",
                    new UriMetadata
                    {
                        UriType = "testLinkTwo",
                        Meta = new Dictionary<string, string>
                        {
                            { "varOne", "some-ident" },
                            { "varTwo", "second-ident" },
                            { "qs__a", "second-ident" }
                        }
                    }
                ).SetName("Check URI path with trailing slash for case with query string");

                yield return new TestCaseData(
                    descriptors,
                    "/shop/some-ident/second-ident?a=second-ident",
                    new UriMetadata
                    {
                        UriType = "testLinkTwo",
                        Meta = new Dictionary<string, string>
                        {
                            { "varOne", "some-ident" },
                            { "varTwo", "second-ident" },
                            { "qs__a", "second-ident" }
                        }
                    }
                ).SetName("Check URI path without trailing slash for case with query string");

                yield return new TestCaseData(
                    descriptors,
                    "/shop/some-ident/second-ident?a=second-ident&a=b",
                    new UriMetadata
                    {
                        UriType = "testLinkTwo",
                        Meta = new Dictionary<string, string>
                        {
                            { "varOne", "some-ident" },
                            { "varTwo", "second-ident" },
                            { "qs__a", "second-ident,b" }
                        }
                    }
                ).SetName("Check URI path without trailing slash for case with query string with multiple values for one variable");

                yield return new TestCaseData(
                    descriptors,
                    "/shop/some-ident/second-ident?varOne=second-ident",
                    new UriMetadata
                    {
                        UriType = "testLinkTwo",
                        Meta = new Dictionary<string, string>
                        {
                            { "varOne", "some-ident" },
                            { "varTwo", "second-ident" },
                            { "qs__varOne", "second-ident" }
                        }
                    }
                ).SetName("Check URI path without trailing slash for case with query string has parameter with the same name as in linkFormat");

                yield return new TestCaseData(
                    descriptors,
                    "/sHOP/some-ident/second-ident?varOne=second-ident",
                    new UriMetadata
                    {
                        UriType = "testLinkTwo",
                        Meta = new Dictionary<string, string>
                        {
                            { "varOne", "some-ident" },
                            { "varTwo", "second-ident" },
                            { "qs__varOne", "second-ident" }
                        }
                    }
                ).SetName("Check URI path with ignore case");
            }
        }

        public static IEnumerable TypedMetaTestCases
        {
            get
            {
                // Arrange
                var descr = new UriPathDescriptor("/shop/selas/{varOne}/{varTwo}/x/{varInherit}//",
                    "testLinkWithSameMetadata", typeof(TestTypedMetadata));

                var descriptors = new[]
                {
                    new UriPathDescriptor("/shop/sales/{varOne}/{varTwo}/x/{varInherit}//", "testLink", typeof(TestTypedMetadata)),

                    // NOTE: test for duplicates, it should be OK
                    descr,
                    descr,

                    new UriPathDescriptor("/shop/sales/{varOne}/{varTwo}/y/{varInheritTwo}//", "testLinkAttrInheritance", typeof(TestTypedMetadata)),

                    // NOTE: should be escaped, because key is format URI path
                    new UriPathDescriptor("/shop/sales/{varOne}/{varTwo}/x/{varInherit}//", "testLink2", typeof(TestTypedMetadata)),

                    // NOTE: duplicates var name, should be first, also uses "_" for placeholder (no such var in the model)
                    new UriPathDescriptor("/shop/{_}/{varOne}/{varTwo}/{varTwo}/{varInherit}//", "testLink2", typeof(TestTypedMetadata)),
                };

                // Test case data
                yield return new TestCaseData(
                    descriptors,
                    "/shop/sales/some-ident/second-ident/x/a?a=3",
                    new UriMetadata
                    {
                        UriType = "testLink",
                        Meta = new TestTypedMetadata
                        {
                            VarOne = "some-ident",
                            VarTwo = "second-ident",
                            A = "3"
                        }
                    }
                ).SetName("Check typed meta for case with query string");

                yield return new TestCaseData(
                    descriptors,
                    "/shop/sales/some-ident/second-ident/x/three-ident",
                    new UriMetadata
                    {
                        UriType = "testLink",
                        Meta = new TestTypedMetadata
                        {
                            VarOne = "some-ident",
                            VarTwo = "second-ident",
                        }
                    }
                ).SetName("Check typed meta for case without query string");

                yield return new TestCaseData(
                    descriptors,
                    "/sshop/sales/some-ident/second-ident/x",
                    null
                ).SetName("Check typed meta for case when no match #1");

                yield return new TestCaseData(
                    descriptors,
                    "/shop/sales/some-ident/second-ident/",
                    null
                ).SetName("Check typed meta for case when no match #2");

                yield return new TestCaseData(
                    descriptors,
                    "/shop/sales/some-ident/second-ident/three-ident",
                    null
                ).SetName("Check typed meta for case when no match #3");

                yield return new TestCaseData(
                    descriptors,
                    "/shop/sales/3/2/x/1/?b=132",
                    new UriMetadata
                    {
                        UriType = "testLink",
                        Meta = new TestTypedMetadata
                        {
                            VarOne = "3",
                            VarTwo = "2",
                            B = 132.ToString()
                        }
                    }
                ).SetName("Check typed meta for case without not query string params and assigning to the object");

                yield return new TestCaseData(
                    descriptors,
                    "/shop/selas/3/2/x/1/////?b=132",
                    new UriMetadata
                    {
                        UriType = "testLinkWithSameMetadata",
                        Meta = new TestTypedMetadata
                        {
                            VarOne = "3",
                            VarTwo = "2",
                            B = 132.ToString()
                        }
                    }
                ).SetName("Check typed meta for another type with same metadata model and with many trailing slashes");

                yield return new TestCaseData(
                    descriptors,
                    "/shop/sales/3/2/y/1?b=132",
                    new UriMetadata
                    {
                        UriType = "testLinkAttrInheritance",
                        Meta = new TestTypedMetadata
                        {
                            VarOne = "3",
                            VarTwo = "2",
                            B = 132.ToString()
                        }
                    }
                ).SetName("Check typed meta attribute inheritance");

                yield return new TestCaseData(
                    descriptors,
                    "/shopp/lases/some-ident/second-ident/x/three-ident",
                    null
                ).SetName("Check typed meta for case when structure of URI path is the same as in the descriptor, but static parts of path are different");

                yield return new TestCaseData(
                    descriptors,
                    "/shop/xxxx/some-ident/second-ident/yet_another_ident/a?a=3",
                    new UriMetadata
                    {
                        UriType = "testLink2",
                        Meta = new TestTypedMetadata
                        {
                            VarOne = "some-ident",
                            VarTwo = "second-ident",
                            A = "3"
                        }
                    }
                ).SetName("Check typed meta for case when placeholder is used and duplicate var name in the format string");
            }
        }

        public class TestTypedMetadataBase
        {
            [UriMeta("varInherit")]
            public string VarInherit { get; set; }

            [UriMeta("varInheritTwo")]
            public string VarInheritTwo { get; set; }
        }

        public class TestTypedMetadata : TestTypedMetadataBase, IEquatable<TestTypedMetadata>
        {
            [UriMeta("varOne")]
            public string VarOne { get; set; }

            [UriMeta("varTwo")]
            public string VarTwo { get; set; }

            [UriMetaQuery("a")]
            public string A { get; set; }

            [UriMeta("qs__b")]
            public object B { get; set; }

            /// <summary>
            /// Attribute inheritance, will work OK
            /// </summary>
            public new string VarInheritTwo { get; set; }

            public bool Equals(TestTypedMetadata other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(VarOne, other.VarOne) && string.Equals(VarTwo, other.VarTwo) &&
                       string.Equals(A, other.A) && Equals(B, other.B) &&
                       string.Equals(VarInheritTwo, other.VarInheritTwo);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((TestTypedMetadata) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (VarOne != null ? VarOne.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (VarTwo != null ? VarTwo.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (A != null ? A.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (B != null ? B.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (VarInheritTwo != null ? VarInheritTwo.GetHashCode() : 0);
                    return hashCode;
                }
            }

            public static bool operator ==(TestTypedMetadata left, TestTypedMetadata right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(TestTypedMetadata left, TestTypedMetadata right)
            {
                return !Equals(left, right);
            }
        }
        
    }
}
