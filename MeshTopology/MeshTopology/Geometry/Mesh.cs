using System;
using System.Collections.Generic;
using GRV11;
using System.IO;
using System.Globalization;

namespace MeshTopology
{
    class Mesh
    {
        public enum Direction : byte { Boundary, Right, Left, In, Out }
        public enum Dominance : byte { DominantX, DominantY, DominantZ }

        #region Private Member Variables

        private List<Node> nodes = null;
        private List<Edge> edges = null;
        private List<Face> faces = null;

        #endregion

        #region Public Properties

        /// <summary>
        /// A hálót alkotó csomópontok listája
        /// </summary>
        public List<Node> Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }

        /// <summary>
        /// A hálót alkotó élek listája
        /// </summary>
        public List<Edge> Edges
        {
            get { return edges; }
            set { edges = value; }
        }

        /// <summary>
        /// A hálót alkotó háromszögek listája
        /// </summary>
        public List<Face> Faces
        {
            get { return faces; }
            set { faces = value; }
        }

        /// <summary>
        /// Indexelő a hálót alkotó háromszögekhez
        /// </summary>
        /// <param name="index">Index a háló i-edik háromszögére</param>
        /// <returns>Visszatér a háromszöggel</returns>
        public Face this[int index]
        {
            get { return faces[index]; }
        }

        #endregion

        #region Constructors

