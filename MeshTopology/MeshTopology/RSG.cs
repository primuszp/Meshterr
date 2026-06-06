using System;
using System.Collections.Generic;
using System.IO;
using GRV11;
using System.Drawing;

namespace MeshTopology
{
    class RSG
    {
        public int NrOfLines;
        public int NrOfCellsPerLine;
        public int Xdimension = 1;
        public int Ydimension = 1;

        public float[,] zData;
        public float minElevation = 0f;
        public float maxElevation = 0f;

        public Vector[,] normals = null;

        float[] Vertices = new float[] { 0.0f, 1.0f, -10.0f, -1.0f, -1.0f, -10.0f, 1.0f, -1.0f, -10.0f };
        float[] Colors = new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f };

        public enum Direction : byte { Boundary, Right, Left, In, Out }

        GR gr = null;

        public void Terep(GR gr)
        {
            LoadErsMap("T1");
            //LoadHeightMap("sthelens257.bmp");

            computeNormals();
            this.gr = gr;
            DrawColor2(gr);
        }

        public void DrawList(GR gr)
        {
            gr.glCallList(1);
            //rmo.Draw3(gr);
        }

        public void Draw2(GR gr)
        {
            Bezier bezier = new Bezier();

            float[] ambientColor = { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] lightColor0 = { 0.6f, 0.6f, 0.6f, 1.0f };
            float[] lightPos0 = { -0.5f, 0.8f, 0.1f, 0.0f };

            gr.glNewList(1, GR.GL_COMPILE);

            //gr.glPolygonMode(GR.GL_FRONT_AND_BACK, GR.GL_LINE);

            gr.glLightModelfv(GR.GL_LIGHT_MODEL_AMBIENT, ambientColor);
            gr.glLightfv(GR.GL_LIGHT0, GR.GL_DIFFUSE, lightColor0);
            gr.glLightfv(GR.GL_LIGHT0, GR.GL_POSITION, lightPos0);
            gr.glColor3f(0.3f, 0.9f, 0.0f);

            List<Vector> points = new List<Vector>();
            List<Vector> points2 = new List<Vector>();

            for (int x = 0; x < NrOfLines - 1; x++)
            {
                for (int y = 0; y < NrOfCellsPerLine - 1; y++)
                {
                    bezier.P1 = new Vector(x * Xdimension, y * Ydimension, zData[x, y]);
                    bezier.P2 = new Vector(x * Xdimension, (y + 1) * Ydimension, zData[x, y + 1]);
                    bezier.P3 = new Vector((x + 1) * Xdimension, y * Ydimension, zData[x + 1, y]);
                    bezier.N1 = getNormal(x, y);
                    bezier.N2 = getNormal(x, y + 1);
                    bezier.N3 = getNormal(x + 1, y);
                    bezier.PointCalculation();

                    gr.glBegin(GR.GL_TRIANGLES);
                    gr.glColor3f(0.3f, 0.9f, 0.0f);
                    for (int i = 0; i < bezier.Vertices.Count; i = i + 3)
                    {
                        points.InsertRange(points.Count, Szintvonal(bezier.Vertices[i], bezier.Vertices[i + 1], bezier.Vertices[i + 2], 20));

                        gr.glNormal3f((float)bezier.Normals[i].X, (float)bezier.Normals[i].Y, (float)bezier.Normals[i].Z);
                        gr.glVertex3f((float)bezier.Vertices[i].X, (float)bezier.Vertices[i].Y, (float)bezier.Vertices[i].Z);

                        gr.glNormal3f((float)bezier.Normals[i + 1].X, (float)bezier.Normals[i + 1].Y, (float)bezier.Normals[i + 1].Z);
                        gr.glVertex3f((float)bezier.Vertices[i + 1].X, (float)bezier.Vertices[i + 1].Y, (float)bezier.Vertices[i + 1].Z);

                        gr.glNormal3f((float)bezier.Normals[i + 2].X, (float)bezier.Normals[i + 2].Y, (float)bezier.Normals[i + 2].Z);
                        gr.glVertex3f((float)bezier.Vertices[i + 2].X, (float)bezier.Vertices[i + 2].Y, (float)bezier.Vertices[i + 2].Z);
                    }

                    bezier.P1 = new Vector((x + 1) * Xdimension, y * Ydimension, zData[x + 1, y]);
                    bezier.P2 = new Vector(x * Xdimension, (y + 1) * Ydimension, zData[x, y + 1]);
                    bezier.P3 = new Vector((x + 1) * Xdimension, (y + 1) * Ydimension, zData[x + 1, y + 1]);
                    bezier.N1 = getNormal(x + 1, y);
                    bezier.N2 = getNormal(x, y + 1);
                    bezier.N3 = getNormal(x + 1, y + 1);
                    bezier.PointCalculation();

                    for (int i = 0; i < bezier.Vertices.Count; i = i + 3)
                    {
                        points2.InsertRange(points2.Count, Szintvonal(bezier.Vertices[i], bezier.Vertices[i + 1], bezier.Vertices[i + 2], 20));

                        gr.glNormal3f((float)bezier.Normals[i].X, (float)bezier.Normals[i].Y, (float)bezier.Normals[i].Z);
                        gr.glVertex3f((float)bezier.Vertices[i].X, (float)bezier.Vertices[i].Y, (float)bezier.Vertices[i].Z);

                        gr.glNormal3f((float)bezier.Normals[i + 1].X, (float)bezier.Normals[i + 1].Y, (float)bezier.Normals[i + 1].Z);
                        gr.glVertex3f((float)bezier.Vertices[i + 1].X, (float)bezier.Vertices[i + 1].Y, (float)bezier.Vertices[i + 1].Z);

                        gr.glNormal3f((float)bezier.Normals[i + 2].X, (float)bezier.Normals[i + 2].Y, (float)bezier.Normals[i + 2].Z);
                        gr.glVertex3f((float)bezier.Vertices[i + 2].X, (float)bezier.Vertices[i + 2].Y, (float)bezier.Vertices[i + 2].Z);
                    }
                    gr.glEnd();

                    gr.glDisable(GR.GL_DEPTH_TEST);
                    gr.glLineWidth(2);
                    gr.glBegin(GR.GL_LINES);
                    gr.glColor3f(0.7f, 0.9f, 0.5f);

                    for (int j = 0; j < points.Count; j++)
                    {
                        gr.glVertex3d(points[j].X, points[j].Y, points[j].Z);
                    }
                    for (int k = 0; k < points2.Count; k++)
                    {
                        gr.glVertex3d(points2[k].X, points2[k].Y, points2[k].Z);
                    }
                    gr.glEnd();
                    points.Clear();
                    points2.Clear();
                    gr.glEnable(GR.GL_DEPTH_TEST);
                }
            }
            gr.glEndList();
        }

