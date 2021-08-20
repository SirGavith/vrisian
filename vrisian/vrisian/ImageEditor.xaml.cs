using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
//using Xceed.Wpf.Toolkit;


namespace vrisian
{

    public partial class ImageEditor : UserControl, IEditor
    {
        public Editors Type { get; } = Editors.Image;

        public DirectoryItem OpenFile;

        public ByteImage Img;

        public ByteImage Bkgd;
        public Animation Anim;

        public Pixel SelectedColor = new Pixel(100, 100, 100);
        public Tools Tool = Tools.Move;

        public bool ShouldAnimate { get; protected set; }
        public bool ShouldTile { get; protected set; }

        public ImageEditor()
        {
            InitializeComponent();
        }

        public ImageEditor(DirectoryItem file)
        {
            InitializeComponent();
            OpenEditor(file);
        }

        public enum Tools
        {
            Move,
            Select,
            Pencil,
            Fill,
            Erase
        }

        public void OpenEditor(DirectoryItem file)
        {
            OpenFile = file;

            Img = new ByteImage(file.FullPath);

            Utils.Window.ImageEditorCommands.Register();

            //look for .mcmeta
            if (File.Exists(OpenFile.FullPath + ".mcmeta"))
            {
                OptionAnimate.IsChecked = true;
            }
            Refresh();
        }

        public void PreviousFrame()
        {
            Anim.PreviousFrame();
            Refresh();
        }

        public void NextFrame()
        {
            Anim.NextFrame();
            Refresh();
        }

        public void AddNewFrame()
        {
            Anim.AddNewFrame();
            Anim.CurrentSpriteIndex = Anim.Frames.Count - 1;
            Refresh();
            SetAnimationBar();
        }

        private void SetAnimationBar()
        {
            AnimationFrames.Children.Clear();
            for(int i = 0; i < Anim.Frames.Count; i++)
            {
                var b = new Button
                {
                    Content = i.ToString(),
                    Tag = i
                };
                b.Click += AnimationFrameButton_Click;
                AnimationFrames.Children.Add(b);
            }
        }

        public void AnimationFrameButton_Click(object sender, RoutedEventArgs e)
        {
            int Index = (int)((Button)sender).Tag;
            Anim.CurrentSpriteIndex = Index;
            Refresh();
        }

        public void LoadAsAnimation()
        {
            Anim = new Animation(Img, OpenFile);
        }

        public void CloseEditor()
        {
            Img.Save();
            Anim?.SaveMCMeta();
            Utils.Window.ImageEditorCommands.Deregister();
        }

        public void OnMouseTrigger(object s, MouseEventArgs e)
        {
            Point pos = e.GetPosition((IInputElement)s);

            XY xy = new XY(pos.X / Canvas.ActualWidth * Anim.SpriteSize.X + Anim.CurrentSpriteOffset.X,
                pos.Y / Canvas.ActualHeight * Anim.SpriteSize.Y + Anim.CurrentSpriteOffset.Y);

            if (Img.SetPixel(xy, SelectedColor))
            {
                RefreshImage();
            }
        }

        public void Refresh()
        {
            RefreshBackground();
            RefreshImage();
        }

        public void RefreshImage()
        {
            if (Bkgd == null) { RefreshBackground(); }

            if (Img == null) { return; }

            ByteImage mainImg = Bkgd.Copy();

            var frame = ShouldAnimate ? Anim.CurrentSprite : Img;

            if (ShouldTile)
            {
                foreach (XY offset in Utils.Neighborhood)
                {
                    mainImg.Blit(frame, (offset.X + 1) * frame.Width, (offset.Y + 1) * frame.Height);
                }
            }
            else
            {
                mainImg.Blit(frame);
            }

            Canvas.Source = mainImg.ToBitmap();
        }

        public void RefreshBackground()
        {
            int bkgdSizeModifier = ShouldTile ? 3 : 1;
            Bkgd = new ByteImage((ShouldAnimate ? Anim.SpriteSize : new XY(Img.Width, Img.Height)) * bkgdSizeModifier);

            //checker background
            Bkgd.ForEachPixel((x, y) =>
            {
                if ((x + y) % 2 == 0)
                {
                    Bkgd.SetPixel(x, y, new Pixel(220, 220, 220));
                }
            });
        }

