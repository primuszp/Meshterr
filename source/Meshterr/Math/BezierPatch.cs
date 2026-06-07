
namespace Meshterr
{
    class BezierPatch
    {
        #region Private Members

        private float[,] points = new float[4, 4];
        private Vector3d[,] vectors = new Vector3d[4, 4];
        private Vector3d[] tempNormals = new Vector3d[4];

        #endregion

        #region Private Methodes

        private float[] tParam(float t)
        {
            float[] ft = new float[4];

            ft[0] = 1 - 3 * t + 3 * t * t - t * t * t;
            ft[1] = 3 * t - 6 * t * t + 3 * t * t * t;
            ft[2] = 3 * t * t - 3 * t * t * t;
            ft[3] = t * t * t;

            return (ft);
        }

        private float[] diffParam(float t)
        {
            float[] dft = new float[4];

            dft[0] = -3 + 6 * t - 3 * t * t;
            dft[1] = +3 - 12 * t + 9 * t * t;
            dft[2] = +6 * t - 9 * t * t;
            dft[3] = +3 * t * t;

            return (dft);
        }

        private Vector3d diffFu(float u, float v)
        {
            float[]  ft = tParam(v);
            float[] dft = diffParam(u);

            return (dft[0] * vectors[0, 0] + dft[1] * vectors[1, 0] + dft[2] * vectors[2, 0] + dft[3] * vectors[3, 0]) * ft[0] + 
                   (dft[0] * vectors[0, 1] + dft[1] * vectors[1, 1] + dft[2] * vectors[2, 1] + dft[3] * vectors[3, 1]) * ft[1] +
                   (dft[0] * vectors[0, 2] + dft[1] * vectors[1, 2] + dft[2] * vectors[2, 2] + dft[3] * vectors[3, 2]) * ft[2] +
                   (dft[0] * vectors[0, 3] + dft[1] * vectors[1, 3] + dft[2] * vectors[2, 3] + dft[3] * vectors[3, 3]) * ft[3];
        }

        private Vector3d diffFv(float u, float v)
        {
            float[]  ft = tParam(u);
            float[] dft = diffParam(v);

            return (ft[0] * vectors[0, 0] + ft[1] * vectors[1, 0] + ft[2] * vectors[2, 0] + ft[3] * vectors[3, 0]) * dft[0] + 
                   (ft[0] * vectors[0, 1] + ft[1] * vectors[1, 1] + ft[2] * vectors[2, 1] + ft[3] * vectors[3, 1]) * dft[1] + 
                   (ft[0] * vectors[0, 2] + ft[1] * vectors[1, 2] + ft[2] * vectors[2, 2] + ft[3] * vectors[3, 2]) * dft[2] + 
                   (ft[0] * vectors[0, 3] + ft[1] * vectors[1, 3] + ft[2] * vectors[2, 3] + ft[3] * vectors[3, 3]) * dft[3];
        }

        private Vector3d getVector(float u, float v)
        {
            float[] fu = tParam(u);
            float[] fv = tParam(v);

            return (fu[0] * vectors[0, 0] + fu[1] * vectors[1, 0] + fu[2] * vectors[2, 0] + fu[3] * vectors[3, 0]) * fv[0] +
                   (fu[0] * vectors[0, 1] + fu[1] * vectors[1, 1] + fu[2] * vectors[2, 1] + fu[3] * vectors[3, 1]) * fv[1] + 
                   (fu[0] * vectors[0, 2] + fu[1] * vectors[1, 2] + fu[2] * vectors[2, 2] + fu[3] * vectors[3, 2]) * fv[2] + 
                   (fu[0] * vectors[0, 3] + fu[1] * vectors[1, 3] + fu[2] * vectors[2, 3] + fu[3] * vectors[3, 3]) * fv[3];
        }

        private float getElevation(float u, float v)
        {
            float[] fu = tParam(u);
            float[] fv = tParam(v);

            return (fu[0] * points[0, 0] + fu[1] * points[1, 0] + fu[2] * points[2, 0] + fu[3] * points[3, 0]) * fv[0] +
                   (fu[0] * points[0, 1] + fu[1] * points[1, 1] + fu[2] * points[2, 1] + fu[3] * points[3, 1]) * fv[1] +
                   (fu[0] * points[0, 2] + fu[1] * points[1, 2] + fu[2] * points[2, 2] + fu[3] * points[3, 2]) * fv[2] +
                   (fu[0] * points[0, 3] + fu[1] * points[1, 3] + fu[2] * points[2, 3] + fu[3] * points[3, 3]) * fv[3];
        }

        private Vector3d getNormal(float u, float v)
        {
            return Vector3d.Normalize(diffFu(u, v) % diffFv(u, v));
        }

        private Vector3d GetControlPointUU(Vector3d p0, Vector3d p1, Vector3d n0)
        {
            double cPx = ((p1.X - p0.X) / 3.0) + p0.X;
            double cPy = ((p1.Y - p0.Y) / 3.0) + p0.Y;
            double cPz = (((p1.X - p0.X) * (-n0.X / n0.Z)) / 3.0) + p0.Z;

            return (new Vector3d(cPx, cPy, cPz));
        }

