using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AsciiArt.RenderOptions;

namespace AsciiArt.Converter
{
    /// <summary>
    /// Converts images to ASCII art images. See <see cref="Converter{T}"/> for more information.
    /// </summary>
    public class BitmapSourceConverter : Converter<BitmapSource>
    {
        private byte[] _pixels;
        private Size _size;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapSourceConverter"/> class.
        /// </summary>
        /// <param name="renderOptions">The AsciiArtRenderOptions used for conversion.</param>
        /// <param name="drawingMode">The DrawingMode used for creating the output.</param>
        public BitmapSourceConverter(RenderOption renderOptions, DrawingMode drawingMode) : base(renderOptions, drawingMode, true)
        {            
        }

        /// <summary>
        /// Copies the pixel values to the output image. See <see cref="Converter{T}.AddToOutput(KeyValuePair{char, PixelMatrix}, Point, PixelMatrix)"/> for more information.
        /// </summary>
        /// <param name="character">The character to be drawn.</param>
        /// <param name="point">A point that represents the coordinates of the upper left corner to be drawn.</param>
        /// <param name="tile">The PixelMatrix representing a tile of the source image.</param>
        protected override void AddToOutput(KeyValuePair<char, PixelMatrix> character, Point point, PixelMatrix tile)
        {
            for (int y = point.Y; y < point.Y + RenderOptions.TileSize.Height; y++)
            {
                for (int x = point.X * Utils.BytesPerPixel; x < (point.X + RenderOptions.TileSize.Width) * Utils.BytesPerPixel; x += Utils.BytesPerPixel)
                {
                    int index = y * _size.Width * Utils.BytesPerPixel;
                    System.Drawing.Color c = DrawingMode.CalculateColor(character.Value.GetPixel(x - Utils.BytesPerPixel * point.X, y - point.Y), tile.GetPixel(x - Utils.BytesPerPixel * point.X, y - point.Y));
                    _pixels[index + x] = c.B; // b
                    _pixels[index + x + 1] = c.G; // g
                    _pixels[index + x + 2] = c.R; // r
                    _pixels[index + x + 3] = c.A; // a
                }
            }
        }
        
        /// <summary>
        /// Creates a System.Windows.Media.Imaging.BitmapSource that represents the converted image
        /// </summary>
        /// <returns>The converted image as unmodifiable System.Windows.Media.Imaging.BitmapSource</returns>
        protected override BitmapSource FinalizeOutput()
        {
            int stride = Utils.BytesPerPixel * _size.Width;
            BitmapSource img = BitmapSource.Create(_size.Width, _size.Height, 96, 96, Utils.PixelFormat, null, _pixels, stride);
            img.Freeze();
            return img;
        }

        /// <summary>
        /// Creates an empty byte array that will eventually hold the pixel data. See <see cref="Converter{T}.InitializeOutput(Size)"/> for more information.
        /// </summary>
        /// <param name="size">The size in pixels of the final image.</param>
        protected override void InitializeOutput(System.Drawing.Size size)
        {
            _size = size;
            _pixels = new byte[_size.Width * Utils.BytesPerPixel * _size.Height];
        }
    }
}