        public void SetZoom(double Zoom, bool mousecentered)
        {
            Point relative = Mouse.GetPosition(Canvas);

            var oldLocation = Canvas.LayoutTransform.Transform(relative);

            Canvas.LayoutTransform = new ScaleTransform(Zoom, Zoom);

            ScrollViewer scroll = CanvasScrollViewer;

            if (!mousecentered)
            {
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

        public void AnimationOn()
        {
            ShouldAnimate = true;

            try
            {
                LoadAsAnimation();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Could not load as Animation. {e.Message}");
                Console.WriteLine($"Could not load as Animation. {e.Message}");
                OptionAnimate.IsChecked = false;
                return;
            }
            Utils.Window.ImageEditorAnimationCommands.Register();

            SetAnimationBar();
            Refresh();
        }

        public void AnimationOff()
        {
            ShouldAnimate = false;

            Utils.Window.ImageEditorAnimationCommands.Deregister();

            Refresh();
        }

        public void TilingOn()
        {
            ShouldTile = true;
            Refresh();
            Utils.CurrentWindow.UpdateZoom(0, false, 3);
        }

        public void TilingOff()
        {
            ShouldTile = false;
            Refresh();
            Utils.CurrentWindow.UpdateZoom(0, false, 1d / 3d);
        }



        public void OptionAnimate_Checked(object sender = null, RoutedEventArgs e = null)
        {
            AnimationBarGridSplitter.Height = new GridLength(5);
            AnimationBar.Height = new GridLength(1, GridUnitType.Star);
            AnimationOn();
        }

        public void OptionAnimate_Unchecked(object sender = null, RoutedEventArgs e = null)
        {
            AnimationBarGridSplitter.Height = new GridLength(0);
            AnimationBar.Height = new GridLength(0);
            AnimationOff();
        }

        private void OptionTile_Checked(object sender, RoutedEventArgs e)
        {
            TilingOn();
        }

        private void OptionTile_Unchecked(object sender, RoutedEventArgs e)
        {
            TilingOff();
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnMouseTrigger(sender, e);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                OnMouseTrigger(sender, e);
            }
        }

        private void ZoomBorder_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Alt) > 0)
            {
                Utils.CurrentWindow.UpdateZoom(e.Delta > 0 ? 0.2 : -0.2, true);
                e.Handled = true;
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
            {
                if (e.Delta > 0)
                {
                    CanvasScrollViewer.LineLeft();
                }
                else
                {
                    CanvasScrollViewer.LineRight();
                }
                e.Handled = true;
            }
        }

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Color c = (Color) ColorPicker.SelectedColor;
            SelectedColor = new Pixel(c.R, c.G, c.B, c.A);
            SelectedColorViewer.Source = new ByteImage(new Pixel[1, 1] { { new Pixel(c.R, c.G, c.B, c.A) } }).ToBitmap();
        }

        private void AnimationNewFrame_Click(object sender, RoutedEventArgs e)
        {
            AddNewFrame();
        }

        private void ExecuteAddNewFrameCommand(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Custom Command Executed");
        }



        private void AnimationPrev_Click(object sender, RoutedEventArgs e)
        {
            PreviousFrame();
        }

        private void AnimationNext_Click(object sender, RoutedEventArgs e)
        {
            NextFrame();
        }

        private void AnimationPlay_Click(object sender, RoutedEventArgs e)
        {
            var Dialog = new AnimationViewerDialog(Anim);
            Dialog.ShowDialog();
        }

        private void AnimationOptions_Click(object sender, RoutedEventArgs e)
        {
            var AnimCopy = Anim.Copy();

            var Dialog = new AnimationOptionsDialog(Anim);
            bool? Result = Dialog.ShowDialog();
            if (Result == true)
            {
                //Anim.Interpolate = (bool)Dialog.OptionInterpolate.IsChecked;
                //Anim.SpriteSizeRatio = XY.Parse(Dialog.OptionWidth.Text, Dialog.OptionHeight.Text);
                //Anim.FrameTime = int.Parse(Dialog.FrameTime.Text);
                //int i = 0;
                //foreach (AnimationFrameGrid G in Dialog.FrameList.Children)
                //{
                //    Anim.Frames[i] = new AnimationFrame() { Index = int.Parse(G.FrameIndex.Text), Time = int.Parse(G.FrameTime.Text) };
                //    i++;
                //}
                Anim.SaveMCMeta();
            }
            else
            {
                Anim = AnimCopy;
            }
        }
    }
}