        private Vector3d GetControlPointVV(Vector3d p0, Vector3d p1, Vector3d n0)
        {
            double cPx = ((p1.X - p0.X) / 3.0) + p0.X;
            double cPy = ((p1.Y - p0.Y) / 3.0) + p0.Y;
            double cPz = (((p1.Y - p0.Y) * (-n0.Y / n0.Z)) / 3.0) + p0.Z;

            return (new Vector3d(cPx, cPy, cPz));
        }

        private Vector3d GetControlPointUV(Vector3d p00, Vector3d p10, Vector3d p01, Vector3d n00)
        {
            double cPx = (p10.X - p00.X) + p00.X;
            double cPy = (p01.Y - p00.Y) + p00.Y;
            double cPz = (((p10.X - p00.X) * (-n00.X / n00.Z) + (p01.Y - p00.Y) * (-n00.Y / n00.Z))) + p00.Z;

            return (new Vector3d(cPx, cPy, cPz));
        }

        private float GetKnotUU(float height, float xDimension, Vector3d normal)
        {
            return ((float)(height + ((xDimension * (-normal.X / normal.Z) / 3.0))));
        }

        private float GetKnotVV(float height, float yDimension, Vector3d normal)
        {
            return ((float)(height + ((yDimension * (-normal.Y / normal.Z) / 3.0))));
        }

        private float GetKnotUV(float height, float xDimension, float yDimension, Vector3d normal)
        {
            return ((float)(height + (((xDimension / 3.0) * (-normal.X / normal.Z) + (yDimension / 3.0) * (-normal.Y / normal.Z)))));
        }

        private void ElevationPoints(int xlt, int ylt, Rsg dem)
        {
            float xDimension = dem.XDimension;
            float yDimension = dem.YDimension;

            points[0, 0] = dem.ZData[xlt, ylt + 1];
            points[3, 0] = dem.ZData[xlt + 1, ylt + 1];
            points[0, 3] = dem.ZData[xlt, ylt];
            points[3, 3] = dem.ZData[xlt + 1, ylt];

            points[1, 0] = GetKnotUU(points[0, 0], +xDimension, dem.NData[xlt, ylt + 1]);
            points[2, 0] = GetKnotUU(points[3, 0], -xDimension, dem.NData[xlt + 1, ylt + 1]);

            points[0, 1] = GetKnotVV(points[0, 0], -yDimension, dem.NData[xlt, ylt + 1]);
            points[3, 1] = GetKnotVV(points[3, 0], -yDimension, dem.NData[xlt + 1, ylt + 1]);

            points[0, 2] = GetKnotVV(points[0, 3], +yDimension, dem.NData[xlt, ylt]);
            points[3, 2] = GetKnotVV(points[3, 3], +yDimension, dem.NData[xlt + 1, ylt]);

            points[1, 3] = GetKnotUU(points[0, 3], +xDimension, dem.NData[xlt, ylt]);
            points[2, 3] = GetKnotUU(points[3, 3], -xDimension, dem.NData[xlt + 1, ylt]);

            points[1, 1] = GetKnotUV(points[0, 0], +xDimension, -yDimension, dem.NData[xlt, ylt + 1]);
            points[2, 1] = GetKnotUV(points[3, 0], -xDimension, -yDimension, dem.NData[xlt + 1, ylt + 1]);
            points[1, 2] = GetKnotUV(points[0, 3], +xDimension, +yDimension, dem.NData[xlt, ylt]);
            points[2, 2] = GetKnotUV(points[3, 3], -xDimension, +yDimension, dem.NData[xlt + 1, ylt]);
        }

        private void VectorPoints(int xlt, int ylt, Rsg dem)
         {
             float xDimension = dem.XDimension;
             float yDimension = dem.YDimension;

             tempNormals[0] = dem.NData[xlt, ylt + 1];
             vectors[0, 0] = new Vector3d(xDimension * xlt, yDimension * (ylt + 1), dem.ZData[xlt, ylt + 1]);

             tempNormals[1] = dem.NData[xlt + 1, ylt + 1];
             vectors[3, 0] = new Vector3d(xDimension * (xlt + 1), yDimension * (ylt + 1), dem.ZData[xlt + 1, ylt + 1]);

             tempNormals[2] = dem.NData[xlt, ylt];
             vectors[0, 3] = new Vector3d(xDimension * xlt, yDimension * ylt, dem.ZData[xlt, ylt]);

             tempNormals[3] = dem.NData[xlt + 1, ylt];
             vectors[3, 3] = new Vector3d(xDimension * (xlt + 1), yDimension * ylt, dem.ZData[xlt + 1, ylt]);

             vectors[1, 0] = GetControlPointUU(vectors[0, 0], vectors[3, 0], tempNormals[0]);
             vectors[2, 0] = GetControlPointUU(vectors[3, 0], vectors[0, 0], tempNormals[1]);
             vectors[0, 1] = GetControlPointVV(vectors[0, 0], vectors[0, 3], tempNormals[0]);
             vectors[3, 1] = GetControlPointVV(vectors[3, 0], vectors[3, 3], tempNormals[1]);
             vectors[0, 2] = GetControlPointVV(vectors[0, 3], vectors[0, 0], tempNormals[2]);
             vectors[3, 2] = GetControlPointVV(vectors[3, 3], vectors[3, 0], tempNormals[3]);
             vectors[1, 3] = GetControlPointUU(vectors[0, 3], vectors[3, 3], tempNormals[2]);
             vectors[2, 3] = GetControlPointUU(vectors[3, 3], vectors[0, 3], tempNormals[3]);
             vectors[1, 1] = GetControlPointUV(vectors[0, 0], vectors[1, 0], vectors[0, 1], tempNormals[0]);
             vectors[2, 1] = GetControlPointUV(vectors[3, 0], vectors[2, 0], vectors[3, 1], tempNormals[1]);
             vectors[1, 2] = GetControlPointUV(vectors[0, 3], vectors[1, 3], vectors[0, 2], tempNormals[2]);
             vectors[2, 2] = GetControlPointUV(vectors[3, 3], vectors[2, 3], vectors[3, 2], tempNormals[3]);
         }

