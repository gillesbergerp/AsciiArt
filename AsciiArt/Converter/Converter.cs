using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using AsciiArt.RenderOptions;

namespace AsciiArt.Converter
{
    /// <summary>
    /// Converts images to ASCII art images in a specific output format and based on some render options.
    /// </summary>
    /// <typeparam name="T">The type of the output format.</typeparam>
    public abstract class Converter<T>
    {
        private readonly bool _parallelConversion;

        /// <summary>
        /// Initializes a new instance of the <see cref="Converter{T}"/> class.
        /// </summary>
        /// <param name="renderOptions">The AsciiArtRenderOptions used for conversion.</param>
        /// <param name="drawingMode">The DrawingMode used for creating the output.</param>
        /// <param name="parallelConversion">Specifies whether conversion is done in parallel.</param>
        public Converter(RenderOption renderOptions, DrawingMode drawingMode, bool parallelConversion)
        {
            DrawingMode = drawingMode;
            RenderOptions = renderOptions;
            _parallelConversion = parallelConversion;
        }

        /// <summary>
        /// Gets the drawing mode associated with this object
        /// </summary>
        protected DrawingMode DrawingMode { get; private set; }

        /// <summary>
        /// Gets the AsciiArtRenderOption used to calculate the best-fitting character
        /// </summary>
        protected RenderOption RenderOptions { get; private set; }

        /// <summary>
        /// Converts an image to ASCII art.
        /// </summary>
        /// <param name="fileUri">The original image's file path.</param>
        /// <param name="progress">Provider for progress updates.</param>
        /// <returns>A System.Threading.Tasks.Task that represents the conversion operation. The Task's result holds the converted image.</returns>
        public Task<T> ConvertAsync(Uri fileUri, IProgress<double> progress)
        {
            return ConvertAsync(new BitmapImage(fileUri), progress);
        }

        /// <summary>
        /// Converts an image to ASCII art
        /// </summary>
        /// <param name="source">The original image as System.Windows.Media.Imaging.BitmapSource.</param>
        /// <param name="progress">Provider for progress updates.</param>
        /// <returns>A System.Threading.Tasks.Task that represents the conversion operation. The Task's result holds the converted image.</returns>
        public Task<T> ConvertAsync(BitmapSource source, IProgress<double> progress)
        {
            source.Freeze();
            return Task.Factory.StartNew(() => Convert(source, progress));
        }

        internal T Convert(BitmapSource source, IProgress<double> progress)
        {
            if (source.Format != Utils.PixelFormat)
            {
                source = new FormatConvertedBitmap(source, Utils.PixelFormat, null, 0);
            }

            List<PixelMatrix> tiles = PixelMatrix.CreateTiles(source, new Size(RenderOptions.TileSize.Width, RenderOptions.TileSize.Height));
            int width = source.PixelWidth - (source.PixelWidth % RenderOptions.TileSize.Width);
            int height = source.PixelHeight - (source.PixelHeight % RenderOptions.TileSize.Height);

            InitializeOutput(new Size(width, height));

            int tempCount = 0;

            if (_parallelConversion)
            {
                Parallel.For(0, tiles.Count, i =>
                {
                    AddToOutput(RenderOptions.CalculateCharForMatrix(tiles[i]), new Point(i % (width / RenderOptions.TileSize.Width) * RenderOptions.TileSize.Width, i / (width / RenderOptions.TileSize.Width) * RenderOptions.TileSize.Height), tiles[i]);

                    if (progress != null)
                    {
                        progress.Report((double)tempCount / tiles.Count);
                        tempCount++;
                    }
                });
            }
            else
            {
                for (int i = 0; i < tiles.Count; i++)
                {
                    AddToOutput(RenderOptions.CalculateCharForMatrix(tiles[i]), new Point(i % (width / RenderOptions.TileSize.Width) * RenderOptions.TileSize.Width, i / (width / RenderOptions.TileSize.Width) * RenderOptions.TileSize.Height), tiles[i]);

                    if (progress != null)
                    {
                        progress.Report((double)tempCount / tiles.Count);
                        tempCount++;
                    }
                }
            }

            return FinalizeOutput();
        }

        /// <summary>
        /// Initializes the converter's output.
        /// </summary>
        /// <param name="size">The size in pixels of the final image.</param>
        protected abstract void InitializeOutput(Size size);

        /// <summary>
        /// The action that need to be taken at the end of the conversion.
        /// </summary>
        /// <returns>Returns the converted image.</returns>
        protected abstract T FinalizeOutput();

        /// <summary>
        /// Adds the character at a specified position to the output.
        /// </summary>
        /// <param name="character">The character to be drawn.</param>
        /// <param name="point">A point that represents the coordinates of the upper left corner to be drawn.</param>
        /// <param name="tile">The PixelMatrix representing a tile of the source image.</param>
        protected abstract void AddToOutput(KeyValuePair<char, PixelMatrix> character, Point point, PixelMatrix tile);
    }
}
