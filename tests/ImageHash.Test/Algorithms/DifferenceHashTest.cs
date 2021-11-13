namespace CoenM.ImageHash.Test.Algorithms
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using CoenM.ImageHash.HashAlgorithms;
    using CoenM.ImageHash.Test.Data;
    using EasyTestFileXunit;
    using FluentAssertions;
    using Xunit;

    [UsesEasyTestFile]
    public class DifferenceHashTest
    {
        private readonly DifferenceHash _sut;

        private readonly Dictionary<string, ulong> _expectedHashes = new Dictionary<string, ulong>
        {
            { "Alyson_Hannigan_500x500_0.jpg", 10346094587896157266 },
            { "Alyson_Hannigan_500x500_1.jpg", 10346094587896157266 },
            { "Alyson_Hannigan_200x200_0.jpg", 10346094587896157266 },
            { "Alyson_Hannigan_4x4_0.jpg", 02242545344976519395 },
            { "github_1.jpg", 03609409886373023246 },
            { "github_2.jpg", 03604624846665550860 },
        };

        public DifferenceHashTest()
        {
            _sut = new DifferenceHash();
        }

        [Theory]
        [InlineData("Alyson_Hannigan_500x500_0.jpg", 10346094587359286354)]
        [InlineData("Alyson_Hannigan_500x500_1.jpg", 10346094587359286354)]
        [InlineData("Alyson_Hannigan_200x200_0.jpg", 10346094587896157266)]
        [InlineData("Alyson_Hannigan_4x4_0.jpg", 2242545344976519395)]
        [InlineData("github_1.jpg", 3609409886373023246)]
        [InlineData("github_2.jpg", 3604624846665550860)]
        public void HashImagesTest(string filename, ulong expectedHash)
        {
            // arrange
            ulong result;

            // act
            using (Stream stream = TestData.GetByName(filename).AsStream())
            {
                result = _sut.Hash(stream);
            }

            // assert
            result.Should().Be(expectedHash);
        }

        [Fact]
        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "Manually reviewed")]
        public void NotAnImageShouldThrowExceptionTest()
        {
            // arrange

            // act
            using Stream stream = TestData.NotAnImage.AsStream();
            Action act = () => _sut.Hash(stream);

            // assert
            act.Should().Throw<SixLabors.ImageSharp.UnknownImageFormatException>();
        }

        [Fact]
        public void NullArgumentShouldThrowArgumentNullExceptionTest()
        {
            // arrange

            // act
            Action act = () => _sut.Hash(null);

            // assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ImageWithFilterShouldHaveAlmostOrExactly100Similarity1Test()
        {
            // arrange
            var hash1 = _expectedHashes["Alyson_Hannigan_500x500_0.jpg"];
            var hash2 = _expectedHashes["Alyson_Hannigan_500x500_1.jpg"];

            // act
            var result = CompareHash.Similarity(hash1, hash2);

            // assert
            result.Should().Be(100);
        }

        [Fact]
        public void ResizedImageShouldHaveAlmostOrExactly100Similarity2Test()
        {
            // arrange
            var hash1 = _expectedHashes["Alyson_Hannigan_500x500_0.jpg"];
            var hash2 = _expectedHashes["Alyson_Hannigan_200x200_0.jpg"];

            // act
            var result = CompareHash.Similarity(hash1, hash2);

            // assert
            result.Should().Be(100);
        }

        [Fact]
        public void ComparingExtremelySmallImageShouldDecreaseSimilarityTest()
        {
            // arrange
            var hash1 = _expectedHashes["Alyson_Hannigan_4x4_0.jpg"];
            var hash2 = _expectedHashes["Alyson_Hannigan_500x500_0.jpg"];

            // act
            var result = CompareHash.Similarity(hash1, hash2);

            // assert
            result.Should().Be(59.375);
        }

        [Fact]
        public void TwoDifferentImagesOfGithubArePrettySimilarTests()
        {
            // arrange
            var hash1 = _expectedHashes["github_1.jpg"];
            var hash2 = _expectedHashes["github_2.jpg"];

            // act
            var result = CompareHash.Similarity(hash1, hash2);

            // assert
            result.Should().Be(89.0625);
        }
    }
}
