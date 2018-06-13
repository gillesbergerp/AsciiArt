using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AsciiArt.RenderOptions
{
    /// <summary>
    /// Calculates an ASCII art image's characters based on brightness.
    /// </summary>
    public class BrightnessBasedRenderOption : RenderOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrightnessBasedRenderOption"/> class.
        /// </summary>
        /// <param name="font">The AsciiImageFont used to draw the characters.</param>
        public BrightnessBasedRenderOption(AsciiFont font) : this(font, RenderingMode.Default)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BrightnessBasedRenderOption"/> class.
        /// </summary>
        /// <param name="font">The AsciiImageFont used to draw the characters.</param>
        /// <param name="renderingMode">The RenderingMode used to determine the best-fitting character.</param>
        public BrightnessBasedRenderOption(AsciiFont font, RenderingMode renderingMode) : base(font, renderingMode)
        {
        }

        /// <summary>
        /// Calculates the best-fitting character based on the brightness. See <see cref="RenderOption.CalculateCharForMatrix(PixelMatrix)"/> for more information.
        /// </summary>
        /// <param name="tile">The part of the image to be represented.</param>
        /// <returns>The best-fitting character and its corresponding PixelMatrix.</returns>
        public override KeyValuePair<char, PixelMatrix> CalculateCharForMatrix(PixelMatrix tile)
        {
            KeyValuePair<char, PixelMatrix> bestFit = Font.Chars.FirstOrDefault();

            foreach (var item in Font.Chars)
            {
                if (RenderingMode == RenderingMode.Default && item.Value.CompareBrightness(tile) < bestFit.Value.CompareBrightness(tile))
                {
                    bestFit = item;
                }
                else if (RenderingMode == RenderingMode.Inverted && item.Value.CompareBrightness(tile) > bestFit.Value.CompareBrightness(tile))
                {
                    bestFit = item;
                }
            }

            return bestFit;
        }
    }
}