        public void Draw1(GR gr)
        {
            Vector normal = null;

            float[] ambientColor = { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] lightColor0 = { 0.6f, 0.6f, 0.6f, 1.0f };
            float[] lightPos0 = { -0.5f, 0.8f, 0.1f, 0.0f };

            gr.glNewList(1, GR.GL_COMPILE);
            //gr.glPolygonMode(GR.GL_FRONT_AND_BACK, GR.GL_LINE);
            gr.glLightModelfv(GR.GL_LIGHT_MODEL_AMBIENT, ambientColor);
            gr.glLightfv(GR.GL_LIGHT0, GR.GL_DIFFUSE, lightColor0);
            gr.glLightfv(GR.GL_LIGHT0, GR.GL_POSITION, lightPos0);

            //float scale = 5.0f / Math.Max(width - 1, height - 1);
            //gr.glScalef(scale, scale, scale);
            //gr.glTranslatef(-(float)(width - 1) / 2, 0.0f, -(float)(height - 1) / 2);


            //gr.glPolygonMode(GR.GL_FRONT_AND_BACK, GR.GL_LINE);

            gr.glColor3f(0.3f, 0.9f, 0.0f);
            for (int x = 0; x < NrOfLines - 1; x++)
            {
                gr.glBegin(GR.GL_TRIANGLE_STRIP);

                for (int y = 0; y < NrOfCellsPerLine - 1; y++)
                {
                    normal = getNormal(x, y);
                    gr.glNormal3d(normal[0], normal[1], normal[2]);
                    gr.glVertex3f(x * Xdimension, y * Ydimension, zData[x, y]);

                    normal = getNormal(x, y + 1);
                    gr.glNormal3d(normal[0], normal[1], normal[2]);
                    gr.glVertex3f(x * Xdimension, (y + 1) * Ydimension, zData[x, y + 1]);

                    normal = getNormal(x + 1, y);
                    gr.glNormal3d(normal[0], normal[1], normal[2]);
                    gr.glVertex3f((x + 1) * Xdimension, y * Ydimension, zData[x + 1, y]);

                    normal = getNormal(x + 1, y + 1);
                    gr.glNormal3d(normal[0], normal[1], normal[2]);
                    gr.glVertex3f((x + 1) * Xdimension, (y + 1) * Ydimension, zData[x + 1, y + 1]);
                }
                gr.glEnd();
            }


            //for (int x = 0; x < NrOfLines - 1; x++)
            //{
            //    for (int y = 0; y < NrOfCellsPerLine - 1; y++)
            //    {
            //        List<Vector> points = Szintvonal(new Vector(x, y, zData[x, y]), new Vector(x, y + 1, zData[x, y + 1]), new Vector(x + 1, y, zData[x + 1, y]), 10);

            //        gr.glDisable(GR.GL_DEPTH_TEST);
            //        gr.glColor3f(0.6f, 0.8f, 0.4f);
            //        gr.glLineWidth(2);
            //        gr.glBegin(GR.GL_LINES);

            //        for (int i = 0; i < points.Count; i++)
            //        {
            //            gr.glVertex3d(points[i].X * Xdimension, points[i].Y * Ydimension, points[i].Z);
            //        }

            //        gr.glEnd();

            //        List<Vector> points2 = Szintvonal(new Vector(x, y + 1, zData[x, y + 1]), new Vector(x + 1, y, zData[x + 1, y]), new Vector(x + 1, y + 1, zData[x + 1, y + 1]), 10);
            //        gr.glBegin(GR.GL_LINES);
            //        for (int i = 0; i < points2.Count; i++)
            //        {
            //            gr.glVertex3d(points2[i].X * Xdimension, points2[i].Y * Ydimension, points2[i].Z);
            //        }
            //        gr.glEnd();
            //        gr.glEnable(GR.GL_DEPTH_TEST);
            //    }
            //}

            //gr.glEndList();

            for (int x = 0; x < NrOfLines - 1; x++)
            {
                for (int y = 0; y < NrOfCellsPerLine - 1; y++)
                {
                    gr.glBegin(GR.GL_LINES);
                    gr.glColor3d(0, 1, 1);

                    gr.glVertex3d(x * 100, y * 100, zData[x, y]);
                    gr.glVertex3d(x * 100 + normals[x, y].X, y * 100 + normals[x, y].Y, zData[x, y] + normals[x, y].Z * 50);

                    gr.glEnd();
                }
            }
            gr.glEndList();
        }

