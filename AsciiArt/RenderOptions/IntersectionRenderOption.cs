﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsciiArt.RenderOptions
{
    /// <summary>
    /// Calculates an ASCII art image's characters using intersection distance. See <see cref="Histogram.Intersection(Histogram)"/> for more information.
    /// </summary>
    public class IntersectionRenderOption : RenderOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectionRenderOption"/> class.
        /// </summary>
        /// <param name="font">The AsciiImageFont used to draw the characters.</param>
        public IntersectionRenderOption(AsciiFont font) : this(font, RenderingMode.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectionRenderOption"/> class.
        /// </summary>
        /// <param name="font">The AsciiImageFont used to draw the characters.</param>
        /// <param name="renderingMode">The RenderingMode used to determine the best-fitting character.</param>
        public IntersectionRenderOption(AsciiFont font, RenderingMode renderingMode) : base(font, renderingMode)
        {
        }

        /// <summary>
        /// Calculates the best-fitting character based on intersection of the character's and tile's histograms. See <see cref="RenderOption.CalculateCharForMatrix(PixelMatrix)"/> and <see cref="Histogram.Intersection(Histogram)"/> for more information.
        /// </summary>
        /// <param name="tile">The part of the image to be represented.</param>
        /// <returns>The best-fitting character and its corresponding PixelMatrix.</returns>
        public override KeyValuePair<char, PixelMatrix> CalculateCharForMatrix(PixelMatrix tile)
        {
            double maxScore = RenderingMode == RenderingMode.Default ? double.MinValue : double.MaxValue;
            KeyValuePair<char, PixelMatrix> bestFit = Font.Chars.FirstOrDefault();

            foreach (var item in Font.Chars)
            {
                double score = tile.Histogram.Intersection(item.Value.Histogram);

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
