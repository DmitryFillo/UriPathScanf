using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;

namespace UriPathScanf.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class UrlParserTest
    {
        [Test, TestCaseSource(typeof(UrlParserTestSource), nameof(UrlParserTestSource.NonTypedMetaTestCases))]
        public void NonTypedMetaTest(UriPathDescriptor[] linkDescriptors, string url, UriMetadata expectedResult)
        {
            // Arrange
            var urlParser = new UriPathScanf(linkDescriptors);

            // Act
            var result = urlParser.GetMeta(url);

            // Assert
            if (expectedResult != null)
            {
                result.UriType.Should().BeEquivalentTo(expectedResult.UriType);
                ((IDictionary<string, string>)result.Meta).Should().BeEquivalentTo(
                    (IDictionary<string, string>)expectedResult.Meta);
            }
            else
            {
                result.Should().BeNull();
            }
        }

        [Test, TestCaseSource(typeof(UrlParserTestSource), nameof(UrlParserTestSource.TypedMetaTestCases))]
        public void TypedMetaTest(UriPathDescriptor[] linkDescriptors, string url, UriMetadata expectedResult)
        {
            // Arrange
            var urlParser = new UriPathScanf(linkDescriptors);

            // Act
            var result = urlParser.GetMeta(url);

            // Assert
            if (expectedResult != null)
            {
                result.UriType.Should().BeEquivalentTo(expectedResult.UriType);
                ((UrlParserTestSource.TestTypedMetadata)result.Meta).Should().BeEquivalentTo(
                    (UrlParserTestSource.TestTypedMetadata)expectedResult.Meta);
            }
            else
            {
                result.Should().BeNull();
            }
        }
    }
}