        public Mesh()
        {
            nodes = new List<Node>();
            edges = new List<Edge>();
            faces = new List<Face>();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Megadja, hogy az él melyik oldalán fekszik a p csomópont
        /// </summary>
        /// <param name="p">A vizsgált csomópont</param>
        /// <param name="node1">Az él kezdőpontja</param>
        /// <param name="node2">Az él végpontja</param>
        /// <returns>Megkapjuk a p csomópont helyzetét</returns>
        private Direction WhichSide(Node p, Node node1, Node node2)
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

        private double ConvertToDouble(string number)
        {
            return (double.Parse(number, CultureInfo.InvariantCulture));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Csomópont hozzáadása a hálóhoz
        /// </summary>
        /// <param name="node">Az új csomópont</param>
        /// <returns>A csomópontra mutató indexet kapjuk vissza</returns>
        public int AddNode(Node node)
        {
            int index = nodes.IndexOf(node);

            if (index < 0)
            {
                nodes.Add(node);
                return (nodes.Count - 1);
            }
            else
            {
                return (index);
            }
        }

        public bool LoadPoints(string filename)
        {
            StreamReader myFile = File.OpenText(filename);
            try
            {
                do
                {
                    string[] buffer = myFile.ReadLine().Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    //Vector vertex = new Vector(ConvertToDouble(buffer[1]), ConvertToDouble(buffer[2]), ConvertToDouble(buffer[3]));
                    //nodes.Add(new Node(Convert.ToInt32(buffer[0]), vertex, buffer[4]));

                    Vector vertex = new Vector(ConvertToDouble(buffer[0]), ConvertToDouble(buffer[1]), ConvertToDouble(buffer[2]) / 15);
                    nodes.Add(new Node(vertex));
                } while (!myFile.EndOfStream);
                myFile.Close();
                return (true);
            }
            catch
            {
                return (false);
            }
        }


        private void CalcNormals()
        {
            foreach (Node node in nodes)
            {
                double angle = 0.0;
                double sumAngle = 0.0;
                Vector sumVector = new Vector();

                foreach (int index in node.LinkedFaces)
                {
                    Vector A = nodes[faces[index][0]].Vertex;
                    Vector B = nodes[faces[index][1]].Vertex;
                    Vector C = nodes[faces[index][2]].Vertex;

                    if (node.Vertex == A)
                    {
                        angle = Vector.Angle(B - A, C - A);
                    }
                    if (node.Vertex == B)
                    {
                        angle = Vector.Angle(A - B, C - B);
                    }
                    else
                    {
                        angle = Vector.Angle(A - C, B - C);
                    }
                    
                    sumVector += angle * faces[index].FaceNormal;
                    sumAngle += angle;
                }
                node.Normal = sumVector / sumAngle;
            }
        }

        public void Trinagulation(GR gr)
        {
            faces = TIN.Triangulation(nodes);

            foreach (Face face in faces)
            {
                Vector A = nodes[face[0]].Vertex;
                Vector B = nodes[face[1]].Vertex;
                Vector C = nodes[face[2]].Vertex;
                Vector N = (A - B) % (C - B);
                N.Normalize();

                face.FaceNormal = new Vector(N);
            }
            CalcNormals();
            DrawList(gr);
        }

        public void DrawList(GR gr)
        {
            float[] ambientColor = { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] lightColor0 = { 0.6f, 0.6f, 0.6f, 1.0f };
            float[] lightPos0 = { -0.5f, 0.8f, 0.1f, 0.0f };

            gr.glNewList(10, GR.GL_COMPILE);

            gr.glLightModelfv(GR.GL_LIGHT_MODEL_AMBIENT, ambientColor);
            gr.glLightfv(GR.GL_LIGHT0, GR.GL_DIFFUSE, lightColor0);
            gr.glLightfv(GR.GL_LIGHT0, GR.GL_POSITION, lightPos0);
            gr.glColor3f(0.3f, 0.9f, 0.0f);
            gr.glPolygonMode(GR.GL_FRONT_AND_BACK, GR.GL_LINE);

            Bezier bezier = new Bezier();

            gr.glBegin(GR.GL_TRIANGLES);
            foreach (Face face in Faces)
            {
                bezier.P1 = nodes[face[0]].Vertex;
                bezier.P2 = nodes[face[1]].Vertex;
                bezier.P3 = nodes[face[2]].Vertex;
                bezier.N1 = nodes[face[0]].Normal;
                bezier.N2 = nodes[face[1]].Normal;
                bezier.N3 = nodes[face[2]].Normal;
                bezier.PointCalculation();

                for (int i = 0; i < bezier.Vertices.Count; i = i + 3)
                {
                    gr.glNormal3f((float)bezier.Normals[i].X, (float)bezier.Normals[i].Y, (float)bezier.Normals[i].Z);
                    gr.glVertex3d(bezier.Vertices[i].X - TIN.xmin, bezier.Vertices[i].Y - TIN.ymin, bezier.Vertices[i].Z);

                    gr.glNormal3f((float)bezier.Normals[i + 1].X, (float)bezier.Normals[i + 1].Y, (float)bezier.Normals[i + 1].Z);
                    gr.glVertex3d(bezier.Vertices[i+1].X - TIN.xmin, bezier.Vertices[i+1].Y - TIN.ymin, bezier.Vertices[i+1].Z);

                    gr.glNormal3f((float)bezier.Normals[i + 2].X, (float)bezier.Normals[i + 2].Y, (float)bezier.Normals[i + 2].Z);
                    gr.glVertex3d(bezier.Vertices[i+2].X - TIN.xmin, bezier.Vertices[i+2].Y - TIN.ymin, bezier.Vertices[i+2].Z);
                }
            }
            gr.glEnd();




            //gr.glBegin(GR.GL_LINES);
            //gr.glColor3b(86, 110, 120);
            //foreach (Face face in Faces)
            //{
            //    Vector sulypont = (nodes[face[0]].Vertex + nodes[face[1]].Vertex + nodes[face[2]].Vertex) / 3.0;

            //    gr.glVertex3d(sulypont.X, sulypont.Y, sulypont.Z);
            //    gr.glVertex3d(sulypont.X + face.FaceNormal.X, sulypont.Y + face.FaceNormal.Y, sulypont.Z + face.FaceNormal.Z * 1);

            //}
            //gr.glEnd();


            gr.glBegin(GR.GL_LINES);
            gr.glColor3b(86, 110, 120);
            foreach (Face face in Faces)
            {
                bezier.P1 = nodes[face[0]].Vertex;
                bezier.P2 = nodes[face[1]].Vertex;
                bezier.P3 = nodes[face[2]].Vertex;
                bezier.N1 = nodes[face[0]].Normal;
                bezier.N2 = nodes[face[1]].Normal;
                bezier.N3 = nodes[face[2]].Normal;
                bezier.PointCalculation();

                for (int i = 0; i < bezier.Vertices.Count; i++)
                {
                    gr.glVertex3d(bezier.Vertices[i].X - TIN.xmin, bezier.Vertices[i].Y - TIN.ymin, bezier.Vertices[i].Z);
                    gr.glVertex3d((bezier.Vertices[i].X - TIN.xmin) + bezier.Normals[i].X, (bezier.Vertices[i].Y - TIN.ymin) + bezier.Normals[i].Y, bezier.Vertices[i].Z + bezier.Normals[i].Z);
                }
            }
            gr.glEnd();


            //gr.glColor3b(127, 0, 0);
            //gr.glBegin(GR.GL_TRIANGLES);

            //foreach (Face face in Faces)
            //{
            //    gr.glNormal3d(nodes[face[0]].Normal.X, nodes[face[0]].Normal.Y, nodes[face[0]].Normal.Z);
            //    gr.glVertex3d(nodes[face[0]].X - TIN.xmin, nodes[face[0]].Y - TIN.ymin, nodes[face[0]].Z);

            //    gr.glNormal3d(nodes[face[1]].Normal.X, nodes[face[1]].Normal.Y, nodes[face[1]].Normal.Z);
            //    gr.glVertex3d(nodes[face[1]].X - TIN.xmin, nodes[face[1]].Y - TIN.ymin, nodes[face[1]].Z);

            //    gr.glNormal3d(nodes[face[2]].Normal.X, nodes[face[2]].Normal.Y, nodes[face[2]].Normal.Z);
            //    gr.glVertex3d(nodes[face[2]].X - TIN.xmin, nodes[face[2]].Y - TIN.ymin, nodes[face[2]].Z);
            //}

            //gr.glEnd();

            //gr.glBegin(GR.GL_LINES);
            //gr.glColor3b(120, 0, 0);
            //foreach (Node node in nodes)
            //{
            //    gr.glVertex3d(node.X - TIN.xmin, node.Y - TIN.ymin, node.Z);
            //    gr.glVertex3d((node.X - TIN.xmin) + node.Normal.X, (node.Y - TIN.ymin) + node.Normal.Y, node.Z + node.Normal.Z);
            //}
            //gr.glEnd();

            gr.glEndList();
        }

        public void Draw(GR gr)
        {
            gr.glCallList(10);
        }

        #endregion
    }
}