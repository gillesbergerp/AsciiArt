using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AsciiArt
{
    /// <summary>
    /// Defines how a converted image is drawn.
    /// </summary>
    public abstract class DrawingMode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingMode"/> class.
        /// </summary>
        /// <param name="background">A color that represents the image's background.</param>
        /// <param name="foreground">A color that represents the image's foreground.</param>
        public DrawingMode(Color background, Color foreground)
        {
            Background = background;
            Foreground = foreground;
        }

        /// <summary>
        /// Gets the color that describes the background of the resulting image
        /// </summary>
        protected Color Background { get; private set; }

        /// <summary>
        /// Gets the color that describes the foreground of the resulting image
        /// </summary>
        protected Color Foreground { get; private set; }

        /// <summary>
        /// Calculates a color based on the color of the char and the color of a specific pixel of the source image by determining whether the pixel is part of the foreground or the background 
        /// </summary>
        /// <param name="charColor">The color that represents the character's pixel</param>
        /// <param name="imageColor">The color that represents the image's pixel</param>
        /// <returns>The calculated color</returns>
        public abstract Color CalculateColor(Color charColor, Color imageColor);
    }

    /// <summary>
    /// Uses the specified foreground and background. 
    /// </summary>
    public class DefaultDrawingMode : DrawingMode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDrawingMode"/> class.
        /// </summary>
        /// <param name="background">A color that represents the image's background.</param>
        /// <param name="foreground">A color that represents the image's foreground.</param>
        public DefaultDrawingMode(Color background, Color foreground) : base(background, foreground)
        {
        }

        /// <summary>
        /// Returns the specified background color, if this pixel is part of the background. If not the returned value is calculated using the specified foreground color
        /// </summary>
        /// <param name="charColor">The color that represents the character's pixel</param>
        /// <param name="imageColor">The color that represents the image's pixel</param>
        /// <returns>The calculated color</returns>
        public override Color CalculateColor(Color charColor, Color imageColor)
        {
            if (charColor.R != 255 && charColor.G != 255 && charColor.B != 255)
            {
                byte r = (byte)Utils.ClampColorChannel(Foreground.R - charColor.R, 0, Foreground.R + 150);
                byte g = (byte)Utils.ClampColorChannel(Foreground.G - charColor.G, 0, Foreground.G + 150);
                byte b = (byte)Utils.ClampColorChannel(Foreground.B - charColor.B, 0, Foreground.B + 150);
                return Foreground;
            }

            return Background;
        }
    }

    /// <summary>
    /// Uses the original image's color as foreground.
    /// </summary>
    public class ImageColorAsForegroundDrawingMode : DrawingMode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageColorAsForegroundDrawingMode"/> class.
        /// </summary>
        /// <param name="background">A color that represents the image's background.</param>
        public ImageColorAsForegroundDrawingMode(Color background) : base(background, Color.FromArgb(0))
        {
        }
        
        /// <summary>
        /// Returns the specified background color, if this pixel is part of the background. If not the returned value is calculated using imageColor
        /// </summary>
        /// <param name="charColor">The color that represents the character's pixel</param>
        /// <param name="imageColor">The color that represents the image's pixel</param>
        /// <returns>The calculated color</returns>
        public override Color CalculateColor(Color charColor, Color imageColor)
        {
            if (charColor.R != 255 && charColor.G != 255 && charColor.B != 255)
            {
                byte r = (byte)Utils.ClampColorChannel(imageColor.R - charColor.R, 0, imageColor.R + 150);
                byte g = (byte)Utils.ClampColorChannel(imageColor.G - charColor.G, 0, imageColor.G + 150);
                byte b = (byte)Utils.ClampColorChannel(imageColor.B - charColor.B, 0, imageColor.B + 150);

                return Color.FromArgb(255, imageColor.R, imageColor.G, imageColor.B);
            }

            return Background;
        }
    }

    /// <summary>
    /// Draws the original image in the background.
    /// </summary>
    public class ImageColorAsBackgroundDrawingMode : DrawingMode
    {
        private readonly double _backgroundOpacity;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageColorAsBackgroundDrawingMode"/> class.
        /// </summary>
        /// <param name="foreground">A color that represents the image's foreground.</param>
        /// <param name="backgroundOpacity">Determines the opacity of the image's background. from 0.0 to 1.0.</param>
        public ImageColorAsBackgroundDrawingMode(Color foreground, double backgroundOpacity) : base(Color.FromArgb(0), foreground)
        {
            if (backgroundOpacity < 0)
            {
                _backgroundOpacity = 0;
            }
            else if (backgroundOpacity > 1)
            {
                _backgroundOpacity = 1;
            }
            else
            {
                _backgroundOpacity = backgroundOpacity;
            }
        }

        /// <summary>
        /// Returns imageColor, if this pixel is part of the background. If not the returned value is calculated using the specified foreground
        /// </summary>
        /// <param name="charColor">The color that represents the character's pixel</param>
        /// <param name="imageColor">The color that represents the image's pixel</param>
        /// <returns>The calculated color</returns>
        public override Color CalculateColor(Color charColor, Color imageColor)
        {
            if (charColor.R != 255 && charColor.G != 255 && charColor.B != 255)
            {
                byte r = (byte)Utils.ClampColorChannel(Foreground.R - charColor.R, 0, Foreground.R + 150);
                byte g = (byte)Utils.ClampColorChannel(Foreground.G - charColor.G, 0, Foreground.G + 150);
                byte b = (byte)Utils.ClampColorChannel(Foreground.B - charColor.B, 0, Foreground.B + 150);

                return Color.FromArgb(255, Foreground.R, Foreground.G, Foreground.B);
            }

            return Color.FromArgb((int)(255 * _backgroundOpacity), imageColor.R, imageColor.G, imageColor.B);
        }
    }
}
