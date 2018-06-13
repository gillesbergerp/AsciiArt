using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;

namespace AsciiArt
{
    /// <summary>
    /// Represents (part of) an image's pixel data
    /// </summary>
    public class PixelMatrix
    {
        private Size _size;
        private byte[] _pixels;

        /// <summary>
        /// Initializes a new instance of the <see cref="PixelMatrix"/> class.
        /// </summary>
        /// <param name="pixels">The BGRA pixel data.</param>
        /// <param name="size">The size in pixel that this object has.</param>
        public PixelMatrix(byte[] pixels, Size size)
        {
            _size = size;

            if (!ArgumentsValid(pixels.Length / Utils.BytesPerPixel))
            {
                throw new ArgumentException("The size specified is not as expected.");
            }

            Histogram = new Histogram();
            _pixels = pixels;

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PixelMatrix"/> class.
        /// </summary>
        /// <param name="source">The source image's pixels</param>
        /// <param name="size">The size in pixel that this object has.</param>
        /// <param name="start">This object's upper left corner absolute to the image.</param>
        /// <param name="stride">The number of subpixels per row.</param>
        public PixelMatrix(byte[] source, Size size, Point start, int stride)
        {
            _size = size;

            if (!ArgumentsValid(source.Length / Utils.BytesPerPixel))
            {
                throw new ArgumentException("The size specified is not as expected.");
            }

            Histogram = new Histogram();

            Initialize(source, size, start, stride);
        }

        /// <summary>
        /// Gets a color that describes this matrix' average color
        /// </summary>
        public Color AverageColor { get; private set; }

        /// <summary>
        /// Gets a value that describes the this matrix' brightness
        /// </summary>
        public double Brightness { get; private set; }

        internal Histogram Histogram { get; private set; }

        internal double Mean { get; private set; }

        internal double Variance { get; private set; }

        private bool ArgumentsValid(int pixelCount)
        {
            return _size.Height * Utils.BytesPerPixel * _size.Width != pixelCount;
        }

        private void Initialize()
        {
            Dictionary<Color, int> colors = new Dictionary<Color, int>();

            for (int y = 0; y < _size.Height; y++)
            {
                for (int x = 0; x < _size.Width; x++)
                {
                    Color c = GetPixel(x, y);
                    Brightness += c.GetBrightness();

                    Mean += (c.R + c.G + c.B) / 3;

                    CountColor(colors, c);
                }
            }

            FinalizeInitialization(colors);
        }

        private void Initialize(byte[] source, Size size, Point start, int sourceStride)
        {
            Dictionary<Color, int> colors = new Dictionary<Color, int>();
            _pixels = new byte[size.Width * size.Height * Utils.BytesPerPixel];
            int stride = size.Width * Utils.BytesPerPixel;

            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < size.Width * Utils.BytesPerPixel; x += Utils.BytesPerPixel)
                {
                    int index = y * stride + x;
                    int sourceIndex = (y + start.Y) * sourceStride + start.X * Utils.BytesPerPixel + x;
                    _pixels[index] = source[sourceIndex];
                    _pixels[index + 1] = source[sourceIndex + 1];
                    _pixels[index + 2] = source[sourceIndex + 2];
                    _pixels[index + 3] = source[sourceIndex + 3];

                    Color c = Color.FromArgb(_pixels[index + 3], _pixels[index + 2], _pixels[index + 1], _pixels[index]);
                    Brightness += c.GetBrightness();

                    Mean += (c.R + c.G + c.B) / 3;

                    CountColor(colors, c);
                }
            }

            FinalizeInitialization(colors);
        }

        private void CountColor(Dictionary<Color, int> colors, Color c)
        {
            colors.TryGetValue(c, out int val);
            if (val > 0)
            {
                colors.Remove(c);
            }

            val++;
            colors.Add(c, val);

            Histogram.AddValue(c);
        }

        private void FinalizeInitialization(Dictionary<Color, int> colors)
        {
            Mean /= _size.Width * _size.Height;

            Brightness /= _size.Width * _size.Height;

            CalculateVariance();
            CalculateAverageColor(colors);
        }

        private void CalculateVariance()
        {
            for (int i = 0; i < _size.Width * _size.Height; i++)
            {
                Variance += Math.Pow(_pixels[i] - Mean, 2);
            }

            Variance /= _size.Width * _size.Height;
        }

        private void CalculateAverageColor(Dictionary<Color, int> colors)
        {
            int r = 0;
            int g = 0;
            int b = 0;
            var maxCols = colors.Where(h => h.Value >= _size.Width / Utils.BytesPerPixel * _size.Height * 0.7).ToList();

            if (maxCols.Count > 0)
            {
                foreach (var item in maxCols)
                {
                    r += item.Key.R;
                    g += item.Key.G;
                    b += item.Key.B;
                }

                r /= maxCols.Count;
                g /= maxCols.Count;
                b /= maxCols.Count;

                AverageColor = Color.FromArgb(255, r, g, b);
            }
            else
            {
                var max = colors.FirstOrDefault(h => h.Value == colors.Values.Max()).Key;
                AverageColor = Color.FromArgb(255, max.R, max.G, max.B);
            }
        }

        /// <summary>
        /// Compares the brightness of two PixelMatrix objects.
        /// </summary>
        /// <param name="other">The PixelMatrix to compare with.</param>
        /// <returns>The difference in brightness as absolute value.</returns>
        public double CompareBrightness(PixelMatrix other)
        {
            return Math.Abs(Brightness - other.Brightness);
        }

        /// <summary>
        /// Gets the color that represents the specified Pixel of this object.
        /// </summary>
        /// <param name="x">The pixel's x coordinate.</param>
        /// <param name="y">The pixel's y coordinate.</param>
        /// <returns>The color that represents the specified Pixel of this object.</returns>
        public Color GetPixel(int x, int y)
        {
            int index = (y * _size.Width * Utils.BytesPerPixel) + x;

            return Color.FromArgb(_pixels[index + 3], _pixels[index + 2], _pixels[index + 1], _pixels[index]);
        }
        
        /// <summary>
        /// Creates a list of PixelMatrix objects in the given size from a source image.
        /// </summary>
        /// <param name="source">The source image.</param>
        /// <param name="tileSize">The size of each resulting PixelMatrix.</param>
        /// <returns>a list of PixelMatrix objects representing the source image.</returns>
        public static List<PixelMatrix> CreateTiles(BitmapSource source, Size tileSize)
        {
            List<PixelMatrix> tiles = new List<PixelMatrix>();

            var pixels = source.GetImagePixels();

            int rows = source.PixelHeight / tileSize.Height;
            int cols = source.PixelWidth / tileSize.Width;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    tiles.Add(new PixelMatrix(pixels, tileSize, new Point(x * tileSize.Width, y * tileSize.Height), source.PixelWidth * Utils.BytesPerPixel));
                }
            }

            return tiles;
        }
    }
}