        #endregion

        public Vector3d[,] BezierQuadv(Rsg dem)
        {
            int newWidth  = 2 * dem.NrOfCellsPerLine - 1;
            int newHeight = 2 * dem.NrOfLines - 1;

            Vector3d[,] data = new Vector3d[newWidth, newHeight];

            for (int y = 0; y < dem.NrOfLines - 1; y++)
            {
                for (int x = 0; x < dem.NrOfCellsPerLine - 1; x++)
                {
                    VectorPoints(x, y, dem);

                    data[2 * x + 0, 2 * y + 0] = getVector(0.0f, 0.0f);
                    data[2 * x + 0, 2 * y + 1] = getVector(0.0f, 0.5f);
                    data[2 * x + 1, 2 * y + 1] = getVector(0.5f, 0.5f);
                    data[2 * x + 2, 2 * y + 1] = getVector(1.0f, 0.5f);
                    data[2 * x + 1, 2 * y + 0] = getVector(0.5f, 1.0f);
                    data[2 * x + 1, 2 * y + 2] = getVector(0.5f, 0.0f);
                }
            }

            // A folt-iteráció csak [0..W-2, 0..H-2]-t fed le; a jobb és alsó szél
            // eredeti csúcsait külön kell bemásolni, különben nullán maradnak.
            int lastX = dem.NrOfCellsPerLine - 1;
            int lastY = dem.NrOfLines - 1;
            for (int y = 0; y < dem.NrOfLines; y++)
                data[2 * lastX, 2 * y] = new Vector3d(dem.XDimension * lastX, dem.YDimension * y, dem.ZData[lastX, y]);
            for (int x = 0; x < dem.NrOfCellsPerLine - 1; x++)
                data[2 * x, 2 * lastY] = new Vector3d(dem.XDimension * x, dem.YDimension * lastY, dem.ZData[x, lastY]);

            return (data);
        }

        public float[,] BezierQuadf(Rsg dem)
        {
            int newWidth  = 2 * dem.NrOfCellsPerLine - 1;
            int newHeight = 2 * dem.NrOfLines - 1;

            float[,] data = new float[newWidth, newHeight];

            for (int y = 0; y < dem.NrOfLines - 1; y++)
            {
                for (int x = 0; x < dem.NrOfCellsPerLine - 1; x++)
                {
                    ElevationPoints(x, y, dem);

                    data[2 * x + 0, 2 * y + 0] = dem.ZData[x, y];
                    data[2 * x + 0, 2 * y + 1] = getElevation(0.0f, 0.5f);
                    data[2 * x + 1, 2 * y + 1] = getElevation(0.5f, 0.5f);
                    data[2 * x + 2, 2 * y + 1] = getElevation(1.0f, 0.5f);
                    data[2 * x + 1, 2 * y + 0] = getElevation(0.5f, 1.0f);
                    data[2 * x + 1, 2 * y + 2] = getElevation(0.5f, 0.0f);
                }
            }

            // A jobb szél és alsó szél eredeti magasság-csúcsai kimaradnának,
            // mert a ciklus csak [0..W-2, 0..H-2] foltokat fed le.
            int lastX = dem.NrOfCellsPerLine - 1;
            int lastY = dem.NrOfLines - 1;
            for (int y = 0; y < dem.NrOfLines; y++)
                data[2 * lastX, 2 * y] = dem.ZData[lastX, y];
            for (int x = 0; x < dem.NrOfCellsPerLine - 1; x++)
                data[2 * x, 2 * lastY] = dem.ZData[x, lastY];

            return (data);
        }

        public Rsg GetBezierRsg(Rsg dem, int lod)
        {
            Rsg rsg = dem;

            if (!dem.Normalized)
            {
                dem.ComputeSurfaceNormals();
            }

            for (int i = 0; i < lod; i++)
            {
                rsg.ZData = BezierQuadf(rsg);
                rsg.XDimension = rsg.XDimension / 2;
                rsg.YDimension = rsg.YDimension / 2;
                rsg.ComputeSurfaceNormals();
            }

            return (rsg);
        }
    }
}