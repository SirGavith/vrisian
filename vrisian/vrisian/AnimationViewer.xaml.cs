using System;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Threading;

namespace vrisian
{

    public partial class AnimationViewer : UserControl
    {
        public Animation Anim { get; set; }
        private readonly DispatcherTimer Timer = new DispatcherTimer();
        private int CurrentFrameIndex = -1;
        private int TimeToNext = 0;

        private int NextFrameIndex => (CurrentFrameIndex + 1) % Anim.Frames.Count;
        private AnimationFrame CurrentFrame => Anim.Frames[CurrentFrameIndex];
        private ByteImage CurrentSprite => Anim.GetSprite(CurrentFrame.Index);
        private AnimationFrame NextFrame => Anim.Frames[NextFrameIndex];
        private ByteImage NextSprite => Anim.GetSprite(NextFrame.Index);

        public AnimationViewer()
        {
            InitializeComponent();
        }

        public void Play(Animation A)
        {
            Anim = A;
            Timer.Interval = TimeSpan.FromMilliseconds(50);
            Timer.IsEnabled = true;
            Timer.Tick += DrawNextFrame;
        }

        private void DrawNextFrame(object source, EventArgs e)
        {

            if (TimeToNext == 0)
            {
                CurrentFrameIndex++;
                CurrentFrameIndex %= Anim.Frames.Count;
                TimeToNext = CurrentFrame.Time;
            }

            ByteImage Sprite = CurrentSprite;

            if (Anim.Interpolate)
            {
                double Delta = 1D - (double)TimeToNext / CurrentFrame.Time;

                Sprite.ForEachPixel(XY =>
                {
                    Pixel Dest = NextSprite.Source[XY.X, XY.Y];
                    Pixel Source = Sprite.Source[XY.X, XY.Y];

                    Sprite.Source[XY.X, XY.Y] = Pixel.Lerp(Delta, Dest, Source);
                    
                });
                
            }
            Display.Source = Sprite.ToBitmap();

            TimeToNext--;
        }
    }
}
