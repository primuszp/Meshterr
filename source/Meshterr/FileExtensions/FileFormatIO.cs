using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Meshterr
{
    static class FileFormatIO
    {
        public static BoundingBox Box { get; set; }
        public static double MaxElevation { get; set; }
        public static double MinElevation { get; set; }

        public static List<Node> LoadXYZ(string filename)
        {
            StreamReader myFile = File.OpenText(filename);
            List<Node> nodes = new List<Node>();

            double xmin = double.MaxValue;
            double ymin = double.MaxValue;
            double zmin = double.MaxValue;
            double xmax = double.MinValue;
            double ymax = double.MinValue;
            double zmax = double.MinValue;

            try
            {
                do
                {
                    string[] buffer = myFile.ReadLine().Split(new char[] { ';', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    Vector3d vertex = new Vector3d(ConvertToDouble(buffer[0]), ConvertToDouble(buffer[1]), ConvertToDouble(buffer[2]));

                    if (vertex.X < xmin) xmin = vertex.X;
                    if (vertex.X > xmax) xmax = vertex.X;
                    if (vertex.Y < ymin) ymin = vertex.Y;
                    if (vertex.Y > ymax) ymax = vertex.Y;
                    if (vertex.Z < zmin) zmin = vertex.Z;
                    if (vertex.Z > zmax) zmax = vertex.Z;

                    nodes.Add(new Node(vertex));
                }
                while (!myFile.EndOfStream);
                Box = new BoundingBox(xmin, ymax, xmax - xmin, ymax - ymin);
                MaxElevation = zmax;
                MinElevation = zmin;
                myFile.Close();
                return (nodes);
            }
            catch
            {
                return (nodes);
            }
        }

        private static double ConvertToDouble(string number)
        {
            return (double.Parse(number, CultureInfo.InvariantCulture));
        }
    }
}
