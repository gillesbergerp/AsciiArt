using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using AsciiArt.RenderOptions;

namespace AsciiArt.Converter
{
    /// <summary>
    /// Converts images to ASCII art formatted as plain text. See <see cref="Converter{T}"/> for more information.
    /// </summary>
    public class TextConverter : Converter<string>
    {
        private StringBuilder _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextConverter"/> class.
        /// </summary>
        /// <param name="renderOptions">The AsciiArtRenderOptions used for conversion.</param>
        public TextConverter(RenderOption renderOptions) : base(renderOptions, null, false)
        {            
        }

        /// <summary>
        /// Adds the character to the output string. See <see cref="Converter{T}.AddToOutput(KeyValuePair{char, PixelMatrix}, Point, PixelMatrix)"/> for more information.
        /// </summary>
        /// <param name="character">The character to be drawn.</param>
        /// <param name="point">A point that represents the coordinates of the upper left corner to be drawn.</param>
        /// <param name="tile">The PixelMatrix representing a tile of the source image.</param>
        protected override void AddToOutput(KeyValuePair<char, PixelMatrix> character, Point point, PixelMatrix tile)
        {
            if (point.X == 0 && point.Y != 0)
            {
                _result.Append("\r\n");
            }

            _result.Append(character.Key);
        }

        /// <summary>
        /// See <see cref="Converter{T}.FinalizeOutput"/> for more information.
        /// </summary>
        /// <returns>Returns the converted image formatted as plain text.</returns>
        protected override string FinalizeOutput()
        {
            return _result.ToString();
        }

        /// <summary>
        /// See <see cref="Converter{T}.InitializeOutput(Size)"/> for more information.
        /// </summary>
        /// <param name="size">The size in pixels of the final image.</param>
        protected override void InitializeOutput(Size size)
        {
            _result = new StringBuilder();
        }
    }
}
