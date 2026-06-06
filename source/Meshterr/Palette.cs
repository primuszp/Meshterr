using System;
using System.Drawing;
using System.IO;

namespace Meshterr
{
    public class Palette
    {
        private Color[] palette = null;

        public Palette()
        {
            palette = new Color[256];
        }

        public Palette(Bitmap bmp)
        {
            palette = new Color[256];

            if (bmp.Width == 256)
            {
                for (int i = 0; i < 256; i++)
                {
                    palette[i] = bmp.GetPixel(i, bmp.Height / 2);
                }
            }
            else
            {
                if (bmp.Height == 256)
                {
                    for (int i = 0; i < 256; i++)
                    {
                        palette[i] = bmp.GetPixel(bmp.Width / 2, i);
                    }
                }
            }
        }

        public Bitmap PaletteToBitmap()
        {
            Bitmap bmp = new Bitmap(16, 256);

            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    bmp.SetPixel(x, y, palette[255 - y]);
                }
            }
            return (bmp);
        }

        public bool ReadPaletteFromTxt(string path)
        {
            using (StreamReader myFile = File.OpenText(path))
            {
                int count = 0;

                do
                {
                    string[] buffer = myFile.ReadLine().Split(new char[] { '\t' });

                    if (count > 0)
                    {
                        palette[count - 1] = Color.FromArgb(Convert.ToInt32(buffer[0]), Convert.ToInt32(buffer[1]), Convert.ToInt32(buffer[2]));
                    }

                    count++;

                } while (!myFile.EndOfStream);
            }
            return (true);
        }

        public bool ReadPaletteFromBmp(string path)
        {
            using (Bitmap bmp = new Bitmap(path))
            {
                if (bmp.Width == 16 && bmp.Height == 256)
                {
                    for (int i = 0; i < bmp.Height; i++)
                    {
                        palette[255 - i] = bmp.GetPixel(8, i);
                    }
                    return (true);
                }
                else
                {
                    return (false);
                }
            }
        }

        public Color InterColor(float elevation)
        {
            if (float.IsNaN(elevation) || float.IsInfinity(elevation))
            {
                elevation = 0f;
            }
            else if (elevation < 0f)
            {
                elevation = 0f;
            }
            else if (elevation > 1f)
            {
                elevation = 1f;
            }

            float z = elevation * 255;
            byte pos = (byte)Math.Floor(z);

            if (pos >= 0 && pos < 255)
            {
                return (calcColor(tCalc(z, pos, pos + 1), palette[pos], palette[pos + 1]));
            }
            else
            {
                return (palette[255]);
            }
        }

        private Color calcColor(double t, Color c0, Color c1)
        {
            double t0 = 1.0 - t;

            int R = Convert.ToInt32(t0 * c0.R + t * c1.R);
            int G = Convert.ToInt32(t0 * c0.G + t * c1.G);
            int B = Convert.ToInt32(t0 * c0.B + t * c1.B);

            return (Color.FromArgb(R, G, B));
        }

        private double tCalc(double z, double z0, double z1)
        {
            return ((z - z0) / (-z0 + z1));
        }
    }
}
