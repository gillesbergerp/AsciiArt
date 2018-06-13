using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AsciiArt
{
    internal static class Utils
    {
        internal static readonly int BytesPerPixel = 4;
        internal static readonly PixelFormat PixelFormat = PixelFormats.Bgra32;

        internal static byte[] GetImagePixels(this BitmapSource source)
        {
            int stride = source.PixelWidth * 4;
            int size = source.PixelHeight * stride;
            byte[] pixels = new byte[size];
            source.CopyPixels(pixels, stride, 0);
            return pixels;
        }

        internal static int ClampColorChannel(int value)
        {
            if (value > 255)
            {
                return 255;
            }
            else if (value < 0)
            {
                return 0;
            }

            return value;
        }

        internal static int ClampColorChannel(int value, int min, int max)
        {
            if (max > 255)
            {
                max = 255;
            }
            else if (min < 0)
            {
                min = 0;
            }

            if (value > max)
            {
                return max;
            }
            else if (value < min)
            {
                return min;
            }

            return value;
        }

        internal static Bitmap CharToBitmap(char c, Font font, Size size)
        {
            Bitmap bmp = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(bmp);
            SizeF charSize = g.MeasureString(c.ToString(), font);
            g.Clear(System.Drawing.Color.White);

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            g.DrawString(c.ToString(), font, System.Drawing.Brushes.Black, new PointF((size.Width - charSize.Width) / 2, (size.Height - charSize.Height) / 2));
            g.Flush();

            return bmp;
        }

        internal static Size MeasureChar(char c, Font font)
        {
            Bitmap bmp = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(bmp);
            SizeF size = g.MeasureString(c.ToString(), font);
            g.Flush();

            return new Size((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
        }

        internal static byte ColorToGray(System.Drawing.Color c)
        {
            return (byte)(.21 * c.R + .71 * c.G + .071 * c.B);
        }
    }
}
