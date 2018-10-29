using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
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
            var result = metadata.AsDict;

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void Cast_ToDictionary_NonSuccess()
        {
            // Arrange
            var metadata = new UriMetadata("someType", new object());

            // Act
            var result = metadata.AsDict;

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public void Cast_ToModel_Success()
        {
            // Arrange
            var metadata = new UriMetadata("someType", new Meta());

            // Act
            var result = metadata.As<Meta>();

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public void Cast_ToModel_NonSuccess()
        {
            // Arrange
            var metadata = new UriMetadata("someType", new object());

            // Act
            var result = metadata.As<Meta>();

            // Assert
            result.Should().BeNull();
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
