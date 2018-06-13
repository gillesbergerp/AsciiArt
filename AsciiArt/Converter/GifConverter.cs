using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AsciiArt.RenderOptions;

namespace AsciiArt.Converter
{
    /// <summary>
    /// Converts a GIF image to (animated) ASCII art. See <see cref="Converter{T}"/> for more information.
    /// </summary>
    public class GifConverter
    {
        private BitmapSourceConverter _converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="GifConverter"/> class.
        /// </summary>
        /// <param name="renderOptions">The AsciiArtRenderOptions used for conversion.</param>
        /// <param name="drawingMode">The DrawingMode used for creating the output.</param>
        public GifConverter(RenderOption renderOptions, DrawingMode drawingMode)
        {
            _converter = new BitmapSourceConverter(renderOptions, drawingMode);
        }

        /// <summary>
        /// Converts a GIF image to ASCII art and later on saves it to the specified file.
        /// </summary>
        /// <param name="srcFileUri">The original image's file path.</param>
        /// <param name="destFileUri">The converted image's file path.</param>
        /// <param name="progress">Provider for progress updates.</param>
        /// <returns>A System.Threading.Tasks.Task that represents the conversion operation.</returns>
        public Task ConvertAndSaveAsync(Uri srcFileUri, Uri destFileUri, IProgress<double> progress)
        {
            return Task.Factory.StartNew(() =>
            {
                GifBitmapDecoder dec = new GifBitmapDecoder(srcFileUri, BitmapCreateOptions.None, BitmapCacheOption.Default);
                GifBitmapEncoder enc = new GifBitmapEncoder();

                int tempCount = 0;
                int totalCount = dec.Frames.Count;

                foreach (var item in dec.Frames)
                {
                    var converted = _converter.Convert(item, new Progress<double>(value =>
                    {
                        if (progress != null)
                        {
                            progress.Report((tempCount + value) / totalCount);
                        }
                    }));

                    var frame = BitmapFrame.Create(converted, null, (BitmapMetadata)item.Metadata, new ReadOnlyCollection<ColorContext>(new List<ColorContext>()));

                    enc.Frames.Add(frame);
                    tempCount++;
                }

                using (var ms = new MemoryStream())
                {
                    enc.Save(ms);
                    byte[] imageBytes = ms.ToArray();
                    List<byte> fileBytes = new List<byte>();
                    fileBytes.AddRange(imageBytes.Take(13));

                    // Looping a GIF image is not supported directly by GifBitmapEncoder, therefore the NETSCAPE2.0 is added to the file.
                    // See also: http://giflib.sourceforge.net/whatsinagif/bits_and_bytes.html
                    byte[] loopingExtension = new byte[] { 33, 255, 11, 78, 69, 84, 83, 67, 65, 80, 69, 50, 46, 48, 3, 1, 0, 0, 0 };
                    fileBytes.AddRange(loopingExtension);
                    fileBytes.AddRange(imageBytes.Skip(13));
                    File.WriteAllBytes(destFileUri.AbsolutePath, fileBytes.ToArray());
                }
            });
        }
    }
}
