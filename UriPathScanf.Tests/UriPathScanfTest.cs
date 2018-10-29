using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;

namespace UriPathScanf.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal class UriPathScanfTest
    {
        [Test,
         TestCaseSource(typeof(UriPathScanfTestSource), nameof(UriPathScanfTestSource.NonTypedMetaTestCases))]
        public void NonTypedMetaTest(IEnumerable<UriPathDescriptor> linkDescriptors, string url, UriMetadata expectedResult)
        {
            // Arrange
            var urlParser = new UriPathScanf(linkDescriptors);

            // Act
            var result = urlParser.Scan(url);
            var resultTyped = urlParser.ScanDict(url);

            // Assert
            if (expectedResult != null)
            {
                result.UriType.Should().BeEquivalentTo(expectedResult.UriType);
                result.Type.Should().Be(expectedResult.Type);

                result.TryCast(out var resultCasted);
                resultCasted.Should().BeEquivalentTo(expectedResult.Meta);

                resultTyped.Meta.Should().BeEquivalentTo(expectedResult.Meta);
            }
            else
            {
                result.Should().BeNull();
                resultTyped.Should().BeNull();
            }
        }

        [Test,
         TestCaseSource(typeof(UriPathScanfTestSource), nameof(UriPathScanfTestSource.TypedMetaTestCases))]
        public void TypedMetaTest(IEnumerable<UriPathDescriptor> linkDescriptors, string url, UriMetadata expectedResult)
        {
            // Arrange
            var urlParser = new UriPathScanf(linkDescriptors);

            // Act
            var result = urlParser.Scan(url);
            var resultTyped = urlParser.Scan<UriPathScanfTestSource.TestTypedMetadata>(url);

            // Assert
            if (expectedResult != null)
            {
                result.UriType.Should().BeEquivalentTo(expectedResult.UriType);
                result.Type.Should().Be(expectedResult.Type);

                result.TryCast<UriPathScanfTestSource.TestTypedMetadata>(out var resultCasted);
                resultCasted.Should().BeEquivalentTo(expectedResult.Meta);

                resultTyped.Meta.Should().BeEquivalentTo(expectedResult.Meta);
            }
            else
            {
                result.Should().BeNull();
                resultTyped.Should().BeNull();
            }
        }

        [Test]
        public void NonScanTypeTest()
        {
            // Arrange
            const string uri = "/xxx/";

            var urlParser = new UriPathScanf(new []
            {
                new UriPathDescriptor(uri, "xxx", typeof(UriPathScanfTestSource.TestTypedMetadata)), 
            });

            // Act
            var resultTyped = urlParser.Scan<UriPathScanfTestSource.TestTypedMetadataFake>(uri);
            var resultDictTyped = urlParser.ScanDict(uri);

            // Assert
            resultTyped.Should().BeNull();
            resultDictTyped.Should().BeNull();
        }
    }
}
