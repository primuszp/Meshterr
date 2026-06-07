using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Meshterr
{
    /// <summary>
    /// Regular Square Grid (RSG)
    /// </summary>
    public class Rsg : DisplayList
    {
        #region Private Members

        private enum CellType
        {
            Unsigned8BitInteger,
            Unsigned16BitInteger,
            Unsigned32BitInteger,
            Signed8BitInteger,
            Signed16BitInteger,
            Signed32BitInteger,
            IEEE4ByteReal,
            IEEE8ByteReal,
            None
        };

        private BitmapData bd;
        private IntPtr ptr_bd;
        private Bitmap bitmap;

        private int nrOfLines = 0;
        private int nrOfCellsPerLine = 0;
        private float xDimension = 1;
        private float yDimension = 1;
        private float eastings = 0;
        private float northings = 0;
        private BoundingBox box;

        private float[,] zData = null;
        private float[,] normalizedImageData = null;
        private Vector3d[,] nData = null;
        private float minElevation;
        private float maxElevation;
        private int distortion = 1;
        private float nodataValue = float.NaN;

        private bool normalized = false;

        #endregion

        #region Public Properties

        public bool Normalized
        {
            get { return normalized; }
            set { normalized = value; }
        }

        public int Distoration
        {
            get { return distortion; }
            set { distortion = value; }
        }

        public int NrOfLines
        {
            get { return nrOfLines; }
        }

        public int NrOfCellsPerLine
        {
            get { return nrOfCellsPerLine; }
        }

        public float XDimension
        {
            get { return xDimension; }
            set { xDimension = value; }
        }

        public float YDimension
        {
            get { return yDimension; }
            set { yDimension = value; }
        }

        public float Eastings
        {
            get { return eastings; }
        }

        public float Northings
        {
            get { return northings; }
        }

        public float MinElevation
        {
            get { return minElevation; }
        }

        public float MaxElevation
        {
            get { return maxElevation; }
        }

        public float[,] ZData
        {
            get { return zData; }
            set 
            { 
                zData = value;
                nrOfCellsPerLine = zData.GetLength(0);
                nrOfLines = zData.GetLength(1);
                if (normalizedImageData != null &&
                    (normalizedImageData.GetLength(0) != nrOfCellsPerLine || normalizedImageData.GetLength(1) != nrOfLines))
                {
                    normalizedImageData = null;
                }
                UpdateGeometry();
            }
        }

        public Vector3d[,] NData
        {
            get { return nData; }
            set { nData = value; }
        }

        public BoundingBox Box
        {
            get { return box; }
        }

        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
        public double OffsetZ { get; set; }

        public Palette RsgPalette { get; set; }
        public bool IsImageBased { get; private set; }

        #endregion;

        #region Constructors

        public Rsg()
        {
            minElevation = float.MaxValue;
            maxElevation = float.MinValue;
        }

        public Rsg(Palette pal)
        {
            RsgPalette = pal;

            minElevation = float.MaxValue;
            maxElevation = float.MinValue;
        }

        public Rsg(Palette pal, Bitmap dem)
        {
            RsgPalette = pal;

            bitmap = CreateWorkingBitmap(dem);

            BmpInfo(bitmap);

            minElevation = float.MaxValue;
            maxElevation = float.MinValue;

            RsgFromBmp();
        }

        #endregion

        #region Rsg and Bmp Conversions

        private void BmpInfo(Bitmap dem)
        {
            xDimension = 1;
            yDimension = 1;
            eastings = 0.5f;
            northings = dem.Height - 0.5f;
            nrOfLines = dem.Height;
            nrOfCellsPerLine = dem.Width;
            UpdateGeometry();
        }

        private void RsgFromBmp()
        {
            BitmapLock();

            float elevation = 0.0f;

            zData = new float[nrOfCellsPerLine, nrOfLines];

            for (int y = 0; y < nrOfLines; y++)
            {
                for (int x = 0; x < nrOfCellsPerLine; x++)
                {
                    elevation = GetPixelElevation(x, y);

                    if (minElevation > elevation)
                    {
                        minElevation = elevation;
                    }
                    if (maxElevation < elevation)
                    {
                        maxElevation = elevation;
                    }

                    zData[x, y] = elevation;
                }
            }

            float elevationRange = maxElevation - minElevation;
            if (elevationRange <= 0f)
            {
                elevationRange = 1f;
            }

            for (int y = 0; y < nrOfLines; y++)
            {
                for (int x = 0; x < nrOfCellsPerLine; x++)
                {
                    ZData[x, y] -= minElevation;
                    ZData[x, y] /= elevationRange;
                }
            }

            minElevation = 0;
            maxElevation = 1;
            normalizedImageData = CopyData(zData);
            IsImageBased = true;
            UpdateGeometry();

            BitmapUnlock();
        }

        private static float[,] CopyData(float[,] source)
        {
            int width = source.GetLength(0);
            int height = source.GetLength(1);
            float[,] copy = new float[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    copy[x, y] = source[x, y];
                }
            }

            return copy;
        }

        private void UpdateGeometry()
        {
            double width = Math.Max(0, nrOfCellsPerLine - 1) * xDimension;
            double height = Math.Max(0, nrOfLines - 1) * yDimension;

            box = new BoundingBox(0, 0, width, height);
            OffsetX = width / 2.0;
            OffsetY = height / 2.0;
            OffsetZ = minElevation + ((maxElevation - minElevation) / 2.0f);
        }

        public void ApplyImageScale(float pixelWidth, float pixelHeight, float elevationMin, float elevationMax)
        {
            if (!IsImageBased || normalizedImageData == null)
            {
                return;
            }

            if (pixelWidth <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(pixelWidth), "A pixel X méretnek pozitívnak kell lennie.");
            }

            if (pixelHeight <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(pixelHeight), "A pixel Y méretnek pozitívnak kell lennie.");
            }

            xDimension = pixelWidth;
            yDimension = pixelHeight;
            minElevation = Math.Min(elevationMin, elevationMax);
            maxElevation = Math.Max(elevationMin, elevationMax);

            float range = maxElevation - minElevation;
            for (int y = 0; y < nrOfLines; y++)
            {
                for (int x = 0; x < nrOfCellsPerLine; x++)
                {
                    zData[x, y] = minElevation + (normalizedImageData[x, y] * range);
                }
            }

            normalized = false;
            UpdateGeometry();
        }

        private static Bitmap CreateWorkingBitmap(Bitmap source)
        {
            Bitmap target = new Bitmap(source.Width, source.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(target))
            {
                graphics.DrawImage(source, 0, 0, source.Width, source.Height);
            }

            source.Dispose();
            return target;
        }

        public Bitmap BmpFromRsg()
        {
            BitmapLock();

            for (int y = 0; y < nrOfLines; y++)
            {
                for (int x = 0; x < nrOfCellsPerLine; x++)
                {
                    SetPixel(x, y, RsgPalette.InterColor(NormalizedElevation(x, y)));
                }
            }

            BitmapUnlock();

            return (bitmap);
        }

        private float NormalizedElevation(int x, int y)
        {
            if (IsImageBased &&
                normalizedImageData != null &&
                x >= 0 &&
                y >= 0 &&
                x < normalizedImageData.GetLength(0) &&
                y < normalizedImageData.GetLength(1))
            {
                return normalizedImageData[x, y];
            }

            float range = maxElevation - minElevation;
            if (range <= 0f)
            {
                return 0f;
            }

            return (zData[x, y] - minElevation) / range;
        }

        // Nodata cella esetén minElevation-t ad vissza, hogy a geometria ne törjön el
        private float SafeZ(int x, int y)
        {
            float v = zData[x, y];
            return float.IsNaN(v) ? minElevation : v;
        }

        #endregion

        #region ERS Loader

        private CellType ErsCellType(string type)
        {
            if (type == "Unsigned8BitInteger")
            {
                return CellType.Unsigned8BitInteger;
            }
            else
                if (type == "Unsigned16BitInteger")
                {
                    return CellType.Unsigned16BitInteger;
                }
                else
                    if (type == "Unsigned32BitInteger")
                    {
                        return CellType.Unsigned32BitInteger;
                    }
                    else
                        if (type == "Signed8BitInteger")
                        {
                            return CellType.Signed8BitInteger;
                        }
                        else
                            if (type == "Signed16BitInteger")
                            {
                                return CellType.Signed16BitInteger;
                            }
                            else
                                if (type == "Signed32BitInteger")
                                {
                                    return CellType.Signed32BitInteger;
                                }
                                else
                                    if (type == "IEEE4ByteReal")
                                    {
                                        return CellType.IEEE4ByteReal;
                                    }
                                    else
                                        if (type == "IEEE8ByteReal")
                                        {
                                            return CellType.IEEE8ByteReal;
                                        }
                                        else
                                        {
                                            return CellType.None;
                                        }
        }

        public bool LoadErsMap(string path)
        {
            CellType cellType = CellType.None;

            using (StreamReader myFile = File.OpenText(path))
            {
                do
                {
                    string buffer = myFile.ReadLine();

                    if (buffer == null) continue;

                    if (buffer.Contains("CellType"))
                    {
                        string[] temp = buffer.Split(new char[] { '=' });
                        cellType = ErsCellType(temp[1].Trim());
                    }
                    else
                        if (buffer.Contains("Xdimension"))
                        {
                            string[] temp = buffer.Split(new char[] { '=' });
                            xDimension = Convert.ToInt32(temp[1].Trim());
                        }
                        else
                            if (buffer.Contains("Ydimension"))
                            {
                                string[] temp = buffer.Split(new char[] { '=' });
                                yDimension = Convert.ToInt32(temp[1].Trim());
                            }
                            else
                                if (buffer.Contains("NrOfLines"))
                                {
                                    string[] temp = buffer.Split(new char[] { '=' });
                                    nrOfLines = Convert.ToInt32(temp[1].Trim());
                                }
                                else
                                    if (buffer.Contains("NrOfCellsPerLine"))
                                    {
                                        string[] temp = buffer.Split(new char[] { '=' });
                                        nrOfCellsPerLine = Convert.ToInt32(temp[1].Trim());
                                    }
                                    else
                                        if (buffer.Contains("Eastings"))
                                        {
                                            string[] temp = buffer.Split(new char[] { '=' },StringSplitOptions.RemoveEmptyEntries);
                                            eastings = Convert.ToSingle(temp[1].Trim());
                                        }
                                        else
                                            if (buffer.Contains("Northings"))
                                            {
                                                string[] temp = buffer.Split(new char[] { '=' });
                                                northings = Convert.ToSingle(temp[1].Trim());
                                            }
                                            else
                                                if (buffer.Contains("NullCellValue"))
                                                {
                                                    string[] temp = buffer.Split(new char[] { '=' });
                                                    if (float.TryParse(temp[1].Trim(), System.Globalization.NumberStyles.Float,
                                                        System.Globalization.CultureInfo.InvariantCulture, out float ndv))
                                                    {
                                                        nodataValue = ndv;
                                                    }
                                                }
                } while (!myFile.EndOfStream);
            }

            if (File.Exists(path.Substring(0, path.Length - 3) + "bnd"))
            {
                using (StreamReader myFile = File.OpenText(path.Substring(0, path.Length - 3) + "bnd"))
                {
                    string[] temp = myFile.ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < temp.Length; i++)
                    {
                        temp[i] = temp[i].Substring(2).Trim();
                    }

                    float x1 = Convert.ToSingle(temp[0]);
                    float y1 = Convert.ToSingle(temp[1]);
                    float x2 = Convert.ToSingle(temp[2]);
                    float y2 = Convert.ToSingle(temp[3]);

                    box = new BoundingBox(x1, y1, x2 - x1, y2 - y1);

                    OffsetX = (int)((x2 - x1) / 2);
                    OffsetY = (int)((y2 - y1) / 2);
                }
            }
            else
            {
                float deltaX = nrOfCellsPerLine * xDimension;
                float deltaY = nrOfLines * yDimension;

                float x1 = eastings - xDimension / 2;
                float y1 = northings + (yDimension / 2) - deltaY;
                float x2 = x1 + deltaX;
                float y2 = northings + yDimension / 2;

                box = new BoundingBox(x1, y1, x2 - x1, y2 - y1);

                OffsetX = (int)((x2 - x1) / 2);
                OffsetY = (int)((y2 - y1) / 2);
            }

            using (FileStream fileStream = new FileStream(path.Substring(0, path.Length - 4), FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    zData = new float[nrOfCellsPerLine, nrOfLines];
                    float elevation = 0.0f;

                    for (int y = 0; y < nrOfLines; y++)
                    {
                        for (int x = 0; x < nrOfCellsPerLine; x++)
                        {
                            switch (cellType)
                            {
                                case CellType.Unsigned8BitInteger:
                                    elevation = (float)binaryReader.ReadByte();
                                    break;
                                case CellType.Unsigned16BitInteger:
                                    elevation = (float)binaryReader.ReadUInt16();
                                    break;
                                case CellType.Unsigned32BitInteger:
                                    elevation = (float)binaryReader.ReadUInt32();
                                    break;
                                case CellType.Signed8BitInteger:
                                    elevation = (float)binaryReader.ReadSByte();
                                    break;
                                case CellType.Signed16BitInteger:
                                    elevation = (float)binaryReader.ReadInt16();
                                    break;
                                case CellType.Signed32BitInteger:
                                    elevation = (float)binaryReader.ReadInt32();
                                    break;
                                case CellType.IEEE4ByteReal:
                                    elevation = (float)binaryReader.ReadSingle();
                                    break;
                                case CellType.IEEE8ByteReal:
                                    elevation = (float)binaryReader.ReadDouble();
                                    break;
                                default:
                                    elevation = (float)binaryReader.ReadDecimal();
                                    break;
                            }

                            bool isNodata = !float.IsNaN(nodataValue) && elevation == nodataValue;

                            if (!isNodata)
                            {
                                if (minElevation > elevation) minElevation = elevation;
                                if (maxElevation < elevation) maxElevation = elevation;
                            }

                            zData[x, y] = isNodata ? float.NaN : elevation;
                        }
                    }
                }
            }
            bitmap = new Bitmap(NrOfCellsPerLine, NrOfLines, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            UpdateGeometry();
            return (true);
        }

        #endregion

        #region Unsafe Bitmap Methodes

        private void BitmapLock()
        {
            bd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            ptr_bd = bd.Scan0;
        }

        private void BitmapUnlock()
        {
            bitmap.UnlockBits(bd);
        }

        private void SetPixel(int x, int y, Color color)
        {
            SetPixelRGB(ptr_bd, bd.Stride, x, y, color.ToArgb());
        }

        private Color GetPixel(int x, int y)
        {
            return (Color.FromArgb(GetPixelRGB(ptr_bd, bd.Stride, x, y)));
        }

        private float GetPixelElevation(int x, int y)
        {
            Color color = GetPixel(x, y);
            return (0.299f * color.R) + (0.587f * color.G) + (0.114f * color.B);
        }

        unsafe private int GetPixelRGB(IntPtr ip, int Stride, int x, int y)
        {
            int* Pixels = (int*)ip;
            return (Pixels[x + y * Stride / 4]);
        }

        unsafe private void SetPixelRGB(IntPtr ip, int Stride, int x, int y, int color)
        {
            int* Pixels = (int*)ip;
            Pixels[x + y * Stride / 4] = color;
        }

        #endregion

        #region Private Methodes

        private DataField Slope(Vector3d[,] normals)
        {
            float val = 0;
            float min = float.MaxValue;
            float max = float.MinValue;
            float[,] data = new float[NrOfCellsPerLine, NrOfLines];

            double Zdx = 0;
            double Zdy = 0;

            for (int y = 0; y < NrOfLines - 1; y++)
            {
                for (int x = 0; x < NrOfCellsPerLine - 1; x++)
                {
                    double z = normals[x, y].Z;

                    if (z != 0)
                    {
                        Zdx = -normals[x, y].X / nData[x, y].Z;
                        Zdy = -normals[x, y].Y / nData[x, y].Z;

                        val = (float)(360d / (2d * Math.PI) * Math.Atan(Math.Sqrt(Zdx * Zdx + Zdy * Zdy)));

                        if (min > val)
                        {
                            min = val;
                        }
                        if (max < val)
                        {
                            max = val;
                        }

                        data[x, y] = val;
                    }
                    else data[x, y] = 1;
                }
            }
            return (new DataField(data, min, max));
        }

        private void RenderVertices(float size)
        {
            if (zData != null)
            {
                GL.PointSize(size);
                GL.Color3(0.3f, 0.9f, 0.0f);

                for (int y = 0; y < NrOfLines - 1; y++)
                {
                    GL.Begin(BeginMode.Points);

                    for (int x = 0; x < NrOfCellsPerLine - 1; x++)
                    {
                        GL.Vertex3(x * XDimension - OffsetX, y * YDimension - OffsetY, (SafeZ(x, y) - OffsetZ) * Distoration);
                        GL.Vertex3(x * XDimension - OffsetX, (y + 1) * YDimension - OffsetY, (SafeZ(x, y + 1) - OffsetZ) * Distoration);
                        GL.Vertex3((x + 1) * XDimension - OffsetX, y * YDimension - OffsetY, (SafeZ(x + 1, y) - OffsetZ) * Distoration);
                        GL.Vertex3((x + 1) * XDimension - OffsetX, (y + 1) * YDimension - OffsetY, (SafeZ(x + 1, y + 1) - OffsetZ) * Distoration);
                    }

                    GL.End();
                }
            }
            else
            {
                throw new ArgumentNullException("Z Data member is null.");
            }
        }

        private void RenderFaces()
        {
            if (zData != null)
            {
                if (!Normalized)
                {
                    Normalized = ComputeSurfaceNormals();
                }

                DataField df = Slope(nData);

                GL.Color3(0.3f, 0.9f, 0.0f);

                for (int y = 0; y < NrOfLines - 1; y++)
                {
                    GL.Begin(BeginMode.TriangleStrip);

                    for (int x = 0; x < NrOfCellsPerLine - 1; x++)
                    {
                        GL.Color3(RsgPalette.InterColor(NormalizedElevation(x, y)));
                        //GL.Color3(RsgPalette.InterColor(df.Normal(x,y)));
                        GL.Normal3(nData[x, y].X, nData[x, y].Y, nData[x, y].Z);
                        GL.Vertex3(x * XDimension - OffsetX, y * YDimension - OffsetY, (SafeZ(x, y) - OffsetZ) * Distoration);

                        GL.Color3(RsgPalette.InterColor(NormalizedElevation(x, y + 1)));
                        //GL.Color3(RsgPalette.InterColor(df.Normal(x, y+1)));
                        GL.Normal3(nData[x, y + 1].X, nData[x, y + 1].Y, nData[x, y + 1].Z);
                        GL.Vertex3(x * XDimension - OffsetX, (y + 1) * YDimension - OffsetY, (SafeZ(x, y + 1) - OffsetZ) * Distoration);

                        GL.Color3(RsgPalette.InterColor(NormalizedElevation(x + 1, y)));
                        //GL.Color3(RsgPalette.InterColor(df.Normal(x+1, y)));
                        GL.Normal3(nData[x + 1, y].X, nData[x + 1, y].Y, nData[x + 1, y].Z);
                        GL.Vertex3((x + 1) * XDimension - OffsetX, y * YDimension - OffsetY, (SafeZ(x + 1, y) - OffsetZ) * Distoration);

                        GL.Color3(RsgPalette.InterColor(NormalizedElevation(x + 1, y + 1)));
                        //GL.Color3(RsgPalette.InterColor(df.Normal(x+1, y+1)));
                        GL.Normal3(nData[x + 1, y + 1].X, nData[x + 1, y + 1].Y, nData[x + 1, y + 1].Z);
                        GL.Vertex3((x + 1) * XDimension - OffsetX, (y + 1) * YDimension - OffsetY, (SafeZ(x + 1, y + 1) - OffsetZ) * Distoration);
                    }

                    GL.End();
                }
            }
            else
            {
                throw new ArgumentNullException("Z Data member is null.");
            }
        }

        private void RenderRibbon()
        {
            double eBottom;
            double elevationRange = maxElevation - minElevation;

            eBottom = (minElevation - Math.Max(1.0, elevationRange * 0.2) - OffsetZ) * Distoration;

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Color3(Color.Chocolate);

            // Bal fal (x=0), normál: -X irány
            GL.Begin(BeginMode.TriangleStrip);
            GL.Normal3(-1.0, 0.0, 0.0);
            double left = 0 - OffsetX;
            for (int y = 0; y < NrOfLines; y++)
            {
                double z0 = float.IsNaN(zData[0, y]) ? minElevation : zData[0, y];
                GL.Vertex3(left, y * YDimension - OffsetY, eBottom);
                GL.Vertex3(left, y * YDimension - OffsetY, (z0 - OffsetZ) * Distoration);
            }
            GL.End();

            // Jobb fal (x=max), normál: +X irány
            GL.Begin(BeginMode.TriangleStrip);
            GL.Normal3(1.0, 0.0, 0.0);
            left = (NrOfCellsPerLine - 1) * XDimension - OffsetX;
            for (int y = 0; y < NrOfLines; y++)
            {
                double z0 = float.IsNaN(zData[NrOfCellsPerLine - 1, y]) ? minElevation : zData[NrOfCellsPerLine - 1, y];
                GL.Vertex3(left, y * YDimension - OffsetY, eBottom);
                GL.Vertex3(left, y * YDimension - OffsetY, (z0 - OffsetZ) * Distoration);
            }
            GL.End();

            // Első fal (y=0), normál: -Y irány
            GL.Begin(BeginMode.TriangleStrip);
            GL.Normal3(0.0, -1.0, 0.0);
            double front = 0 - OffsetY;
            for (int x = 0; x < NrOfCellsPerLine; x++)
            {
                double z0 = float.IsNaN(zData[x, 0]) ? minElevation : zData[x, 0];
                GL.Vertex3(x * XDimension - OffsetX, front, eBottom);
                GL.Vertex3(x * XDimension - OffsetX, front, (z0 - OffsetZ) * Distoration);
            }
            GL.End();

            // Hátsó fal (y=max), normál: +Y irány
            GL.Begin(BeginMode.TriangleStrip);
            GL.Normal3(0.0, 1.0, 0.0);
            double back = (NrOfLines - 1) * YDimension - OffsetY;
            for (int x = 0; x < NrOfCellsPerLine; x++)
            {
                double z0 = float.IsNaN(zData[x, NrOfLines - 1]) ? minElevation : zData[x, NrOfLines - 1];
                GL.Vertex3(x * XDimension - OffsetX, back, eBottom);
                GL.Vertex3(x * XDimension - OffsetX, back, (z0 - OffsetZ) * Distoration);
            }
            GL.End();

            //A terepmodell alsó lapja, normál: -Z irány
            GL.Color3(Color.Brown);
            GL.Normal3(0.0, 0.0, -1.0);
            GL.Begin(BeginMode.Triangles);

            GL.Vertex3(0 - OffsetX, 0 - OffsetY, eBottom);
            GL.Vertex3((NrOfCellsPerLine - 1) * XDimension - OffsetX, 0 - OffsetY, eBottom);
            GL.Vertex3(0 - OffsetX, (nrOfLines - 1) * YDimension - OffsetY, eBottom);

            GL.Vertex3((NrOfCellsPerLine - 1) * XDimension - OffsetX, 0 - OffsetY, eBottom);
            GL.Vertex3(0 - OffsetX, (nrOfLines - 1) * YDimension - OffsetY, eBottom);
            GL.Vertex3((NrOfCellsPerLine - 1) * XDimension - OffsetX, (NrOfLines - 1) * YDimension - OffsetY, eBottom);

            GL.End();
        }

        #endregion

        #region Public Methodes

        public bool ComputeSurfaceNormals()
        {
            double Zdx = 0;
            double Zdy = 0;
            double Rdx = 2 * XDimension;
            double Rdy = 2 * YDimension;

            nData = new Vector3d[NrOfCellsPerLine, NrOfLines];

            try
            {
                for (int y = 1; y < NrOfLines - 1; y++)
                {
                    for (int x = 1; x < NrOfCellsPerLine - 1; x++)
                    {
                        Zdx = ((SafeZ(x + 1, y - 1) - SafeZ(x - 1, y - 1)) / Rdx) +
                              ((SafeZ(x + 1, y + 0) - SafeZ(x - 1, y + 0)) / Rdx) +
                              ((SafeZ(x + 1, y + 1) - SafeZ(x - 1, y + 1)) / Rdx);

                        Zdy = ((SafeZ(x + 1, y + 1) - SafeZ(x + 1, y - 1)) / Rdy) +
                              ((SafeZ(x + 0, y + 1) - SafeZ(x + 0, y - 1)) / Rdy) +
                              ((SafeZ(x - 1, y + 1) - SafeZ(x - 1, y - 1)) / Rdy);

                        nData[x, y] = new Vector3d(-Zdx / 3.0, -Zdy / 3.0, 1.0);
                    }
                }

                for (int y = 1; y < NrOfLines - 1; y++)
                {
                    nData[0, y] = nData[1, y];
                    nData[NrOfCellsPerLine - 1, y] = nData[NrOfCellsPerLine - 2, y];
                }

                for (int x = 1; x < NrOfCellsPerLine - 1; x++)
                {
                    nData[x, 0] = nData[x, 1];
                    nData[x, NrOfLines - 1] = nData[x, NrOfLines - 2];
                }

                nData[0, 0] = (nData[1, 0] + nData[0, 1] + nData[1, 1]) / 3.0;
                nData[NrOfCellsPerLine - 1, 0] = (nData[NrOfCellsPerLine - 2, 0] + nData[NrOfCellsPerLine - 1, 1] + nData[NrOfCellsPerLine - 2, 1]) / 3.0;
                nData[0, NrOfLines - 1] = (nData[1, NrOfLines - 1] + nData[0, NrOfLines - 2] + nData[1, NrOfLines - 2]) / 3.0;
                nData[NrOfCellsPerLine - 1, NrOfLines - 1] = (nData[NrOfCellsPerLine - 2, NrOfLines - 1] + nData[NrOfCellsPerLine - 1, NrOfLines - 2] + nData[NrOfCellsPerLine - 2, NrOfLines - 2]) / 3.0;

                return (true);
            }
            catch
            {
                return (false);
            }
        }

        /// <summary>
        /// Virtual method to Regenerate the display lists based on the parameters for solid fill and edge drawing 
        /// </summary>
        /// <param name="renderingOptions">Rendering options to impose on the regenerated display list objects.</param>
        /// <exception cref="ArgumentNullException">Mesh member is null.</exception>
        public override void Regenerate(RenderingType RenderingOption)
        {
            if (IsList())
            {
                Delete();
            }

            NewList(ListMode.Compile);

            switch (RenderingOption)
            {
                case RenderingType.Vertex:
                    {
                        GL.Enable(EnableCap.PolygonOffsetFill);
                        GL.PolygonOffset(1f, 1f);
                        RenderVertices(2);
                        RenderRibbon();
                        GL.Disable(EnableCap.PolygonOffsetFill);
                        break;
                    }
                case RenderingType.Wireframe:
                    {
                        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                        GL.Enable(EnableCap.PolygonOffsetFill);
                        GL.PolygonOffset(1f, 1f);
                        RenderFaces();
                        RenderRibbon();
                        GL.Disable(EnableCap.PolygonOffsetFill);
                        break;
                    }
                case RenderingType.Solid:
                    {
                        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                        GL.Enable(EnableCap.PolygonOffsetFill);
                        GL.PolygonOffset(1f, 1f);
                        RenderFaces();
                        RenderRibbon();
                        GL.Disable(EnableCap.PolygonOffsetFill);
                        break;
                    }
                case RenderingType.Shaded:
                    {
                        float[] ambientColor = { 0.4f, 0.4f, 0.4f, 1.0f };
                        float[] lightColor0 = { 0.6f, 0.6f, 0.6f, 1.0f };
                        float[] lightPos0 = { -0.5f, 0.8f, 0.1f, 0.0f };

                        GL.Enable(EnableCap.ColorMaterial);
                        GL.Enable(EnableCap.Lighting);
                        GL.Enable(EnableCap.Light0);
                        GL.ShadeModel(ShadingModel.Smooth);

                        GL.LightModel(LightModelParameter.LightModelAmbient, ambientColor);
                        GL.Light(LightName.Light0, LightParameter.Diffuse, lightColor0);
                        GL.Light(LightName.Light0, LightParameter.Position, lightPos0);

                        GL.Enable(EnableCap.PolygonOffsetFill);
                        GL.PolygonOffset(1f, 1f);
                        RenderFaces();
                        RenderRibbon();
                        GL.Disable(EnableCap.PolygonOffsetFill);
                        break;
                    }
                default:
                    break;
            }

            EndList();
        }

        #endregion
    }
}
