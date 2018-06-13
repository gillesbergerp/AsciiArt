using AsciiArt;
using AsciiArt.Converter;
using AsciiArt.RenderOptions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AsciiArtTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static char[] _chars = { 'i', '@', '#', '$', '%', '&', '8', 'B', 'M', 'W', '*', 'm', 'w', 'q', 'p', 'd', 'b', 'k', 'h', 'a', 'o', 'Q', '0', 'O', 'Z', 'X', 'Y', 'U', 'J', 'C', 'L', 't', 'f', 'j', 'z', 'x', 'n', 'u', 'v', 'c', 'r', ']', '[', '}', '{', '1', ')', '(', '|', '\\', '/', '?', 'I', 'l', '!', 'i', '>', '<', '+', '_', '-', '~', ';', '"', ':', '^', ',', '`', '\'', '.', ' ' };
        private static char[] _chars1 = { '$', '@', 'B', '%', '8', '&', 'W', 'M', '#', '*', 'o', 'a', 'h', 'k', 'b', 'd', 'p', 'q', 'w', 'm', 'Z', 'O', '0', 'Q', 'L', 'C', 'J', 'U', 'Y', 'X', 'z', 'c', 'v', 'u', 'n', 'x', 'r', 'j', 'f', 't', '/', '\\', '|', '(', ')', '1', '{', '}', '[', ']', '?', '-', '_', '+', '~', '<', '>', 'i', '!', 'l', 'I', ';', ':', ',', '"', '^', '`', '\'', '.', ' ' };
        private static char[] _chars2 = { ' ', '/', '\\', 'o', '_', '-', '|' };
        private static char[] _chars3 = { ' ', '/' };

        private AsciiFont _font;
        private Uri _source;
        private bool _isSourceGif;

        public MainWindow()
        {
            InitializeComponent();
            _font = new AsciiFont(new Font("Courier New", 16), _chars2);
        }

        private void BtnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToElementState(GridMain, "ImageShown", false);

            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg;*.gif)|*.png;*.jpeg;*.jpg;*.gif|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures)
            };

            if ((bool)dlg.ShowDialog())
            {
                _isSourceGif = false;
                _source = new Uri(dlg.FileName);
                ImgOriginal.Source = new BitmapImage(_source);
                UpdateInfos();
            }
        }

        private void BtnLoadGif_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToElementState(GridMain, "GifShown", false);

            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Image files (*.gif)|*.gif|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures)
            };

            if ((bool)dlg.ShowDialog())
            {
                _isSourceGif = true;
                _source = new Uri(dlg.FileName);
                MeOriginal.Source = _source;
                UpdateInfos();
            }
        }

        private void BtnChangeFont_Click(object sender, RoutedEventArgs e)
        {
            FontDialog dlg = new FontDialog
            {
                Owner = this,
                Font = _font
            };

            dlg.ShowDialog();

            if (dlg.DialogResult == true)
            {
                _font = dlg.Font;

                UpdateInfos();

                TbAscii.FontFamily = new System.Windows.Media.FontFamily(_font.Font.FontFamily.Name);
            }
        }

        private void BtnConvert_Click(object sender, RoutedEventArgs e)
        {
            if (_source == null)
            {
                return;
            }

            if (_isSourceGif)
            {
                ConvertAndSaveGif();
            }

            switch (CbOutput.SelectedIndex)
            {
                case 0:
                    ConvertToHtml();
                    break;
                case 1:
                    ConvertToImage();
                    break;
                default:
                    ConvertToText();
                    break;
            }
        }

        private RenderOption CreateOptions()
        {
            RenderingMode renderingMode = CbRenderingMode.SelectedIndex == 1 ? RenderingMode.Inverted : RenderingMode.Default;
            switch (CbRenderingOption.SelectedIndex)
            {
                case 1:
                    return new SsimBasedRenderOption(_font, renderingMode);
                case 2:
                    return new MseBasedRenderOption(_font);
                case 3:
                    return new ChiSquaredDistanceRenderOption(_font, renderingMode);
                case 4:
                    return new EuclideanDistanceRenderOption(_font, renderingMode);
                case 5:
                    return new EarthMoversDistanceRenderOption(_font, renderingMode);
                case 6:
                    return new IntersectionRenderOption(_font, renderingMode);
                case 7:
                    return new BhattacharyyaDistanceRenderOption(_font, renderingMode);
                case 8:
                    return new ManhattanDistanceRenderOption(_font, renderingMode);
                case 9:
                    return new MatusitaDistanceRenderOption(_font, renderingMode);
                default:
                    return new BrightnessBasedRenderOption(_font, renderingMode);
            }
        }

        private async void ConvertToText()
        {
            VisualStateManager.GoToElementState(GridMain, "TextShown", false);

            TextConverter converter = new TextConverter(CreateOptions());

            try
            {
                Progress<double> progress = new Progress<double>(UpdateProgress);
                TbAscii.Text = await converter.ConvertAsync(_source, progress);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Operation failed!");
            }
        }

        private async void ConvertToImage()
        {
            VisualStateManager.GoToElementState(GridMain, "ImageShown", false);

            DrawingMode mode = GetDrawingMode();

            BitmapSourceConverter converter = new BitmapSourceConverter(CreateOptions(), mode);

            try
            {
                Progress<double> progress = new Progress<double>(UpdateProgress);
                ImgAscii.Source = await converter.ConvertAsync(_source, progress);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Operation failed!");
            }
        }

        private async void ConvertToHtml()
        {
            VisualStateManager.GoToElementState(GridMain, "HtmlShown", false);

            DrawingMode mode = GetDrawingMode();

            HtmlConverter converter = new HtmlConverter(CreateOptions(), mode);

            try
            {
                Progress<double> progress = new Progress<double>(UpdateProgress);
                WvAscii.NavigateToString(await converter.ConvertAsync(_source, progress));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Operation failed!");
            }
        }

        private async void ConvertAndSaveGif()
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                FileName = "asciiart",
                DefaultExt = ".gif",
                Filter = "Image files (*.gif)|*.gif|All files (*.*)|*.*"
            };

            if (!(bool)dlg.ShowDialog())
            {
                return;
            }

            VisualStateManager.GoToElementState(GridMain, "GifShown", false);

            var options = CreateOptions();

            DrawingMode mode = GetDrawingMode();

            GifConverter converter = new GifConverter(options, mode);

            try
            {
                Progress<double> progress = new Progress<double>(UpdateProgress);
                await converter.ConvertAndSaveAsync(_source, new Uri(dlg.FileName), progress);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Operation failed!");
            }
        }

        private DrawingMode GetDrawingMode()
        {
            System.Drawing.Color background;
            if (CpBackground.SelectedColor == null)
            {
                background = System.Drawing.Color.FromArgb(255, 255, 255, 255);
            }
            else
            {
                var col = (System.Windows.Media.Color)CpBackground.SelectedColor;
                background = System.Drawing.Color.FromArgb(col.A, col.R, col.G, col.B);
            }

            System.Drawing.Color foreground;
            if (CpForeground.SelectedColor == null)
            {
                foreground = System.Drawing.Color.FromArgb(255, 255, 255, 255);
            }
            else
            {
                var col = (System.Windows.Media.Color)CpForeground.SelectedColor;
                foreground = System.Drawing.Color.FromArgb(col.A, col.R, col.G, col.B);
            }

            if (RbImgColorBack.IsChecked == true)
            {
                return new ImageColorAsBackgroundDrawingMode(foreground, SliderOpacity.Value);
            }
            else if (RbImgColorFore.IsChecked == true)
            {
                return new ImageColorAsForegroundDrawingMode(background);
            }


            return new DefaultDrawingMode(background, foreground);
        }

        private void UpdateInfos()
        {
            if (_source != null)
            {
                BitmapImage img = new BitmapImage(_source);
                TbInfos.Text = string.Format("Image dimensions: {0}px x {1}px\nFont dimensions: {2}px x {3}px\nResult: {4} chars x {5} chars",
                    img.PixelWidth, img.PixelHeight, _font.CharSize.Width, _font.CharSize.Height, img.PixelWidth / _font.CharSize.Width, img.PixelHeight / _font.CharSize.Height);
            }
        }

        private void UpdateProgress(double value)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                PbProgress.Value = value * 100;
            }));
        }
    }
}
