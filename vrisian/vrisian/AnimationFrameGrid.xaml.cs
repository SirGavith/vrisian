using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace vrisian
{
    public partial class AnimationFrameGrid : UserControl
    {
        private readonly TextBox DefaultFrameTimeText;
        public AnimationFrameGrid(TextBox frameTime)
        {
            InitializeComponent();
            DefaultFrameTimeText = frameTime;
        }
        //private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text

        //private static bool IsTextAllowed(string text)
        //{
        //    return !_regex.IsMatch(text);
        //}

        private void FrameTimeDefault_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            FrameTime.Text = DefaultFrameTimeText.Text;
        }

        private void FrameTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FrameTime.Text == DefaultFrameTimeText.Text)
            {
                FrameTimeDefault.IsChecked = true;
            }
            else
            {
                FrameTimeDefault.IsChecked = false;
            }
        }
    }
}
