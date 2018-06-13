using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AsciiArt.RenderOptions
{
    /// <summary>
    /// Calculates an ASCII art image's characters using Mean squared error. See <see cref="https://en.wikipedia.org/wiki/Mean_squared_error"/> for more information.
    /// </summary>
    public class MseBasedRenderOption : RenderOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MseBasedRenderOption"/> class.
        /// </summary>
        /// <param name="font">The AsciiImageFont used to draw the characters.</param>
        public MseBasedRenderOption(AsciiFont font) : base(font)
        {
        }

        /// <summary>
        /// Calculates the best-fitting character based on Mean squared error. See <see cref="RenderOption.CalculateCharForMatrix(PixelMatrix)"/> and <see cref="https://en.wikipedia.org/wiki/Mean_squared_error"/> for more information.
        /// </summary>
        /// <param name="tile">The part of the image to be represented.</param>
        /// <returns>The best-fitting character and its corresponding PixelMatrix.</returns>
        public override KeyValuePair<char, PixelMatrix> CalculateCharForMatrix(PixelMatrix tile)
        {
            double minScore = double.MaxValue;
            KeyValuePair<char, PixelMatrix> bestFit = Font.Chars.FirstOrDefault();

            foreach (var item in Font.Chars)
            {
                double score = 0;

                for (int y = 0; y < Font.CharSize.Height; y++)
                {
                    for (int x = 0; x < Font.CharSize.Width; x += Utils.BytesPerPixel)
                    {
                        byte grayChar = Utils.ColorToGray(item.Value.GetPixel(x, y));
                        byte grayMatrix = Utils.ColorToGray(tile.GetPixel(x, y));

                        score += (grayMatrix - grayChar) * (grayMatrix - grayChar);
                    }
                }

                if (score < minScore)
                {
                    bestFit = item;
                    minScore = score;
                }
            }

            return bestFit;
        }
    }
}