        public void DrawColor2(GR gr)
        {
            Bezier bezier = new Bezier();
            Color color;
            Palette pal = new Palette();
            pal.ReadPalette("foldtenger.pal");

            float[] ambientColor = { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] lightColor0 = { 0.6f, 0.6f, 0.6f, 1.0f };
            float[] lightPos0 = { -0.5f, 0.8f, 0.1f, 0.0f };

            gr.glNewList(1, GR.GL_COMPILE);

            //gr.glPolygonMode(GR.GL_FRONT_AND_BACK, GR.GL_LINE);

            gr.glLightModelfv(GR.GL_LIGHT_MODEL_AMBIENT, ambientColor);
            gr.glLightfv(GR.GL_LIGHT0, GR.GL_DIFFUSE, lightColor0);
            gr.glLightfv(GR.GL_LIGHT0, GR.GL_POSITION, lightPos0);
            gr.glColor3f(0.3f, 0.9f, 0.0f);

            List<Vector> points = new List<Vector>();
            List<Vector> points2 = new List<Vector>();

            for (int x = 0; x < NrOfLines - 1; x++)
            {
                for (int y = 0; y < NrOfCellsPerLine - 1; y++)
                {
                    bezier.P1 = new Vector(x * Xdimension, y * Ydimension, zData[x, y]);
                    bezier.P2 = new Vector(x * Xdimension, (y + 1) * Ydimension, zData[x, y + 1]);
                    bezier.P3 = new Vector((x + 1) * Xdimension, y * Ydimension, zData[x + 1, y]);
                    bezier.N1 = getNormal(x, y);
                    bezier.N2 = getNormal(x, y + 1);
                    bezier.N3 = getNormal(x + 1, y);
                    bezier.PointCalculation();

                    gr.glBegin(GR.GL_TRIANGLES);
                    //gr.glColor3f(0.3f, 0.9f, 0.0f);
                    for (int i = 0; i < bezier.Vertices.Count; i = i + 3)
                    {
                        points.InsertRange(points.Count, Szintvonal(bezier.Vertices[i], bezier.Vertices[i + 1], bezier.Vertices[i + 2], 200));

                        color = pal.interColor(nElevation((float)bezier.Vertices[i].Z));
                        gr.glColor3ub(color.R, color.G, color.B);
                        gr.glNormal3f((float)bezier.Normals[i].X, (float)bezier.Normals[i].Y, (float)bezier.Normals[i].Z);
                        gr.glVertex3f((float)bezier.Vertices[i].X, (float)bezier.Vertices[i].Y, (float)bezier.Vertices[i].Z);

                        color = pal.interColor(nElevation((float)bezier.Vertices[i + 1].Z));
                        gr.glColor3ub(color.R, color.G, color.B);
                        gr.glNormal3f((float)bezier.Normals[i + 1].X, (float)bezier.Normals[i + 1].Y, (float)bezier.Normals[i + 1].Z);
                        gr.glVertex3f((float)bezier.Vertices[i + 1].X, (float)bezier.Vertices[i + 1].Y, (float)bezier.Vertices[i + 1].Z);

                        color = pal.interColor(nElevation((float)bezier.Vertices[i + 2].Z));
                        gr.glColor3ub(color.R, color.G, color.B);
                        gr.glNormal3f((float)bezier.Normals[i + 2].X, (float)bezier.Normals[i + 2].Y, (float)bezier.Normals[i + 2].Z);
                        gr.glVertex3f((float)bezier.Vertices[i + 2].X, (float)bezier.Vertices[i + 2].Y, (float)bezier.Vertices[i + 2].Z);
                    }

                    bezier.P1 = new Vector((x + 1) * Xdimension, y * Ydimension, zData[x + 1, y]);
                    bezier.P2 = new Vector(x * Xdimension, (y + 1) * Ydimension, zData[x, y + 1]);
                    bezier.P3 = new Vector((x + 1) * Xdimension, (y + 1) * Ydimension, zData[x + 1, y + 1]);
                    bezier.N1 = getNormal(x + 1, y);
                    bezier.N2 = getNormal(x, y + 1);
                    bezier.N3 = getNormal(x + 1, y + 1);
                    bezier.PointCalculation();

                    for (int i = 0; i < bezier.Vertices.Count; i = i + 3)
                    {
                        points2.InsertRange(points2.Count, Szintvonal(bezier.Vertices[i], bezier.Vertices[i + 1], bezier.Vertices[i + 2], 200));

                        color = pal.interColor(nElevation((float)bezier.Vertices[i].Z));
                        gr.glColor3ub(color.R, color.G, color.B);
                        gr.glNormal3f((float)bezier.Normals[i].X, (float)bezier.Normals[i].Y, (float)bezier.Normals[i].Z);
                        gr.glVertex3f((float)bezier.Vertices[i].X, (float)bezier.Vertices[i].Y, (float)bezier.Vertices[i].Z);

                        color = pal.interColor(nElevation((float)bezier.Vertices[i + 1].Z));
                        gr.glColor3ub(color.R, color.G, color.B);
                        gr.glNormal3f((float)bezier.Normals[i + 1].X, (float)bezier.Normals[i + 1].Y, (float)bezier.Normals[i + 1].Z);
                        gr.glVertex3f((float)bezier.Vertices[i + 1].X, (float)bezier.Vertices[i + 1].Y, (float)bezier.Vertices[i + 1].Z);

                        color = pal.interColor(nElevation((float)bezier.Vertices[i + 2].Z));
                        gr.glColor3ub(color.R, color.G, color.B);
                        gr.glNormal3f((float)bezier.Normals[i + 2].X, (float)bezier.Normals[i + 2].Y, (float)bezier.Normals[i + 2].Z);
                        gr.glVertex3f((float)bezier.Vertices[i + 2].X, (float)bezier.Vertices[i + 2].Y, (float)bezier.Vertices[i + 2].Z);
                    }
                    gr.glEnd();

                    gr.glLineWidth(2);
                    gr.glBegin(GR.GL_LINES);
                    gr.glColor3f(0.7f, 0.9f, 0.5f);

                    for (int j = 0; j < points.Count; j++)
                    {
                        gr.glVertex3d(points[j].X, points[j].Y, points[j].Z);
                    }
                    for (int k = 0; k < points2.Count; k++)
                    {
                        gr.glVertex3d(points2[k].X, points2[k].Y, points2[k].Z);
                    }
                    gr.glEnd();
                    points.Clear();
                    points2.Clear();
                }
            }

            //gr.glEnable(GR.GL_DEPTH_TEST);
            gr.glEndList();
        }

