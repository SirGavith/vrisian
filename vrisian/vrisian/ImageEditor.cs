using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit;

namespace vrisian
{
    class ImageEditor
    {
        // Elements
        public static Image Canvas;
        public static ColorPicker ColorPicker;
        public static Image SelectedColorViewer;
        public static RowDefinition AnimationBar;
        public static ScrollViewer scroll;
        public static Border ZoomBorder; 

        public static bool IsOpen = false;
        private static DirectoryItem OpenFile;

        public static ByteImage img;
        public static ByteImage background;

        //private static double Zoom = 1d;
        public static Pixel SelectedColor = new Pixel(100, 100, 100);
        public static bool ShouldTile = false;
        public static bool ShouldAnimate = false;
        public static Tools Tool = Tools.Move;

        public enum Tools
        {
            Move,
            Select,
            Pencil,
            Fill,
            Erase
        }

        public static void OpenEditor(DirectoryItem file)
        {
            if (IsOpen) { return; }
            IsOpen = true;
            OpenFile = file;

            img = new ByteImage(file.FullPath);
            RefreshBackground();
            RefreshImage();
        }

        public static void CloseEditor()
        {
            if (!IsOpen) { return; }
            img.Save();

            IsOpen = false;
            OpenFile = null;
        }

        public static void OnMouseTrigger(object s, MouseEventArgs e)
        {
            if (!IsOpen) { return; }
            Point pos = e.GetPosition((IInputElement) s);

            XY xy = new XY(pos.X / Canvas.ActualWidth * img.Width,
                pos.Y / Canvas.ActualHeight * img.Height);

            if (img.SetPixel(xy, SelectedColor))
            {
                RefreshImage();
            } 
        }

        public static void RefreshImage()
        {
            if (background == null) { RefreshBackground(); }

            if (img == null) { return; }

            ByteImage mainImg = background.Copy();

            if (ShouldTile)
            {
                foreach (XY offset in Utils.Neighborhood)
                {
                    mainImg.Blit(img, (offset.X + 1) * img.Width, (offset.Y + 1) * img.Height);
                }
            }
            else
            {
                mainImg.Blit(img);
            }

            Canvas.Source = mainImg.ToBitmap();
        }

        public static void RefreshBackground()
        {
            int bkgdSizeModifier = ShouldTile ? 3 : 1;
            background = new ByteImage(img.Width * bkgdSizeModifier, img.Height * bkgdSizeModifier);

            //checker background
            background.ForEachPixel((x, y) => { 
                if ((x + y) % 2 == 0) { 
                    background.SetPixel(x, y, new Pixel(220, 220, 220));
                }
            });
        }

        public static void Refresh()
        {
            RefreshBackground();
            RefreshImage();
        }

