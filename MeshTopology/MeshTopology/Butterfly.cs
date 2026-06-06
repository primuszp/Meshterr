using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeshTopology
{
    class Butterfly
    {
        private static double[,] map = null;

        public static double[,] Calculate(double[,] elevations)
        {
            int w = elevations.GetLength(0);
            int h = elevations.GetLength(1);

            map = new double[w + w - 1, h + h - 1];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    map[x + 1, y + 1] = elevations[x, y];
                }
            }

            return (map);
        }
    }
}
