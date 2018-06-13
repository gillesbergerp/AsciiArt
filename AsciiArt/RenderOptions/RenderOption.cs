using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AsciiArt.RenderOptions
{
    public enum RenderingMode
    {
        /// <summary>
        /// The default rendering mode used to select the best-fitting character.
        /// </summary>
        Default,

        /// <summary>
        /// The inverted rendering mode used to select the best-fitting character.
        /// </summary>
        Inverted,
    }

    /// <summary>
    /// The options used for calculating an ASCII art image's characters.
    /// </summary>
    public abstract class RenderOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderOption"/> class.
        /// </summary>
        /// <param name="font">The AsciiImageFont used to draw the characters.</param>
        public RenderOption(AsciiFont font) : this(font, RenderingMode.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderOption"/> class.
        /// </summary>
        /// <param name="font">The AsciiImageFont used to draw the characters.</param>
        /// <param name="renderingMode">The RenderingMode used to determine the best-fitting character.</param>
        public RenderOption(AsciiFont font, RenderingMode renderingMode)
        {
            Font = font;
            TileSize = font.CharSize;
            RenderingMode = renderingMode;
        }

        /// <summary>
        /// Gets the AsciiImageFont of this object.
        /// </summary>
        public AsciiFont Font { get; private set; }

        internal Size TileSize { get; private set; }

        /// <summary>
        /// Gets the RenderingMode used to decide which character fits best.
        /// </summary>
        protected RenderingMode RenderingMode { get; private set; }

        /// <summary>
        /// Calculates the best-fitting character for a part of the image.
        /// </summary>
        /// <param name="tile">The part of the image to be represented.</param>
        /// <returns>The best-fitting character and its corresponding PixelMatrix.</returns>
        public abstract KeyValuePair<char, PixelMatrix> CalculateCharForMatrix(PixelMatrix tile);
    }
}