        public void DrawColor(GR gr)
        {
            Vector normal = null;
            Color color;
            Palette pal = new Palette();
            pal.ReadPalette("foldtenger.pal");

            float[] ambientColor = { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] lightColor0 = { 0.6f, 0.6f, 0.6f, 1.0f };
            float[] lightPos0 = { -0.5f, 0.8f, 0.1f, 0.0f };

            gr.glNewList(1, GR.GL_COMPILE);
            //gr.glPolygonMode(GR.GL_FRONT_AND_BACK, GR.GL_LINE);
            gr.glLightModelfv(GR.GL_LIGHT_MODEL_AMBIENT, ambientColor);
            gr.glLightfv(GR.GL_LIGHT0, GR.GL_DIFFUSE, lightColor0);
            gr.glLightfv(GR.GL_LIGHT0, GR.GL_POSITION, lightPos0);


            //gr.glColor3f(0.3f, 0.9f, 0.0f);
            for (int x = 0; x < NrOfLines - 1; x++)
            {
                gr.glBegin(GR.GL_TRIANGLE_STRIP);

                for (int y = 0; y < NrOfCellsPerLine - 1; y++)
                {
                    color = pal.interColor(nElevation(zData[x, y]));
                    gr.glColor3ub(color.R, color.G, color.B);
                    normal = getNormal(x, y);
                    gr.glNormal3d(normal[0], normal[1], normal[2]);
                    gr.glVertex3f(x * Xdimension, y * Ydimension, zData[x, y]);

                    color = pal.interColor(nElevation(zData[x, y + 1]));
                    gr.glColor3ub(color.R, color.G, color.B);
                    normal = getNormal(x, y + 1);
                    gr.glNormal3d(normal[0], normal[1], normal[2]);
                    gr.glVertex3f(x * Xdimension, (y + 1) * Ydimension, zData[x, y + 1]);

                    color = pal.interColor(nElevation(zData[x + 1, y]));
                    gr.glColor3ub(color.R, color.G, color.B);
                    normal = getNormal(x + 1, y);
                    gr.glNormal3d(normal[0], normal[1], normal[2]);
                    gr.glVertex3f((x + 1) * Xdimension, y * Ydimension, zData[x + 1, y]);

                    color = pal.interColor(nElevation(zData[x + 1, y + 1]));
                    gr.glColor3ub(color.R, color.G, color.B);
                    normal = getNormal(x + 1, y + 1);
                    gr.glNormal3d(normal[0], normal[1], normal[2]);
                    gr.glVertex3f((x + 1) * Xdimension, (y + 1) * Ydimension, zData[x + 1, y + 1]);
                }
                gr.glEnd();
            }


            //for (int x = 0; x < NrOfLines - 1; x++)
            //{
            //    for (int y = 0; y < NrOfCellsPerLine - 1; y++)
            //    {
            //        List<Vector> points = Szintvonal(new Vector(x, y, zData[x, y]), new Vector(x, y + 1, zData[x, y + 1]), new Vector(x + 1, y, zData[x + 1, y]), 10);

            //        gr.glDisable(GR.GL_DEPTH_TEST);
            //        gr.glColor3f(0.6f, 0.8f, 0.4f);
            //        gr.glLineWidth(2);
            //        gr.glBegin(GR.GL_LINES);

            //        for (int i = 0; i < points.Count; i++)
            //        {
            //            gr.glVertex3d(points[i].X * Xdimension, points[i].Y * Ydimension, points[i].Z);
            //        }

            //        gr.glEnd();

            //        List<Vector> points2 = Szintvonal(new Vector(x, y + 1, zData[x, y + 1]), new Vector(x + 1, y, zData[x + 1, y]), new Vector(x + 1, y + 1, zData[x + 1, y + 1]), 10);
            //        gr.glBegin(GR.GL_LINES);
            //        for (int i = 0; i < points2.Count; i++)
            //        {
            //            gr.glVertex3d(points2[i].X * Xdimension, points2[i].Y * Ydimension, points2[i].Z);
            //        }
            //        gr.glEnd();
            //        gr.glEnable(GR.GL_DEPTH_TEST);
            //    }
            //}

            gr.glEndList();
        }


