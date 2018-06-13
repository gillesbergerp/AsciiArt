using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AsciiArt.RenderOptions
{
    /// <summary>
    /// Calculates an ASCII art image's characters using Structural similarity index. See <see cref="https://en.wikipedia.org/wiki/Structural_similarity"/> for more information.
    /// </summary>
    public class SsimBasedRenderOption : RenderOption
    {
        // Given constants
        private static readonly double K1 = 0.01;
        private static readonly double K2 = 0.03;
        private static readonly double L = Math.Pow(2, Utils.BytesPerPixel) - 1;
        private static readonly double C1 = Math.Pow(K1 * L, 2);
        private static readonly double C2 = Math.Pow(K2 * L, 2);

        /// <summary>
        /// Initializes a new instance of the <see cref="SsimBasedRenderOption"/> class.
        /// </summary>
        /// <param name="font">The AsciiImageFont used to draw the characters.</param>
        public SsimBasedRenderOption(AsciiFont font) : this(font, RenderingMode.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SsimBasedRenderOption"/> class.
        /// </summary>
        /// <param name="font">The AsciiImageFont used to draw the characters.</param>
        /// <param name="renderingMode">The RenderingMode used to determine the best-fitting character.</param>
        public SsimBasedRenderOption(AsciiFont font, RenderingMode renderingMode) : base(font, renderingMode)
        {
        }

        /// <summary>
        /// Calculates the best-fitting character based on Structural similarity index. See <see cref="RenderOption.CalculateCharForMatrix(PixelMatrix)"/> and <see cref="https://en.wikipedia.org/wiki/Structural_similarity"/> for more information.
        /// </summary>
        /// <param name="tile">The part of the image to be represented.</param>
        /// <returns>The best-fitting character and its corresponding PixelMatrix.</returns>
        public override KeyValuePair<char, PixelMatrix> CalculateCharForMatrix(PixelMatrix tile)
        {
            double maxScore = RenderingMode == RenderingMode.Default ? double.MinValue : double.MaxValue;
            KeyValuePair<char, PixelMatrix> bestFit = Font.Chars.FirstOrDefault();

            foreach (var item in Font.Chars)
            {
                double score = 0;

                double covariance = 0;
                for (int y = 0; y < Font.CharSize.Height; y++)
                {
                    for (int x = 0; x < Font.CharSize.Width; x += Utils.BytesPerPixel)
                    {
                        byte grayChar = Utils.ColorToGray(item.Value.GetPixel(x, y));
                        byte grayMatrix = Utils.ColorToGray(tile.GetPixel(x, y));

                        covariance += (grayChar - item.Value.Mean) * (grayMatrix - tile.Mean);
                    }
                }

                covariance /= Font.CharSize.Height * Font.CharSize.Width;

                score = (2 * Math.Sqrt(item.Value.Variance) * Math.Sqrt(tile.Variance) + C1) * (2 * covariance + C2);
                score /= (item.Value.Mean * item.Value.Mean + tile.Mean * tile.Mean + C1) * (covariance * covariance + C2);

                if (RenderingMode == RenderingMode.Default && score > maxScore)
                {
                    bestFit = item;
                    maxScore = score;
                }
                else if (RenderingMode == RenderingMode.Inverted && score < maxScore)
                {
                    bestFit = item;
                    maxScore = score;
                }
            }

            return bestFit;
        }
    }
}
