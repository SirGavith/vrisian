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

            new CustomCommand(Key.O, ModifierKeys.Control,
                (object sender, ExecutedRoutedEventArgs e) => ButtonOpen_Click()
            );

            new CustomCommand(Key.N, ModifierKeys.Control,
                (object sender, ExecutedRoutedEventArgs e) => EditorManager.Current.AddNewFrame(),
                (object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = EditorManager.IsCurrent(Editors.Image) && EditorManager.Current.ShouldAnimate
            );
            new CustomCommand(Key.R, ModifierKeys.Control,
                (object sender, ExecutedRoutedEventArgs e) => EditorManager.Current.Refresh(),
                (object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = EditorManager.IsCurrent(Editors.Image)
            );
            new CustomCommand(Key.Left, ModifierKeys.None,
                (object sender, ExecutedRoutedEventArgs e) => { EditorManager.Current.NextFrame(); },
                (object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = EditorManager.IsCurrent(Editors.Image) && EditorManager.Current.ShouldAnimate
            );
            new CustomCommand(Key.T, ModifierKeys.Control, 
                (object sender, ExecutedRoutedEventArgs e) => EditorManager.Current.PreviousFrame(), 
                (object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = EditorManager.IsCurrent(Editors.Image) && EditorManager.Current.ShouldAnimate
            );
        }

        public EditorManager EditorManager = new EditorManager();

        public double Zoom = 1;
        public double MinZoom = 0.09;

        private void ButtonOpen_Click(object sender = null, RoutedEventArgs e = null)
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
            EditorManager.CloseAll();
        }

        private void ButtonZoomIn_Click(object sender, RoutedEventArgs e)
        {
            UpdateZoom(0.1, false);
        }

        private void ButtonZoomOut_Click(object sender, RoutedEventArgs e)
        {
            UpdateZoom(-0.1, false);
        }

        public void UpdateZoom(double change, bool usemousecentered, double multiplier = 1)
        {
            Zoom *= multiplier;
            if (Zoom < MinZoom)
            {
                Zoom = MinZoom;
            }
            if (change < 0 && Zoom + change < MinZoom)
            {
                return;
            }
            Zoom += change;

            EditorManager.Current.SetZoom(Zoom, usemousecentered);

            zoomLabel.Text = $"{Math.Round(100 * Zoom)}%";
        }
    }
}