        public float[,] LoadBitmap(Bitmap DEM)
        {
            zData = new float[DEM.Width, DEM.Height];

			for(int x = 0; x < DEM.Width; x++)
			{
				for(int y = 0; y < DEM.Height; y++)
				{
					Color p = DEM.GetPixel(x,y);
					float v = (float)p.R / 255f;	// Scale Red to range 0..1
                    zData[DEM.Width - 1 - x, DEM.Height - 1 - y] = v;

                    if (v < minElevation)
                    {
                        minElevation = v;
                    }
                    if (v > maxElevation)
                    {
                        maxElevation = v;
                    }

				}
			}
			DEM.Dispose();
            return (zData);
        }

        public float[,] LoadHeightMap(string filnename)
        {
            int offset = 0;
            byte dummy = 0;

            FileStream fileStream = new FileStream(filnename, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);

            for (int i = 0; i < 10; i++)
            {
                dummy = binaryReader.ReadByte();
            }

            offset = binaryReader.ReadByte();
            offset += binaryReader.ReadByte() * 256;
            offset += binaryReader.ReadByte() * 256 * 256;
            offset += binaryReader.ReadByte() * 256 * 256 * 256;

            for (int i = 0; i < 4; i++)
            {
                dummy = binaryReader.ReadByte();
            }

            NrOfCellsPerLine = binaryReader.ReadByte();
            NrOfCellsPerLine += binaryReader.ReadByte() * 256;
            NrOfCellsPerLine += binaryReader.ReadByte() * 256 * 256;
            NrOfCellsPerLine += binaryReader.ReadByte() * 256 * 256 * 256;

            NrOfLines = binaryReader.ReadByte();
            NrOfLines += binaryReader.ReadByte() * 256;
            NrOfLines += binaryReader.ReadByte() * 256 * 256;
            NrOfLines += binaryReader.ReadByte() * 256 * 256 * 256;

            zData = new float[NrOfLines, NrOfCellsPerLine];
            float elevation = 1.0f;

            for (int i = 0; i < (offset - 26); i++)
            {
                dummy = binaryReader.ReadByte();
            }

            for (int x = 0; x < NrOfLines; x++)
            {
                for (int y = 0; y < NrOfCellsPerLine; y++)
                {
                    elevation = binaryReader.ReadByte() + binaryReader.ReadByte() + binaryReader.ReadByte();

                    zData[NrOfLines - 1 - x, NrOfCellsPerLine - 1 - y] = elevation;

                    if (elevation < minElevation)
                    {
                        minElevation = elevation;
                    }
                    if (elevation > maxElevation)
                    {
                        maxElevation = elevation;
                    }
                }
            }

            Xdimension = 100;
            Ydimension = 100;

            fileStream.Close();
            binaryReader.Close();
            return (zData);
        }

        public float[,] LoadErsMap(string filename)
        {
            StreamReader myFile = File.OpenText(filename + ".ers");

            do
            {
                string buffer = myFile.ReadLine();

                if (buffer.Contains("Xdimension"))
                {
                    string[] temp = buffer.Split(new char[] { '=' });
                    Xdimension = Convert.ToInt16(temp[1]);
                }

                if (buffer.Contains("Ydimension"))
                {
                    string[] temp = buffer.Split(new char[] { '=' });
                    Ydimension = Convert.ToInt16(temp[1]);
                }

                if (buffer.Contains("NrOfLines"))
                {
                    string[] temp = buffer.Split(new char[] { '=' });
                    NrOfLines = Convert.ToInt16(temp[1]);
                }

                if (buffer.Contains("NrOfCellsPerLine"))
                {
                    string[] temp = buffer.Split(new char[] { '=' });
                    NrOfCellsPerLine = Convert.ToInt16(temp[1]);
                }

            } while (!myFile.EndOfStream);

            myFile.Close();

            FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);

            zData = new float[NrOfLines, NrOfCellsPerLine];
            float elevation = 1.0f;

