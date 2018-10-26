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
        [Test, TestCaseSource(typeof(UriPathScanfTestSource), nameof(UriPathScanfTestSource.NonTypedMetaTestCases))]
        public void NonTypedMetaTest(UriPathDescriptor[] linkDescriptors, string url, UriMetadata expectedResult)
        {
            // Arrange
            var urlParser = new UriPathScanf(linkDescriptors);

            // Act
            var result = urlParser.Scan(url);

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

        [Test, TestCaseSource(typeof(UriPathScanfTestSource), nameof(UriPathScanfTestSource.TypedMetaTestCases))]
        public void TypedMetaTest(UriPathDescriptor[] linkDescriptors, string url, UriMetadata expectedResult)
        {
            // Arrange
            var urlParser = new UriPathScanf(linkDescriptors);

            // Act
            var result = urlParser.Scan(url);
            
            // Assert
            if (expectedResult != null)
            {
                result.UriType.Should().BeEquivalentTo(expectedResult.UriType);
                ((UriPathScanfTestSource.TestTypedMetadata)result.Meta).Should().BeEquivalentTo(
                    (UriPathScanfTestSource.TestTypedMetadata)expectedResult.Meta);
            }
            else
            {
                result.Should().BeNull();
            }
        }
    }
}
