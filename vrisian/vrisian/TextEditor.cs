using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace vrisian
{
    static class TextEditor
    {
        public static Boolean IsOpen = false;
        public static double Zoom;
        public static RichTextBox Textbox;
        public static TextBlock LineNumbers;
        private static DirectoryItem OpenFile;
        private static FlowDocument FD
        {
            get
            {
                return Textbox.Document;
            }
        }
        public static string Text
        {
            get
            {
                return new TextRange(FD.ContentStart, FD.ContentEnd).Text;
            }
        }
        public static int TextLength
        {
            get
            {
                return Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Length - 1;
            }
        }

        public static void OpenEditor(DirectoryItem file)
        {
            if (IsOpen)
            {
                return;
            }
            IsOpen = true;
            OpenFile = file;
            var text = File.ReadAllText(file.FullPath);
            var FD = new FlowDocument();
            // create nice FD from text with Zoom
            FD.Blocks.Add(new Paragraph(new Run(text)));


            Textbox.Document = FD;
        }

        public static void CloseEditor()
        {
            if (!IsOpen)
            {
                return;
            }
            using (var fileStream = File.Open(OpenFile.FullPath, FileMode.Create))
            {
                TextRange range = new TextRange(Textbox.Document.ContentStart, Textbox.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Text);
            }
            IsOpen = false;
            OpenFile = null;
        }
    }
    public partial class MainWindow : Window
    {
        public void TextEditorTextboxChanged(object sender, TextChangedEventArgs args)
        {
            if (!TextEditor.IsOpen) { return; }
            TextEditorLineNumbers.Text = Utils.GenerateLabels(TextEditor.TextLength);
            var Textbox = TextEditorTextbox;
            Textbox.Document.PageWidth = new FormattedText(TextEditor.Text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                new Typeface(Textbox.FontFamily, Textbox.FontStyle, Textbox.FontWeight, Textbox.FontStretch), Textbox.FontSize, Brushes.Black, 1).Width + 12;
        }

        private void TextEditorTextbox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            TextEditorLineNumbersScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
            TextEditorLineNumbersScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
        }

        private void TextEditorTextbox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
            {
                Utils.GetMainWindow().UpdateZoom(e.Delta > 0 ? 0.2 : -0.2, false);
                e.Handled = true;
            }
        }
    }
}