            for (int x = 0; x < NrOfLines; x++)
            {
                for (int y = 0; y < NrOfCellsPerLine; y++)
                {
                    elevation = (binaryReader.ReadSingle()) * 3;
                    zData[NrOfLines - 1 - x, NrOfCellsPerLine - 1 - y] = elevation;

                    if (elevation < minElevation)
                    {
                        minElevation = elevation;
                    }
                    if (elevation > maxElevation)
                    {
                        maxElevation = elevation;
                    }
                }
            }
            fileStream.Close();
            binaryReader.Close();
            return (zData);
        }

        public void computeNormals()
        {
            normals = new Vector[NrOfLines, NrOfCellsPerLine];

            for (int x = 0; x < NrOfLines; x++)
            {
                for (int y = 0; y < NrOfCellsPerLine; y++)
                {
                    Vector sum = new Vector(0.0f, 0.0f, 0.0f);

                    if ((x > 0 && y > 0) && (x < NrOfLines - 1 && y < NrOfCellsPerLine - 1))
                    {
                        Vector v0 = new Vector(x * Xdimension, y * Ydimension, zData[x, y]);
                        Vector v1 = new Vector((x - 1) * Xdimension, y * Ydimension, zData[x - 1, y]);
                        Vector v2 = new Vector(x * Xdimension, (y - 1) * Ydimension, zData[x, y - 1]);
                        Vector v3 = new Vector((x + 1) * Xdimension, (y - 1) * Ydimension, zData[x + 1, y - 1]);
                        Vector v4 = new Vector((x + 1) * Xdimension, y * Ydimension, zData[x + 1, y]);
                        Vector v5 = new Vector(x * Xdimension, (y + 1) * Ydimension, zData[x, y + 1]);
                        Vector v6 = new Vector((x - 1) * Xdimension, (y + 1) * Ydimension, zData[x - 1, y + 1]);

                        sum += (2.0 / 8.0) * Vector.Normalize((v1 - v0) % (v2 - v0));
                        sum += (1.0 / 8.0) * Vector.Normalize((v2 - v0) % (v3 - v0));
                        sum += (1.0 / 8.0) * Vector.Normalize((v3 - v0) % (v4 - v0));
                        sum += (2.0 / 8.0) * Vector.Normalize((v4 - v0) % (v5 - v0));
                        sum += (1.0 / 8.0) * Vector.Normalize((v5 - v0) % (v6 - v0));
                        sum += (1.0 / 8.0) * Vector.Normalize((v6 - v0) % (v1 - v0));
                        normals[x, y] = sum;
                    }
                    else
                        if ((x == 0) && (y > 0 && y < NrOfCellsPerLine - 1))
                        {
                            Vector v0 = new Vector(x * Xdimension, y * Ydimension, zData[x, y]);
                            Vector v1 = new Vector(x * Xdimension, (y - 1) * Ydimension, zData[x, y - 1]);
                            Vector v2 = new Vector((x + 1) * Xdimension, (y - 1) * Ydimension, zData[x + 1, y - 1]);
                            Vector v3 = new Vector((x + 1) * Xdimension, y * Ydimension, zData[x + 1, y]);
                            Vector v4 = new Vector(x * Xdimension, (y + 1) * Ydimension, zData[x, y + 1]);

                            sum += Vector.Normalize((v1 - v0) % (v2 - v0));
                            sum += Vector.Normalize((v2 - v0) % (v3 - v0));
                            sum += Vector.Normalize((v3 - v0) % (v4 - v0));
                            normals[x, y] = sum / 3.0;
                        }
                        else
                            if ((x == NrOfLines - 1) && (y > 0 && y < NrOfCellsPerLine - 1))
                            {
                                Vector v0 = new Vector(x * Xdimension, y * Ydimension, zData[x, y]);
                                Vector v1 = new Vector(x * Xdimension, (y - 1) * Ydimension, zData[x, y - 1]);
                                Vector v2 = new Vector((x - 1) * Xdimension, y * Ydimension, zData[x - 1, y]);
                                Vector v3 = new Vector((x - 1) * Xdimension, (y + 1) * Ydimension, zData[x - 1, y + 1]);
                                Vector v4 = new Vector(x * Xdimension, (y + 1) * Ydimension, zData[x, y + 1]);

                                sum += Vector.Normalize((v1 - v0) % (v2 - v0));
                                sum += Vector.Normalize((v2 - v0) % (v3 - v0));
                                sum += Vector.Normalize((v3 - v0) % (v4 - v0));
                                normals[x, y] = -sum / 3.0;
                            }
                            else
                            {
                                normals[x, y] = new Vector(0, 0, 0);
                            }
                }
            }
        }

        //Returns the normal at (x, z)
        private Vector getNormal(int x, int y)
        {
            //if (!computedNormals)
            //{
            //    computeNormals();
            //}
            return normals[x, y];
        }

        private float nElevation(float elevation)
        {
            return ((minElevation - elevation) / (minElevation - maxElevation));
        }

        private List<Vector> Szintvonal(Vector p0, Vector p1, Vector p2, double level)
        {
            List<Vector> points = new List<Vector>();

            double dZ0 = Math.Abs(p0.Z - p1.Z);
            double dZ1 = Math.Abs(p1.Z - p2.Z);
            double dZ2 = Math.Abs(p2.Z - p0.Z);

            if (dZ0 >= dZ1 && dZ0 >= dZ2)
            {
                isoLineCalc(p0, p1, p2, level, points);
            }
            else
                if (dZ1 >= dZ0 && dZ1 >= dZ2)
                {
                    isoLineCalc(p1, p2, p0, level, points);
                }
                else
                    if (dZ2 >= dZ0 && dZ2 >= dZ1)
                    {
                        isoLineCalc(p2, p0, p1, level, points);
                    }
            return (points);
        }

        private void isoLineCalc(Vector p0, Vector p1, Vector p2, double level, List<Vector> points)
        {
            double min = Math.Min(p0.Z, p1.Z);
            double max = Math.Max(p0.Z, p1.Z);
            double step = Math.Round(min / level, 0) * level;

            do
            {
                isoPointCalc(p0, p1, points, step);

                if (!isoPointCalc(p1, p2, points, step))
                {
                    isoPointCalc(p2, p0, points, step);
                }

                step = step + level;
            } while (step <= max);
        }

        private bool isoPointCalc(Vector p0, Vector p1, List<Vector> points, double step)
        {
            double t = tCalc(step, p0.Z, p1.Z);

            if (t >= 0.0 && t <= 1.0)
            {
                points.Add(CalcPoint(t, p0, p1));
                return (true);
            }
            else
            {
                return (false);
            }
        }

        private double tCalc(double z, double z0, double z1)
        {
            return ((z - z0) / (-z0 + z1));
        }

        private Vector CalcPoint(double t, Vector p0, Vector p1)
        {
            return (new Vector((1.0 - t) * p0 + t * p1));
        }








        private double A(Vector p1, Vector p2, Vector p3)
        {
            double x1 = p1.X;
            double x2 = p2.X;
            double x3 = p3.X;

            double y1 = p1.Y;
            double y2 = p2.Y;
            double y3 = p3.Y;

            double z1 = p1.Z;
            double z2 = p2.Z;
            double z3 = p3.Z;

            return (-(y2 * z3 - y2 * z1 + y3 * z1 - z3 * y1 + z2 * y1 - z2 * y3) / (x1 * y2 - x1 * y3 - y1 * x2 + y1 * x3 - x3 * y2 + y3 * x2));
        }

        private double B(Vector p1, Vector p2, Vector p3)
        {
            double x1 = p1.X;
            double x2 = p2.X;
            double x3 = p3.X;

            double y1 = p1.Y;
            double y2 = p2.Y;
            double y3 = p3.Y;

            double z1 = p1.Z;
            double z2 = p2.Z;
            double z3 = p3.Z;

            return ((-x1 * z3 + x1 * z2 - x3 * z2 + z3 * x2 - z1 * x2 + z1 * x3) / (x1 * y2 - x1 * y3 - y1 * x2 + y1 * x3 - x3 * y2 + y3 * x2));
        }

        private double C(Vector p1, Vector p2, Vector p3)
        {
            double x1 = p1.X;
            double x2 = p2.X;
            double x3 = p3.X;

            double y1 = p1.Y;
            double y2 = p2.Y;
            double y3 = p3.Y;

            double z1 = p1.Z;
            double z2 = p2.Z;
            double z3 = p3.Z;

            return ((-y2 * z1 * x3 + z2 * y1 * x3 - y3 * x1 * z2 + y3 * z1 * x2 + y2 * x1 * z3 - z3 * y1 * x2) / (x1 * y2 - x1 * y3 - y1 * x2 + y1 * x3 - x3 * y2 + y3 * x2));
        }

        private double zCalc(Vector a, Vector b, Vector c, double x, double y)
        {
            double pA = A(a, b, c);
            double pB = B(a, b, c);
            double pC = C(a, b, c);

            return ((pA * x) + (pB * y) + pC);
        }

        private bool PointInTriangle(Vector A, Vector B, Vector C, Vector P)
        {
            // Compute vectors        
            Vector v0 = C - A;
            Vector v1 = B - A;
            Vector v2 = P - A;

            // Compute dot products
            double dot00 = v0 * v0;
            double dot01 = v0 * v1;
            double dot02 = v0 * v2;
            double dot11 = v1 * v1;
            double dot12 = v1 * v2;

            // Compute barycentric coordinates
            double invDenom = 1.0 / (dot00 * dot11 - dot01 * dot01);
            double u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            double v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            // Check if point is in triangle
            return (u >= 0) && (v >= 0) && (u + v <= 1);
        }

        private bool PointInTriangle2D(Vector A, Vector B, Vector C, Vector P, out double u, out double v)
        {
            // Compute vectors        
            Vector v0 = C - A;
            Vector v1 = B - A;
            Vector v2 = P - A;

            v0.Z = 0.0;
            v1.Z = 0.0;
            v2.Z = 0.0;

            // Compute dot products
            double dot00 = v0 * v0;
            double dot01 = v0 * v1;
            double dot02 = v0 * v2;
            double dot11 = v1 * v1;
            double dot12 = v1 * v2;

            // Compute barycentric coordinates
            double invDenom = 1.0 / (dot00 * dot11 - dot01 * dot01);
            u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            // Check if point is in triangle
            return (u >= 0) && (v >= 0) && (u + v <= 1);
        }

        /// <summary>
        /// Megadja, hogy az él melyik oldalán fekszik a p csomópont
        /// </summary>
        /// <param name="p">A vizsgált csomópont</param>
        /// <param name="node1">Az él kezdőpontja</param>
        /// <param name="node2">Az él végpontja</param>
        /// <returns>Megkapjuk a p csomópont helyzetét</returns>
        private Direction WhichSide(Vector p, Vector node1, Vector node2)
        {
            double equation = ((p.Y - node1.Y) * (node2.X - node1.X)) - ((node2.Y - node1.Y) * (p.X - node1.X));
            //double equation = ((node2.X - node1.X) * (p.Y - node1.Y)) - ((p.X - node1.X) * (node2.Y - node1.Y));

            if (equation > 0)
            {
                return (Direction.Left);
            }
            else if (equation < 0)
            {
                return (Direction.Right);
            }
            else
            {
                return (Direction.Boundary);
            }
        }

        private Direction PointInTri(Vector A, Vector B, Vector C, Vector P)
        {
            if (WhichSide(P, A, B) == Direction.Left)
            {
                if (WhichSide(P, B, C) == Direction.Left)
                {
                    if (WhichSide(P, C, A) == Direction.Left)
                    {
                        return (Direction.In);
                    }
                    else return (Direction.Out);
                }
                else return (Direction.Out);
            }
            else return (Direction.Out);
        }

        public Point[] RowSection(int row, Rectangle rec)
        {
            Point[] points = new Point[zData.GetLength(1)];

            for (int i = 0; i < zData.GetLength(1); i++)
            {
                points[i].X = i * Xdimension;
                points[i].Y = (int)zData[row, i];
            }

            double maxWidth = (zData.GetLength(1) - 1) * Xdimension;

            double facW = (rec.Width) / maxWidth;
            double facH = (rec.Height) / (maxElevation - minElevation);

            for (int j = 0; j < points.Length; j++)
            {
                points[j].X = (int)(points[j].X * facW);
                points[j].Y = (int)(rec.Height - points[j].Y * facH);
            }
            return (points);
        }

        public Point[] ColumnSection(int column, Rectangle rec)
        {
            Point[] points = new Point[zData.GetLength(0)];

            for (int i = 0; i < zData.GetLength(0); i++)
            {
                points[i].X = i * Xdimension;
                points[i].Y = (int)zData[i, column];
            }

            double maxWidth = (zData.GetLength(0) - 1) * Xdimension;

            double facW = (rec.Width) / maxWidth;
            double facH = (rec.Height) / (maxElevation - minElevation);

            for (int j = 0; j < points.Length; j++)
            {
                points[j].X = (int)(points[j].X * facW);
                points[j].Y = (int)(rec.Height - points[j].Y * facH);
            }
            return (points);
        }

        public Vector GetHeight(double x, double y)
        {
            int gridX = (int)Math.Truncate(x / Xdimension);
            int gridY = (int)Math.Truncate(y / Ydimension);
            Vector P = new Vector(x, y);
            Bezier BR = new Bezier();

            double u = 0;
            double v = 0;

            if (gridX < 0 || gridX >= zData.GetLength(0) - 1 || gridY < 0 || gridY >= zData.GetLength(1) - 1)
            {
                return (new Vector(0, 0, 0));
            }
            else
            {
                Vector A = new Vector(gridX * Xdimension, gridY * Ydimension, zData[gridX, gridY]);
                Vector B = new Vector((gridX + 1) * Xdimension, gridY * Ydimension, zData[gridX + 1, gridY]);
                Vector C = new Vector(gridX * Xdimension, (gridY + 1) * Ydimension, zData[gridX, gridY + 1]);
                Vector D = new Vector((gridX + 1) * Xdimension, (gridY + 1) * Ydimension, zData[gridX + 1, gridY + 1]);

                Vector nA = normals[gridX, gridY];
                Vector nB = normals[gridX + 1, gridY];
                Vector nC = normals[gridX, gridY + 1];
                Vector nD = normals[gridX + 1, gridY + 1];

                if (PointInTriangle2D(A, B, C, P, out u, out v))
                {
                    //return (zCalc(A, B, C, x, y));
                    //return P = A + u * (C - A) + v * (B - A);
                    BR.P1 = A;
                    BR.P2 = B;
                    BR.P3 = C;
                    BR.N1 = nA;
                    BR.N2 = nB;
                    BR.N3 = nC;
                    return BR.GetPoint(v, u);
                }

                if (PointInTriangle2D(C, D, B, P, out u, out v))
                {
                    //return (zCalc(C, D, B, x, y));
                    //return P = C + u * (B - C) + v * (D - C);
                    BR.P1 = C;
                    BR.P2 = D;
                    BR.P3 = B;
                    BR.N1 = nC;
                    BR.N2 = nD;
                    BR.N3 = nB;
                    return BR.GetPoint(v, u);
                }
            }
            return (new Vector(0, 0, 0));
        }
    }
}