using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

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

        public static MainWindow CurrentWindow
        {
            get
            {
                return (MainWindow) Application.Current.MainWindow;
            }
        }

        public static MainWindow Window { get { return CurrentWindow; } }

        public static bool IsInteger(double n)
        {
            return (Math.Abs(n % 1) <= (double.Epsilon * 100));
        }


        public static string RemoveWhitespace(this string str)
        {
            Regex.Replace(str, @"\s+", "");
            return str;
        }
        
        public static double Lerp(double by, double d1, double d2)
        {
            return d1 + (d2 - d1) * by;
        }

        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
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

        public static XY operator *(XY xy, int n)
        {
            return new XY(xy.X * n, xy.Y * n);
        }

        public override string ToString() => $"{X}, {Y}";

        public static XY Parse(string text1, string text2)
        {
            return new XY(int.Parse(text1), int.Parse(text2));
        }
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

        public static Pixel[,] Slice(Pixel[,] source, XY pos, XY size)
        {
            if (pos.X < 0 || pos.Y < 0 || pos.X >= source.GetLength(0) || pos.Y >= source.GetLength(1))
            { 
                throw new ArgumentOutOfRangeException("pos must be a valid pos in source");
            }
            if (size.X <= 0 || size.Y <= 0 || pos.X + size.X > source.GetLength(0) && pos.Y + size.Y > source.GetLength(1))
            {
                throw new ArgumentOutOfRangeException("size must be >0 and fit in source");
            }

            Pixel[,] slice = new Pixel[size.X, size.Y];

            for (var y = 0; y < size.Y; y++)
            {
                for (var x = 0; x < size.X; x++)
                {
                    slice[x, y] = source[x + pos.X, y + pos.Y];
                }
            }
            return slice;
        }

        public static Pixel Lerp(double Delta, Pixel Dest, Pixel Source)
        {
            var P = new Pixel(
                (byte)Utils.Lerp(Delta, Dest.R, Source.R),
                (byte)Utils.Lerp(Delta, Dest.G, Source.G),
                (byte)Utils.Lerp(Delta, Dest.B, Source.B),
                (byte)Utils.Lerp(Delta, Dest.A, Source.A)
            );
            return P;
        }

        //public static Pixel[,] CopyTo(Pixel[,] Source, Pixel[,] Destination)
        //{
        //    for (var y = 0; y < Source.GetLength(1); y++)
        //    {
        //        for (var x = 0; x < Source.GetLength(0); x++)
        //        {
        //            if (x < Destination.GetLength(0) || y < Destination.GetLength(1));
        //            Destination[x, y] = Source[x, y];
        //        }
        //    }
        //    return Destination;
        //}
    }

    public class BooleanConverter<T> : IValueConverter
    {
        public BooleanConverter(T trueValue, T falseValue)
        {
            True = trueValue;
            False = falseValue;
        }

        public T True { get; set; }
        public T False { get; set; }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolean && boolean ? True : False;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is T t && EqualityComparer<T>.Default.Equals(t, True);
        }
    }

    public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public BooleanToVisibilityConverter() :
            base(Visibility.Visible, Visibility.Collapsed)
        { }
    }
}
