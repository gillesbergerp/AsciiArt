using AsciiArt;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AsciiArtTest
{
    /// <summary>
    /// Interaction logic for FontDialog.xaml
    /// </summary>
    public partial class FontDialog : Window
    {
        public static readonly DependencyProperty FontProperty = DependencyProperty.Register(
                nameof(Font), 
                typeof(AsciiFont), 
                typeof(FontDialog),
                new UIPropertyMetadata(null, FontChanged));

        public AsciiFont Font
        {
            get { return (AsciiFont)GetValue(FontProperty); }
            set { SetValue(FontProperty, value); }
        }

        private static void FontChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is AsciiFont font)
            {
                var dlg = ((FontDialog)sender);
                dlg.TbAlphabet.Text = string.Join("", font.Chars.Select(x => x.Key));
                dlg.TbFontSize.Text = font.Font.Size.ToString();

                dlg.CbFontStyle.SelectedIndex = (int)font.Font.Style;

                dlg.CbFontFamily.SelectedItem = Fonts.SystemFontFamilies.Where(x => x.FamilyNames.Select(y => y.Value).Contains(font.Font.FontFamily.Name)).FirstOrDefault();
            }
        }

        public FontDialog()
        {
            InitializeComponent();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (CbFontFamily.SelectedItem == null)
                return;

            char[] alphabet = TbAlphabet.Text.ToCharArray();
            if (alphabet.Length == 0)
            {
                alphabet = new char[] { ' ' };
            }

            var font = new Font(((System.Windows.Media.FontFamily)CbFontFamily.SelectedItem).FamilyNames.First().Value, float.Parse(TbFontSize.Text, CultureInfo.InvariantCulture.NumberFormat), (System.Drawing.FontStyle)CbFontStyle.SelectedIndex);
            
            Font = new AsciiFont(font, alphabet);

            DialogResult = true;
        }

        private void TbFontSize_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(((TextBox)sender).Text);
        }

        private void TbFontSize_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(typeof(string)) || !IsTextNumeric((string)e.DataObject.GetData(typeof(string))))
                e.CancelCommand();
        }

        private static bool IsTextNumeric(string text)
        {
            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(text);
        }
    }
}
