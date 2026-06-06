using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace MeshTopology
{
    class Palette
    {
        private Color[] colors = new Color[256];

        public Color interColor(float elevation)
        {
            float z = elevation * 255;
            byte pos = (byte)z;

            if (pos >= 0 && pos < 255)
            {
                return (calcColor(tCalc(z, pos, pos + 1), colors[pos], colors[pos + 1]));
            }
            else
            {
                return (colors[255]);
            }
        }

        private double tCalc(double z, double z0, double z1)
        {
            double t = (z - z0) / (-z0 + z1);

            if (t >= 0 && t <= 1)
            {
                return (t);
            }
            if (t < 0)
                return 0;
            else
                return 1;
        }

        private Color calcColor(double t, Color c0, Color c1)
        {
            int R = Convert.ToInt32((1.0 - t) * c0.R + t * c1.R);
            int G = Convert.ToInt32((1.0 - t) * c0.G + t * c1.G);
            int B = Convert.ToInt32((1.0 - t) * c0.B + t * c1.B);

            return (Color.FromArgb(R, G, B));
        }

        public bool ReadPalette(string path)
        {
            StreamReader myFile = File.OpenText(path);
            int count = 0;

            do
            {
                string[] buffer = myFile.ReadLine().Split(new char[] { '\t' });

                if (count > 0)
                {
                    colors[count - 1] = Color.FromArgb(Convert.ToInt32(buffer[0]), Convert.ToInt32(buffer[1]), Convert.ToInt32(buffer[2]));
                }
                count++;

            } while (!myFile.EndOfStream);
            myFile.Close();

            return (true);
        }
    }
}
