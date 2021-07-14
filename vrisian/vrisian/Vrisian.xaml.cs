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
        public float Zoom = 1f;
        public float MinZoom = 0.09f;
        public MainWindow()
        {
            InitializeComponent();
            TextEditor.Textbox = TextEditorTextbox;
            TextEditor.LineNumbers = TextLineNumbers;
            TextEditor.Zoom = Zoom;
            ImageEditor.Canvas = ImageEditorCanvas;
            ImageEditor.AnimationBar = AnimationBar;
            ImageEditor.ColorPicker = ImageEditorColorPicker;
            ImageEditor.SelectedColorViewer = ImageEditorSelectedColorViewer;
            ImageEditor.CanvasBackground = ImageEditorCanvasBackground;
            ImageEditor.Zoom = Zoom;
        }

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
            if (TextEditor.IsOpen)
            {
                TextEditor.CloseEditor();
            }
            if (ImageEditor.IsOpen)
            {
                ImageEditor.CloseEditor();
            }
        }

        private void zoomInButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateZoom(0.1f);
        }

        private void zoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateZoom(-0.1f);
        }

        private void UpdateZoom(float change)
        {
            if (Zoom + change < MinZoom)
            {
                return;
            }
            Zoom += change;
            TextEditor.Zoom = Zoom;
            ImageEditor.Zoom = Zoom;
            ImageEditor.RefreshImage();
            zoomLabel.Text = $"{Math.Round(100 * Zoom)}%";
        }

        
    }
}
