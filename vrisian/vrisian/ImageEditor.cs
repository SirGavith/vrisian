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
        public static Image CanvasBackground;
        public static ColorPicker ColorPicker;
        public static Image SelectedColorViewer;
        public static RowDefinition AnimationBar;

        private static int DefaultHeight = 200;
        private static int DefaultWidth = 200;

        public static bool IsOpen = false;
        private static DirectoryItem OpenFile;
        public static ByteImage img;
        public static Pixel SelectedColor = new Pixel(100, 100, 100);
        public static Boolean ShouldTile = false;
        public static float Zoom;


        public static void OpenEditor(DirectoryItem file)
        {
            IsOpen = true;
            OpenFile = file;

            img = new ByteImage(file.FullPath);
            RefreshImage();
        }

        public static void CloseEditor()
        {
            img.Save();

            IsOpen = false;
            OpenFile = null;
        }

        public static void OnMouseTrigger(object s, MouseEventArgs e)
        {
            IInputElement sender = (IInputElement) s;
            Point pos = e.GetPosition(sender);
            pos.X = Math.Floor(pos.X / Canvas.Width * img.Width);
            pos.Y = Math.Floor(pos.Y / Canvas.Height * img.Height);

            img.Source[(int) pos.X, (int) pos.Y] = SelectedColor;

            RefreshImage();
        }

        public static void RefreshImage()
        {

            Canvas.Width = DefaultWidth * Zoom;
            Canvas.Height = DefaultHeight * Zoom;
            if (img == null)
            {
                return;
            }
            Canvas.Source = img.ToBitmap();

            int bkgdSizeModifier = 1;

            if (ShouldTile)
            {
                bkgdSizeModifier = 3;
            } 


            Pixel[,] bkgd = new Pixel[img.Width * bkgdSizeModifier, img.Height * bkgdSizeModifier];

            //checker background
            for (var x = 0; x < bkgd.GetLength(0); x++)
            {
                for (var y = 0; y < bkgd.GetLength(1); y++)
                {
                    if ((x + y) % 2 == 0)
                    {
                        bkgd[x, y] = new Pixel(220, 220, 220);
                    }
                }
            }

            if (ShouldTile)
            {
                Point[] offsets = new Point[8] { new Point(-1, -1), new Point(-1, 0), new Point(-1, 1), new Point(0, 1), new Point(0, -1), new Point(1, 1), new Point(1, 0), new Point(1, -1) };
                foreach (Point offset in offsets)
                {
                    for (int x = 0; x < img.Width; x++)
                    {
                        for (int y = 0; y < img.Height; y++)
                        {
                            if (img.Source[x, y].A != 0)
                            {
                                bkgd[x + ((int) offset.X + 1) * img.Width, y + ((int)offset.Y + 1) * img.Height] = img.Source[x, y];
                            }
                        }
                    }
                }
            }

            CanvasBackground.Width = DefaultWidth * Zoom * bkgdSizeModifier;
            CanvasBackground.Height = DefaultHeight * Zoom * bkgdSizeModifier;
            CanvasBackground.Source = new ByteImage(bkgd).ToBitmap();
        }

        public static void ColorSelected(ColorPicker sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Color c = (Color)sender.SelectedColor;
            SelectedColor = new Pixel(c.R, c.G, c.B, c.A);
            SelectedColorViewer.Source = new ByteImage(new Pixel[1, 1] { { new Pixel(c.R, c.G, c.B, c.A) } }).ToBitmap();
        }

        public static void ShowAnimationPanel()
        {
            AnimationBar.Height = new GridLength(1, GridUnitType.Star);
        }

        public static void HideAnimationPanel()
        {
            AnimationBar.Height = new GridLength(0);
        }  
    }

    public partial class MainWindow : Window
    {
        private void optionAnimate_Checked(object sender, RoutedEventArgs e)
        {
            ImageEditor.ShowAnimationPanel();
        }

        private void optionAnimate_Unchecked(object sender, RoutedEventArgs e)
        {
            ImageEditor.HideAnimationPanel();
        }

        private void optionTile_Checked(object sender, RoutedEventArgs e)
        {
            ImageEditor.ShouldTile = true;

            ImageEditor.RefreshImage();
        }

        private void optionTile_Unchecked(object sender, RoutedEventArgs e)
        {
            ImageEditor.ShouldTile = false;

            ImageEditor.RefreshImage();
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

        private void ImageEditorCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Alt) > 0)
            {
                UpdateZoom(e.Delta > 0 ? 0.1f : -0.1f);
            }
        }

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            ImageEditor.ColorSelected((ColorPicker) sender, e);
        }
    }
}