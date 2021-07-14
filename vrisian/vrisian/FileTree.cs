using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace vrisian
{
	public partial class MainWindow : Window
    {
        private void FileTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DirectoryItem selectedItem;
            try
            {
                selectedItem = (DirectoryItem)FileTree.SelectedItem;
            }
            catch (Exception)
            {
                return;
            }

            openFilePathLabel.Text = selectedItem.SubPath;

            TextEditorGrid.Visibility = Visibility.Collapsed;
            if (TextEditor.IsOpen)
            {
                TextEditor.CloseEditor();
            }
            ImageEditorGrid.Visibility = Visibility.Collapsed;
            if (ImageEditor.IsOpen)
            {
                ImageEditor.CloseEditor();
            }

            if (new Regex(@"\.(txt|json)").IsMatch(selectedItem.Name))
            {
                //text file
                TextEditorGrid.Visibility = Visibility.Visible;

                TextEditor.OpenEditor(selectedItem);
            }
            else if (new Regex(@"\.(png)").IsMatch(selectedItem.Name))
            {
                //image file
                ImageEditorGrid.Visibility = Visibility.Visible;

                ImageEditor.OpenEditor(selectedItem);
            }
        }
    }
}
