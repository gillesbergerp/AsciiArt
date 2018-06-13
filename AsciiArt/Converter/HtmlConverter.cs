using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using AsciiArt.RenderOptions;

namespace AsciiArt.Converter
{
    /// <summary>
    /// Converts an image to ASCII art, formatted as HTML. See <see cref="Converter{T}"/> for more information.
    /// </summary>
    public class HtmlConverter : Converter<string>
    {
        private int _imageWidth;
        private StringBuilder _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlConverter"/> class.
        /// </summary>
        /// <param name="renderOptions">The AsciiArtRenderOptions used for conversion.</param>
        /// <param name="drawingMode">The DrawingMode used for creating the output.</param>
        public HtmlConverter(RenderOption renderOptions, DrawingMode drawingMode) : base(renderOptions, drawingMode, false)
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
                _result.Append("</p>");
            }

            Color c = DrawingMode.CalculateColor(Color.FromArgb(0), tile.AverageColor);
            _result.Append(string.Format("<font color=\"{0:X2}{1:X2}{2:X2}\">{3}</font>", c.R, c.G, c.B, character.Key));
        }

        /// <summary>
        /// Closes all open HTML tags
        /// </summary>
        /// <returns>The HTML that represents the converted image as string</returns>
        protected override string FinalizeOutput()
        {
            _result.Append("</p></body></html>");
            return _result.ToString();
        }

        /// <summary>
        /// Creates an HTML header containing that specifies font style, font size, etc. See <see cref="Converter{T}.InitializeOutput(Size)"/> for more information.
        /// </summary>
        /// <param name="size">The size in pixels of the final image.</param>
        protected override void InitializeOutput(Size size)
        {
            string style = RenderOptions.Font.Font.Style == FontStyle.Italic ? "italic" : "normal";
            string weight = RenderOptions.Font.Font.Style == FontStyle.Bold ? "bold" : "normal";

            _result = new StringBuilder(string.Format("<!DOCTYPE html><html><head><style>p {{font-family: \"{0}\", monospace;font-size: {1}px;font-style: {2};font-weight: {3};}}</style></head><body><p>", RenderOptions.Font.Font.Name, RenderOptions.Font.Font.Size, style, weight));
            _imageWidth = size.Width;
        }
    }
}
