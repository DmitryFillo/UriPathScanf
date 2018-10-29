using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;

namespace UriPathScanf.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    internal class UriMetadataTest
    {
        [Test]
        public void Cast_ToDictionary_Success()
        {
            // Arrange
            var metadata = new UriMetadata("someType", new Dictionary<string, string>());

            // Act
            var resultCast = metadata.TryCast(out var result);

            // Assert
            result.Should().NotBeNull();
            resultCast.Should().BeTrue();
        }

        [Test]
        public void Cast_ToDictionary_NonSuccess()
        {
            // Arrange
            var metadata = new UriMetadata("someType", new object());

            // Act
            var resultCast = metadata.TryCast(out var result);

            // Assert
            result.Should().BeNull();
            resultCast.Should().BeFalse();
        }

        [Test]
        public void Cast_ToModel_Success()
        {
            // Arrange
            var metadata = new UriMetadata("someType", new Meta());

            // Act
            var resultCast = metadata.TryCast<Meta>(out var result);

            // Assert
            result.Should().NotBeNull();
            resultCast.Should().BeTrue();
        }

        [Test]
        public void Cast_ToModel_NonSuccess()
        {
            // Arrange
            var metadata = new UriMetadata("someType", new object());

            // Act
            var resultCast = metadata.TryCast<Meta>(out var result);

            // Assert
            result.Should().BeNull();
            resultCast.Should().BeFalse();
        }

        [Test]
        public void Equals_Success()
        {
            // Arrange
            var obj = new object();
            var metadata = new UriMetadata("someType", obj) { Type = typeof(string) };
            var metadata2 = new UriMetadata("someType", obj) { Type = typeof(int) };

            // Act

            // Assert
            metadata.Equals(metadata2).Should().BeTrue();
        }

        [Test]
        public void Equals_Fail()
        {
            // Arrange
            var metadata = new UriMetadata("someType", new object());
            var metadata2 = new UriMetadata("someType", new object());

            // Act

            // Assert
            metadata.Equals(metadata2).Should().BeFalse();
        }

        private class Meta : IUriPathMetaModel
        {

        }
    }
}
