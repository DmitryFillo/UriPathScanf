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
                var descriptors = new[]
                {
                    new UriPathDescriptor("testLinkOne", "/shop/sales/{varOne}/{varTwo}/"),
                    new UriPathDescriptor("testLinkOne", "/sales/{varOne}/{varTwo}/"),
                    new UriPathDescriptor("testLinkTwo", "/shop/{varOne}/{varTwo}/"),
                };

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
                ).SetName("Check URL without trailing slash for case when double match can occur");

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
                ).SetName("Check URL with trailing slash for case when only one match can occur #1");

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
                ).SetName("Check URL with trailing slash for case when only one match can occur #2");

                yield return new TestCaseData(
                    descriptors,
                    "xx/shop/some-ident/second-ident/",
                    null
                ).SetName("Check URL with trailing slash for case when no such URL");

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
                ).SetName("Check URL with trailing slash for case with query string");

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
                ).SetName("Check URL without trailing slash for case with query string");

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
                ).SetName("Check URL without trailing slash for case with query string with multiple values for one variable");

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
                ).SetName("Check URL without trailing slash for case with query string has parameter with the same name as in linkFormat");

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
                ).SetName("Check URL with ignore case");
            }
        }

        public static IEnumerable TypedMetaTestCases
        {
            get
            {
                var descriptors = new[]
                {
                    new UriPathDescriptor("testLink", "/shop/sales/{varOne}/{varTwo}/", typeof(TestTypedMetadata)),
                };

                yield return new TestCaseData(
                    descriptors,
                    "/shop/sales/some-ident/second-ident/?a=3",
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
                    "/shop/sales/some-ident/second-ident/",
                    new UriMetadata
                    {
                        UriType = "testLink",
                        Meta = new TestTypedMetadata
                        {
                            VarOne = "some-ident",
                            VarTwo = "second-ident",
                            A = null
                        }
                    }
                ).SetName("Check typed meta for case without query string");

                yield return new TestCaseData(
                    descriptors,
                    "/sshop/sales/some-ident/second-ident/",
                    null
                ).SetName("Check typed meta for case when no match");
            }
        }

        public class TestTypedMetadata : IEquatable<TestTypedMetadata>
        {
            [UriMeta("varOne")]
            public string VarOne { get; set; }

            [UriMeta("varTwo")]
            public string VarTwo { get; set; }

            [UriMeta("qs__a")]
            public string A { get; set; }

            public bool Equals(TestTypedMetadata other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return string.Equals(VarOne, other.VarOne) && string.Equals(VarTwo, other.VarTwo) && string.Equals(A, other.A);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                return obj.GetType() == GetType() && Equals((TestTypedMetadata)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (VarOne != null ? VarOne.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (VarTwo != null ? VarTwo.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (A != null ? A.GetHashCode() : 0);
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
