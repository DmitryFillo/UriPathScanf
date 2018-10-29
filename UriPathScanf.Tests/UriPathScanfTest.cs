using System;
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

            // Assert
            if (expectedResult != null)
            {
                result.UriType.Should().BeEquivalentTo(expectedResult.UriType);
                result.Type.Should().Be(expectedResult.Type);
                result.AsDictionary.Should().BeEquivalentTo(expectedResult.AsDictionary);
            }
            else
            {
                result.Should().BeNull();
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
            
            // Assert
            if (expectedResult != null)
            {
                result.UriType.Should().BeEquivalentTo(expectedResult.UriType);
                result.Type.Should().Be(expectedResult.Type);
                result.As<UriPathScanfTestSource.TestTypedMetadata>().Should()
                    .BeEquivalentTo(expectedResult.As<UriPathScanfTestSource.TestTypedMetadata>());
            }
            else
            {
                result.Should().BeNull();
            }
        }
    }
}
