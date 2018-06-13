using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace AsciiArt
{
    /// <summary>
    /// Represents a font used for drawing ASCII art images consisting of a set of characters and a System.Drawing.Font.
    /// </summary>
    public class AsciiFont
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsciiFont"/> class.
        /// </summary>
        /// <param name="font">The System.Drawing.Font used for rendering characters.</param>
        /// <param name="chars">The alphabet of this object.</param>
        public AsciiFont(Font font, char[] chars)
        {
            Chars = new Dictionary<char, PixelMatrix>();
            Font = font;

            GetCharacterRect(chars);
            AddChars(chars);
        }

        /// <summary>
        /// Gets this object's characters with their corresponding PixelMatrix
        /// </summary>
        public Dictionary<char, PixelMatrix> Chars { get; private set; }

        /// <summary>
        /// Gets the font used to draw the characters
        /// </summary>
        public Font Font { get; private set; }

        /// <summary>
        /// Gets each character's size when drawn
        /// </summary>
        public Size CharSize { get; private set; }

        private void GetCharacterRect(char[] chars)
        {
            foreach (var c in chars)
            {
                CompareAndUpdateSize(c);
            }
        }

        private void CompareAndUpdateSize(char c)
        {
            Size size = Utils.MeasureChar(c, Font);
            if (size.Width > CharSize.Width)
            {
                CharSize = new Size(size.Width, CharSize.Height);
            }

            if (size.Height > CharSize.Height)
            {
                CharSize = new Size(CharSize.Width, size.Height);
            }
        }

        private void AddChars(char[] chars)
        {
            foreach (var c in chars)
            {
                if (!Chars.ContainsKey(c))
                {
                    Chars.Add(c, new PixelMatrix(GetCharPixels(c), CharSize));
                }
            }
        }

        private byte[] GetCharPixels(char c)
        {
            Bitmap bmp = Utils.CharToBitmap(c, Font, CharSize);
            var img = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            return img.GetImagePixels();
        }
    }
}
