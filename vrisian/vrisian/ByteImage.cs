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


            Source = new Pixel[bmp.PixelWidth, bmp.PixelHeight];
            format = bmp.Format;


            byte[] FlattenedSource = new byte[Stride * Height];
            bmp.CopyPixels(FlattenedSource, Stride, 0);

            for (int i = 0; i < FlattenedSource.Length / BytesPerPixel; i++)
            {
                byte[] b = new byte[4] { 0, 0, 0, 255 };
                Array.ConstrainedCopy(FlattenedSource, i * BytesPerPixel, b, 0, BytesPerPixel);

                int x = i % Width;
                int y = (int) Math.Floor(i / Width + 0d);

                Source[x, y] = new Pixel(b[2], b[1], b[0], b[3]);
            }
        }

        public ByteImage(Pixel[,] source, object pixelformat = null)
        {
            Source = new Pixel[source.GetLength(0), source.GetLength(1)];
            Source = source;
            format = pixelformat == null ? PixelFormats.Pbgra32 : (PixelFormat) pixelformat;
        }

        public ByteImage(int width, int height, object pixelformat = null)
        {
            Source = new Pixel[width, height];
            format = pixelformat == null ? PixelFormats.Pbgra32 : (PixelFormat) pixelformat;
        }

        public void Save()
        {
            //using (var fileStream = File.Open(OpenFile.FullPath, FileMode.Create))
            //{
            //    //save file

            //}
        }

        public bool SetPixel(int x, int y, Pixel color)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                Source[x, y] = color;
                return true;
            }
            return false;
        }

        public bool SetPixel(XY xy, Pixel color)
        {
            return SetPixel(xy.X, xy.Y, color);
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

        public void Blit(ByteImage img, int offsetX = 0, int offsetY = 0)
        {
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    if (img.Source[x, y].A != 0)
                    {
                        Source[x + offsetX, y + offsetY] = img.Source[x, y];
                    }
                }
            }

        }

        public void ForEachPixel(Action<int, int> func)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    func(x, y);
                }
            }
        }

        public ByteImage Copy()
        {
            return new ByteImage(Source, Format);
        }

        public BitmapSource ToBitmap()
        {
            var rgb = new BitmapPalette(new List<Color> { Colors.Red, Colors.Green, Colors.Blue });
            return BitmapSource.Create(Width, Height, 300d, 300d, Format, rgb, ToFlattened(), Stride);
        }
    }

    
}