        public static void SetZoom(double Zoom, bool mousecentered)
        {
            Point relative = Mouse.GetPosition(Canvas);

            var oldLocation = Canvas.LayoutTransform.Transform(relative);

            Canvas.LayoutTransform = new ScaleTransform(Zoom, Zoom);

            if (!mousecentered) {
                //go to middle
                scroll.ScrollToVerticalOffset(scroll.ScrollableHeight + 20 / 2);
                scroll.ScrollToHorizontalOffset(scroll.ScrollableWidth + 20 / 2);
                return;
            }

            var newLocation = Canvas.LayoutTransform.Transform(relative);

            var shift = (newLocation - oldLocation) * 1.5;

            scroll.ScrollToVerticalOffset(scroll.VerticalOffset + shift.Y);
            scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset + shift.X);
        }
    }

    public partial class MainWindow : Window
    {
        private void optionAnimate_Checked(object sender, RoutedEventArgs e)
        {
            ImageEditorAnimationBarGridSplitter.Height = new GridLength(5);
            ImageEditorAnimationBar.Height = new GridLength(1, GridUnitType.Star);
            
            ImageEditor.ShouldAnimate = true;
            ImageEditor.RefreshImage();
        }

        private void optionAnimate_Unchecked(object sender, RoutedEventArgs e)
        {
            ImageEditorAnimationBarGridSplitter.Height = new GridLength(0);
            ImageEditorAnimationBar.Height = new GridLength(0);
            
            ImageEditor.ShouldAnimate = true;
            ImageEditor.RefreshImage();
        }

        private void optionTile_Checked(object sender, RoutedEventArgs e)
        {
            ImageEditor.ShouldTile = true;
            ImageEditor.Refresh();
            Zoom *= 3;
            UpdateZoom(0, false);
        }

        private void optionTile_Unchecked(object sender, RoutedEventArgs e)
        {
            ImageEditor.ShouldTile = false;
            ImageEditor.Refresh();
            Zoom /= 3;
            UpdateZoom(0, false);
        }

        private void ImageEditorCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ImageEditor.OnMouseTrigger(sender, e);
        }

        private void ImageEditorCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ImageEditor.OnMouseTrigger(sender, e);
            }
        }

        //public event EventHandler ZoomShouldUpdate;

        private void ImageEditorZoomBorder_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Alt) > 0)
            {
                Utils.GetMainWindow().UpdateZoom(e.Delta > 0 ? 0.2 : -0.2, true);
                e.Handled = true;
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
            {
                if (e.Delta > 0)
                {
                    ImageEditorCanvasScrollViewer.LineLeft();
                }
                else
                {
                    ImageEditorCanvasScrollViewer.LineRight();
                }
                e.Handled = true;
            }
        }

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Color c = (Color) ((ColorPicker) sender).SelectedColor;
            ImageEditor.SelectedColor = new Pixel(c.R, c.G, c.B, c.A);
            ImageEditor.SelectedColorViewer.Source = new ByteImage(new Pixel[1, 1] { { new Pixel(c.R, c.G, c.B, c.A) } }).ToBitmap();
        }
    }

    public class ZoomBorder : Border
    {

        private UIElement child = null;
        private Point origin;
        private Point start;
        

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != Child)
                    Initialize(value);
                base.Child = value;
            }
        }

        public void Initialize(UIElement element)
        {
            child = element;
            if (child != null)
            {
                //TranslateTransform tt = new TranslateTransform();

                //TransformGroup group = new TransformGroup();
                //group.Children.Add(st);
                //group.Children.Add(tt);

                //child.RenderTransform = group;
                //child.RenderTransformOrigin = new Point(0.0, 0.0);

                MouseWheel += child_PreviewMouseWheel;
                //MouseLeftButtonDown += child_MouseLeftButtonDown;
                //MouseLeftButtonUp += child_MouseLeftButtonUp;
                //MouseMove += child_MouseMove;
                //PreviewMouseRightButtonDown += new MouseButtonEventHandler(
                //  child_PreviewMouseRightButtonDown);
            }
        }

        public void Reset()
        {
            if (child != null)
            {
                //reset zoom

            }
        }

        #region Child Events

        private void child_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            
        }

        //private void child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (child != null)
        //    {

        //        var tt = GetTranslateTransform(child);
        //        start = e.GetPosition(this);
        //        origin = new Point(tt.X, tt.Y);
        //        Cursor = Cursors.Hand;
        //        child.CaptureMouse();
        //    }
        //}

        

        //private void child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (child != null)
        //    {
        //        child.ReleaseMouseCapture();
        //        Cursor = Cursors.Arrow;
        //    }
        //}

        //void child_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    Reset();
        //}

        //private void child_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (child != null)
        //    {
        //        if (child.IsMouseCaptured)
        //        {
        //            var tt = GetTranslateTransform(child);
        //            Vector v = start - e.GetPosition(this);
        //            tt.X = origin.X - v.X;
        //            tt.Y = origin.Y - v.Y;
        //        }
        //    }
        //}

        #endregion
    }
}