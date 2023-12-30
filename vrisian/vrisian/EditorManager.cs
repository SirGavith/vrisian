using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace vrisian
{
    public class EditorManager
    {
        public List<UserControl> OpenEditors { get; } = new List<UserControl> { };
        public int CurrentEditorIndex { get; set; }
        public IEditor CurrentEditor { get {
                return OpenEditors == null || CurrentEditorIndex > OpenEditors.Count - 1 ? null :
                    (IEditor)OpenEditors[CurrentEditorIndex]; } }
        public IEditor Current { get { return CurrentEditor; } }

        public void Create(UserControl editor)
        {
            OpenEditors.Add(editor);
            CurrentEditorIndex = OpenEditors.Count - 1;
            editor.SetValue(Grid.RowProperty, 1);
            editor.SetValue(Grid.ColumnProperty, 2);
            Utils.Window.MainGrid.Children.Add(editor);
           
        }

        public bool IsCurrent(Editor.Editors type) => CurrentEditor.Type == type;

        public void CloseCurrent()
        {
            if (Current == null|| OpenEditors.Count == 0) { return; }
            Current?.CloseEditor();
            Utils.Window.MainGrid.Children.Remove(OpenEditors[CurrentEditorIndex]);
            OpenEditors.RemoveAt(CurrentEditorIndex);
            CurrentEditorIndex = OpenEditors.Count == 0 ? 0 : OpenEditors.Count - 1;
        }

        public void CloseAll()
        {
            OpenEditors.ForEach(e => ((IEditor)e).CloseEditor());
            OpenEditors.Clear();
            Utils.Window?.MainGrid.Children.Clear();
        }
    }

    public interface IEditor
    {
        Editor.Editors Type { get; }

        void OpenEditor(DirectoryItem file);
        void CloseEditor();
        void SetZoom(double Zoom, bool mousecentered);

        List<CustomCommandParent> GetCommands();
    }

    public static class Editor
    {
        public static readonly Dictionary<Editors, IEditor> EditorMappings = new Dictionary<Editors, IEditor>()
        {
            { Editors.Image, new ImageEditor() },
            { Editors.Text, new TextEditor() },
        };

        public enum Editors
        {
            Text,
            Image
        }
    }
}