using System.Windows;

namespace vrisian
{
    public partial class AnimationViewerDialog : Window
    {
        public AnimationViewerDialog(Animation Anim)
        {
            InitializeComponent();
            Viewer.Play(Anim);
        }
    }
}
