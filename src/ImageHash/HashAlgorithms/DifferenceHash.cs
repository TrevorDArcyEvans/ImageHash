﻿namespace CoenM.ImageSharp.HashAlgorithms
{
    using System;

    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using SixLabors.ImageSharp.Processing.Filters;
    using SixLabors.ImageSharp.Processing.Transforms;

    /// <summary>
    /// Difference hash; Calculate a hash of an image based on visual characteristics by transforming the image to an 9x8 grayscale bitmap.
    /// Hash is based on each pixel compared to it's right neighbour pixel.
    /// </summary>
    /// <remarks>
    /// Algorith specified by David Oftedal and slightly adjusted by Dr. Neal Krawetz.
    /// See http://www.hackerfactor.com/blog/index.php?/archives/529-Kind-of-Like-That.html for more information.
    /// </remarks>
    // ReSharper disable once StyleCop.SA1650
    public class DifferenceHash : IImageHash
    {
        private const int WIDTH = 9;
        private const int HEIGHT = 8;

        /// <inheritdoc />
        public ulong Hash(Image<Rgba32> image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            // We first auto orient because with and heigth differ.
            image.Mutate(ctx => ctx
                                .AutoOrient()
                                .Resize(WIDTH, HEIGHT)
                                .Grayscale(GrayscaleMode.Bt601));

            var mask = 1UL << HEIGHT * (WIDTH - 1) - 1;
            var hash = 0UL;

            for (var y = 0; y < HEIGHT; y++)
            {
                var leftPixel = image[0, y];
                for (var x = 1; x < WIDTH; x++)
                {
                    var rightPixel = image[x, y];

                    if (leftPixel.R < rightPixel.R)
                        hash |= mask;

                    leftPixel = rightPixel;
                    mask = mask >> 1;
                }
            }

            return hash;
        }
    }
}