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

        public bool IsCurrent(Editors type) => CurrentEditor.Type == type;

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
        Editors Type { get; }

        void OpenEditor(DirectoryItem file);
        void CloseEditor();

        void Refresh();
        void SetZoom(double Zoom, bool mousecentered);

        void PreviousFrame();
        void NextFrame();
        void AddNewFrame();

        bool ShouldAnimate { get; }
        bool ShouldTile { get; }
    }

    public enum Editors
    {
        Text,
        Image
    }
}