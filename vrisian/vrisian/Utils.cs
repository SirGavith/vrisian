using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace vrisian
{
    static class Utils
    {
        public static XY[] Neighbors = new XY[8] 
            { new XY(-1, -1), new XY(-1, 0), new XY(-1, 1), new XY(0, 1), new XY(0, -1), new XY(1, 1), new XY(1, 0), new XY(1, -1) };
        public static XY[] Neighborhood = new XY[9]
            { new XY(-1, -1), new XY(-1, 0), new XY(-1, 1), new XY(0, 1), new XY(0, 0), new XY(0, -1), new XY(1, 1), new XY(1, 0), new XY(1, -1) };

        public static int Floor(double n)
        {
            return (int) Math.Floor(n);
        }

        public static string GenerateLabels(int max)
        {
            string labels = "";
            foreach (int value in Enumerable.Range(1, max))
            {
                labels += value.ToString();
                labels += Environment.NewLine;
            }
            return labels;
        }

        public static MainWindow GetMainWindow()
        {
            return (MainWindow) Application.Current.MainWindow;
        }
    }

    public struct XY
    {
        public XY(int x, int y)
        {
            X = x;
            Y = y;
        }

        public XY(double x, double y)
        {
            X = Utils.Floor(x);
            Y = Utils.Floor(y);
        }

        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString() => $"{X}, {Y}";
    }

    public struct Pixel
    {
        public Pixel(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

        public override string ToString() => $"{R}, {G}, {B}, {A}";

        public byte[] ToByteArray(bool IncludeAlpha)
        {
            byte[] output;

            if (IncludeAlpha)
            {
                output = new byte[4];
                output[3] = A;
            }
            else
            {
                output = new byte[3];
            }

            output[0] = B;
            output[1] = G;
            output[2] = R;

            return output;
        }
    }
}
