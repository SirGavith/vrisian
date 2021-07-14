using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace vrisian
{
    public class ByteImage
    {
        public Pixel[,] Source { get; }
        private PixelFormat format = new PixelFormat();


        public int Width { get { return Source.GetLength(0); } }
        public int Height { get { return Source.GetLength(1); } }

        public PixelFormat Format { get { return format; }}
        public int BytesPerPixel { get { return format.BitsPerPixel / 8;  } }
        public int Stride { get { return Width * BytesPerPixel; } }

        public ByteImage(string path)
        {
            Stream imageStreamSource = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapSource bmp = decoder.Frames[0];

            int width = bmp.PixelWidth;
            int height = bmp.PixelHeight;
            Source = new Pixel[width, height];
            format = bmp.Format;

            var BytesPerPixel = format.BitsPerPixel / 8;

            byte[] FlattenedSource = new byte[Stride * height];
            bmp.CopyPixels(FlattenedSource, Stride, 0);

            for (int i = 0; i < FlattenedSource.Length / BytesPerPixel; i++)
            {
                byte[] b = new byte[4] { 0, 0, 0, 255 };
                Array.ConstrainedCopy(FlattenedSource, i * BytesPerPixel, b, 0, BytesPerPixel);

                int x = i % width;
                int y = (int) Math.Floor(i / width + 0d);

                Source[x, y] = new Pixel(b[2], b[1], b[0], b[3]);
            }
        }

        public ByteImage(Pixel[,] source, object f = null)
        {
            Source = source;
            if (f == null)
            {
                format = PixelFormats.Pbgra32;
            } else
            {
                format = (PixelFormat) f;
            }
        }

        public void Save()
        {
            //using (var fileStream = File.Open(OpenFile.FullPath, FileMode.Create))
            //{
            //    //save file

            //}
        }

        public void SetPixel(int x, int y, Pixel color)
        {
            Source[x, y] = color;
        }

        public void Scale(int scale)
        {
            //foreach (byte row in Source)
            //{

            //}
        }

        public byte[] ToFlattened()
        {
            byte[] flattened = new byte[Source.Length * BytesPerPixel];
            int i = 0;
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    Array.ConstrainedCopy(Source[x, y].ToByteArray(BytesPerPixel == 4), 0, flattened, i, BytesPerPixel);
                    i += BytesPerPixel;
                }
            }

            return flattened;
        }

        public BitmapSource ToBitmap()
        {
            var rgb = new BitmapPalette(new List<Color> { Colors.Red, Colors.Green, Colors.Blue });
            return BitmapSource.Create(Height, Width, 300, 300, Format, rgb, ToFlattened(), Stride);
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

        public byte[] ToByteArray(Boolean IncludeAlpha)
        {
            byte[] output;
            
            if (IncludeAlpha)
            {
                output = new byte[4];
                output[3] = A;
            } else
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
