﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace vrisian
{
    public partial class AnimationOptionsDialog : Window
    {
        public bool Advanced { get; private set; }

        private readonly Animation Anim;

        public AnimationOptionsDialog(Animation A)
        {
            InitializeComponent();

            Anim = A;

            if (Anim.IsSet) { Viewer.Play(Anim); }
            

            OptionInterpolate.IsChecked = Anim.Interpolate;
            FrameTime.Text = Anim.FrameTime.ToString();
            OptionWidth.Text = Anim.SpriteSizeRatio.X.ToString();
            OptionHeight.Text = Anim.SpriteSizeRatio.Y.ToString();
            UpdateFrames();
        }

        private void UpdateFrames()
        {
            FrameList.Children.Clear();
            Anim.Frames.ForEach(F => AddFrame(F));
        }

        private void AddFrame(AnimationFrame F)
        {
            if (Anim.FrameTime != F.Time)
            {
                OptionAdvanced.IsChecked = true;
            }
            FrameList.Children.Add(GetFrameGrid(Anim, F));
        }

        private AnimationFrameGrid GetFrameGrid(Animation Anim, AnimationFrame Frame)
        {
            var G = new AnimationFrameGrid(FrameTime);
            G.FrameIndex.Text = Frame.Index.ToString();
            G.FrameTimeDefault.IsChecked = Frame.Time == Anim.FrameTime;
            G.FrameTime.Text = Frame.Time.ToString();
            return G;
        }

        private void OptionAdvanced_Checked(object sender, RoutedEventArgs e)
        {
            FrameListViewer.Visibility = Visibility.Visible;
            Advanced = true;
        }

        private void OptionAdvanced_Unchecked(object sender, RoutedEventArgs e)
        {
            FrameListViewer.Visibility = Visibility.Collapsed;
            Advanced = false;
        }


        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void FrameTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Anim.FrameTime = int.Parse(FrameTime.Text);
                int i = 0;
                foreach (AnimationFrameGrid G in FrameList.Children)
                {
                    if (G.FrameTimeDefault.IsChecked == true)
                    {
                        G.FrameTime.Text = FrameTime.Text;
                    }
                    Anim.Frames[i] = new AnimationFrame() { Index = int.Parse(G.FrameIndex.Text), Time = int.Parse(G.FrameTime.Text) };
                    i++;
                }
            }
            catch (Exception) { }
        }

        private void OptionInterpolate_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                Anim.Interpolate = (bool)OptionInterpolate.IsChecked;
            }
            catch (Exception) { }
        }

        private void OptionInterpolate_Unchecked(object sender, RoutedEventArgs e)
        {
            OptionInterpolate_Checked(sender, e);
        }

        private void OptionWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Anim.SpriteSizeRatio = XY.Parse(OptionWidth.Text, OptionHeight.Text);
            }
            catch (Exception) { }
            
        }

        private void OptionHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Anim.SpriteSizeRatio = XY.Parse(OptionWidth.Text, OptionHeight.Text);
            }
            catch (Exception) { }
        }

        private void NewFrameButton_Click(object sender, RoutedEventArgs e)
        {
            Anim.AddNewFrame();
            UpdateFrames();
        }
    }
}
