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
        public void NonTypedMetaTest(LinkDescriptor[] linkDescriptors, string url, UrlMetadata expectedResult)
        {
            // Arrange
            var urlParser = new UriPathScanf(linkDescriptors);

            // Act
            var result = urlParser.GetMetadata(url);

            // Assert
            if (expectedResult != null)
            {
                result.UrlType.Should().BeEquivalentTo(expectedResult.UrlType);
                ((IDictionary<string, string>)result.Meta).Should().BeEquivalentTo(
                    (IDictionary<string, string>)expectedResult.Meta);
            }
            else
            {
                result.Should().BeNull();
            }
        }

        [Test, TestCaseSource(typeof(UrlParserTestSource), nameof(UrlParserTestSource.TypedMetaTestCases))]
        public void TypedMetaTest(LinkDescriptor[] linkDescriptors, string url, UrlMetadata expectedResult)
        {
            // Arrange
            var urlParser = new UriPathScanf(linkDescriptors);

            // Act
            var result = urlParser.GetMetadata(url);

            // Assert
            if (expectedResult != null)
            {
                result.UrlType.Should().BeEquivalentTo(expectedResult.UrlType);
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
