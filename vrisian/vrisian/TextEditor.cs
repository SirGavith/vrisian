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

namespace vrisian
{
    static class TextEditor
    {
        public static Boolean IsOpen = false;
        public static float Zoom;
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
        private static string Text
        {
            get
            {
                return new TextRange(FD.ContentStart, FD.ContentEnd).Text;
            }
        }
        private static int TextLength
        {
            get
            {
                return Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Length - 1;
            }
        }
        public static string UiLineLabels
        {
            get
            {
                string labels = "";
                foreach (int value in Enumerable.Range(1, TextLength))
                {
                    labels += value.ToString();
                    labels += Environment.NewLine;
                }
                return labels;
            }
        }

        public static void OpenEditor(DirectoryItem file)
        {
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
            using (var fileStream = File.Open(OpenFile.FullPath, FileMode.Create))
            {
                TextRange range = new TextRange(Textbox.Document.ContentStart, Textbox.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Text);
            }
            IsOpen = false;
            OpenFile = null;
        }

        public static void TextChanged(object sender, TextChangedEventArgs args)
        {
            LineNumbers.Text = UiLineLabels;
            Textbox.Document.PageWidth = new FormattedText(Text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                new Typeface(Textbox.FontFamily, Textbox.FontStyle, Textbox.FontWeight, Textbox.FontStretch), Textbox.FontSize, Brushes.Black).Width + 12;
        }
    }
    public partial class MainWindow : Window
    {
        public void TextEditorTextboxChanged(object sender, TextChangedEventArgs args)
        {
            if (TextEditor.IsOpen)
            {
                TextEditor.TextChanged(sender, args);
            }
        }
    }
}