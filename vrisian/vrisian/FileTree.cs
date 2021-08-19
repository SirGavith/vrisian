using System;
using System.Linq;
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

            
            //TODO: multiple editors
            EditorManager.CloseCurrent();

            string[] TextEditorFormats = new string[] { "txt", "json", "mcmeta" };
            string[] ImageEditorFormats = new string[] { "png" };

            string Format = selectedItem.Name.Split(new char[] { '.' }).Last().ToLower();

            if (TextEditorFormats.Contains(Format))
            {
                //text file
                EditorManager.Create(new TextEditor(selectedItem));
                
            }
            else if (ImageEditorFormats.Contains(Format))
            {
                //image file
                EditorManager.Create(new ImageEditor(selectedItem));
            }
            else
            {
                
                MessageBox.Show($"The format .{Format} cannot be understood or is not supported");
            }
        }
    }
}
