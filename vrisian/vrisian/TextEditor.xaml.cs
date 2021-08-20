using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace vrisian
{
    public partial class TextEditor : UserControl, IEditor
    {
        public Editors Type { get; } = Editors.Text;

        public double Zoom;
        private DirectoryItem OpenFile;
        private FlowDocument FD { get { return Textbox.Document; } set { Textbox.Document = value; } }
        public string Text { get { return new TextRange(FD.ContentStart, FD.ContentEnd).Text; } }

        public int TextLength { get { return Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Length - 1; }}

        public TextEditor(DirectoryItem file)
        {
            InitializeComponent();
            OpenEditor(file);
        }

        public void OpenEditor(DirectoryItem file)
        {
            OpenFile = file;
            var text = File.ReadAllText(file.FullPath);
            var Flow = new FlowDocument();
            // TODO: create nice FD from text with Zoom
            Flow.Blocks.Add(new Paragraph(new Run(text)));

            FD = Flow;

            Utils.Window.TextEditorCommands.Register();
        }

        public void CloseEditor()
        {
            using (var fileStream = File.Open(OpenFile.FullPath, FileMode.Create))
            {
                TextRange range = new TextRange(Textbox.Document.ContentStart, Textbox.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Text);
            }
            Utils.Window.TextEditorCommands.Deregister();
        }

        public void SetZoom(double zoom, bool mousecentered)
        {
            Zoom = zoom;
        }

        public void TextChanged(RichTextBox sender)
        {
            LineNumbers.Text = Utils.GenerateLabels(TextLength);
            var Textbox = sender;
            Textbox.Document.PageWidth = new FormattedText(Text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                new Typeface(Textbox.FontFamily, Textbox.FontStyle, Textbox.FontWeight, Textbox.FontStretch), Textbox.FontSize, Brushes.Black, 1).Width + 50;
        }

        public void Refresh() { }

        public void PreviousFrame() { }

        public void NextFrame() { }

        public void AddNewFrame() { }


        public bool ShouldAnimate { get; }
        public bool ShouldTile { get; }


        public void TextboxChanged(object sender, TextChangedEventArgs args)
        {
            TextChanged((RichTextBox)sender);
        }

        private void Textbox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            LineNumbersScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
            LineNumbersScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
        }

        private void Textbox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
            {
                Utils.CurrentWindow.UpdateZoom(e.Delta > 0 ? 0.2 : -0.2, false);
                e.Handled = true;
            }
        }
    }
}
