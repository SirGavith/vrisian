using System;
using System.Collections.Generic;
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

            var GlobalShortcuts = new CustomCommandCategory("Global", "These shortcuts can be used anywhere", new List<CustomCommandParent>
            {
                new CustomCommand("Open Folder", "Opens the Folder Picker dialog", new KeyGesture(Key.O, ModifierKeys.Control), OpenFolderDialog),
                new CustomCommand("Open Folder", "Opens the Folder Picker dialog", new KeyGesture(Key.O, ModifierKeys.Control), OpenKeyboardShortcutDialog),
            });

            GlobalShortcuts.Register();


            //new CustomCommand(Key.O, ModifierKeys.Control,
            //    (object sender, ExecutedRoutedEventArgs e) => ButtonOpen_Click()
            //);

        }

        public EditorManager EditorManager = new EditorManager();

        public double Zoom = 1;
        public double MinZoom = 0.09;


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

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog();
        }

        private void OpenFolderDialog()
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select the folder you want to open"
            };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var selectedFolder = dlg.SelectedPath.Split('\\');
                FileTreeItem.Header = selectedFolder[selectedFolder.Length - 1];
                FileTreeItem.SetBinding(ItemsControl.ItemsSourceProperty, 
                    new Binding("SubItems") { Source = new DirectoryItem { FullPath = dlg.SelectedPath } });
                FileTreePathDisplay.Text = dlg.SelectedPath;
                FileTree.IsEnabled = true;
            }
        }

        private void OpenKeyboardShortcutsButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void OpenKeyboardShortcutDialog()
        {
            foreach (IEditor E in Editor.EditorMappings.Values)
            {
                List<CustomCommandParent> C = E.GetCommands();
            }
        }
    }
}