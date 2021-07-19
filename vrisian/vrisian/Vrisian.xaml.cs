using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace vrisian
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TextEditor.Textbox = TextEditorTextbox;
            TextEditor.LineNumbers = TextEditorLineNumbers;
            ImageEditor.Canvas = ImageEditorCanvas;
            ImageEditor.AnimationBar = ImageEditorAnimationBar;
            ImageEditor.ColorPicker = ImageEditorColorPicker;
            ImageEditor.SelectedColorViewer = ImageEditorSelectedColorViewer;
            ImageEditor.scroll = ImageEditorCanvasScrollViewer;
            ImageEditor.ZoomBorder = ImageEditorBorder;

            TextEditor.Zoom = Zoom;
        }

        public double Zoom = 1;
        public double MinZoom = 0.09;

        private void buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select the folder you want to open"
            };
            dlg.ShowDialog();
            try
            {
                var binding = new Binding("SubItems")
                {
                    Source = new DirectoryItem { FullPath = dlg.SelectedPath }
                };
                var selectedFolder = dlg.SelectedPath.Split('\\');
                FileTreeItem.Header = selectedFolder[selectedFolder.Length - 1];
                FileTreeItem.SetBinding(ItemsControl.ItemsSourceProperty, binding);
                FileTreePathDisplay.Text = dlg.SelectedPath;
                FileTree.IsEnabled = true;
            }
            catch (Exception)
            {
                Console.WriteLine("The selection dialog was likely closed");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            TextEditor.CloseEditor();
            ImageEditor.CloseEditor();
        }

        private void zoomInButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateZoom(0.1, false);
        }

        private void zoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateZoom(-0.1, false);
        }

        public void UpdateZoom(double change, bool usemousecentered)
        {
            if (Zoom + change < MinZoom)
            {
                return;
            }
            Zoom += change;
            TextEditor.Zoom = Zoom;
            ImageEditor.SetZoom(Zoom, usemousecentered);
            zoomLabel.Text = $"{Math.Round(100 * Zoom)}%";
        }
    }
}
