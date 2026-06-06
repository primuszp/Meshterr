using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Meshterr
{
    class Tin : DisplayList
    {
        #region Private Members

        private int distortion = 1;
        private Mesh mesh = null;

        #endregion

        #region Public Properties

        /// <summary>
        /// A terepmodellt alkotó háló
        /// </summary>
        public Mesh Mesh
        {
            get { return mesh; }
            set { mesh = value; }
        }

        public int Distoration
        {
            get { return distortion; }
            set { distortion = value; }
        }

        public BoundingBox Box { get; set; }
        public double MaxElevation { get; set; }
        public double MinElevation { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int OffsetZ { get; set; }

        #endregion

        #region Constructors

        public Tin()
            : base()
        {
            mesh = new Mesh();
        }

        public Tin(Mesh mesh)
            : base()
        {
            this.mesh = mesh;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// TIN felületmodell generálás
        /// </summary>
        public void Triangulation()
        {
            //Háromszögháló felépítése
            GenerateTIN();

            //A csomópontokhoz kapcsolódó háromszögek beállítása
            Mesh.BuildNodeConnections();

            //A felületi és csomóponti normálisok számítása
            Mesh.CalculateNormals();
        }

        /// <summary>
        /// Delaunay-féle hárömszögesítés
        /// </summary>
        /// <param name="nodes">A hálót alkotó pontok listája</param>
        /// <returns>A hálót alkotó háromszögek listája</returns>
        public List<Face> Triangulation(List<Node> nodes)
        {
            double xmin;
            double ymin;

            int nv = nodes.Count;

            if (nv < 3)
            {
                throw new ArgumentException("Need at least three vertices for triangulation");
            }

            int trimax = 4 * nv;

            // Find the maximum and minimum vertex bounds.
            // This is to allow calculation of the bounding supertriangle
            xmin = nodes[0].X;
            ymin = nodes[0].Y;
            double xmax = xmin;
            double ymax = ymin;

            for (int i = 1; i < nv; i++)
            {
                if (nodes[i].X < xmin) xmin = nodes[i].X;
                if (nodes[i].X > xmax) xmax = nodes[i].X;
                if (nodes[i].Y < ymin) ymin = nodes[i].Y;
                if (nodes[i].Y > ymax) ymax = nodes[i].Y;
            }

            double dx = xmax - xmin;
            double dy = ymax - ymin;
            double dmax = (dx > dy) ? dx : dy;

            double xmid = (xmax + xmin) * 0.5;
            double ymid = (ymax + ymin) * 0.5;

            // Set up the supertriangle
            // This is a triangle which encompasses all the sample points.
            // The supertriangle coordinates are added to the end of the
            // vertex list. The supertriangle is the first triangle in
            // the triangle list.
            nodes.Add(new Vector3d((xmid - 2 * dmax), (ymid - dmax)));
            nodes.Add(new Vector3d(xmid, (ymid + 2 * dmax)));
            nodes.Add(new Vector3d((xmid + 2 * dmax), (ymid - dmax)));

            List<Face> Triangles = new List<Face>();

            // SuperTriangle placed at index 0
            Triangles.Add(new Face(nv, nv + 1, nv + 2));

            // Include each point one at a time into the existing mesh
            for (int i = 0; i < nv; i++)
            {
                List<Edge> Edges = new List<Edge>(); //[trimax * 3];

                // Set up the edge buffer.
                // If the point (Vertex(i).x,Vertex(i).y) lies inside the circumcircle then the
                // three edges of that triangle are added to the edge buffer and the triangle is removed from list.
                for (int j = 0; j < Triangles.Count; j++)
                {
                    if (InCircle2D(nodes[i].Vertex, nodes[Triangles[j][0]].Vertex, nodes[Triangles[j][2]].Vertex, nodes[Triangles[j][1]].Vertex))
                    {
                        Edges.Add(new Edge(Triangles[j][0], Triangles[j][1]));
                        Edges.Add(new Edge(Triangles[j][1], Triangles[j][2]));
                        Edges.Add(new Edge(Triangles[j][2], Triangles[j][0]));
                        Triangles.RemoveAt(j);
                        j--;
                    }
                }

                //In case we the last duplicate point we removed was the last in the array
                if (i >= nv)
                {
                    continue;
                }

                // Remove duplicate edges
                // Note: if all triangles are specified anticlockwise then all
                // interior edges are opposite pointing in direction.
                for (int j = Edges.Count - 2; j >= 0; j--)
                {
                    for (int k = Edges.Count - 1; k >= j + 1; k--)
                    {
                        if (Edges[j].Equals(Edges[k]))
                        {
                            Edges.RemoveAt(k);
                            Edges.RemoveAt(j);
                            k--;
                            continue;
                        }
                    }
                }

                // Form new triangles for the current point
                // Skipping over any tagged edges.
                // All edges are arranged in clockwise order.
                foreach (Edge iEdge in Edges)
                {
                    if (Triangles.Count >= trimax)
                    {
                        throw new ApplicationException("Exceeded maximum edges");
                    }

                    Triangles.Add(new Face(iEdge.StartNode, iEdge.EndNode, i));
                }
                Edges.Clear();
                Edges = null;
            }

            // Remove triangles with supertriangle vertices
            // These are triangles which have a vertex number greater than nv
            for (int i = Triangles.Count - 1; i >= 0; i--)
            {
                if (Triangles[i][0] >= nv || Triangles[i][1] >= nv || Triangles[i][2] >= nv)
                {
                    Triangles.RemoveAt(i);
                }
            }

            // Remove SuperTriangle vertices
            nodes.RemoveAt(nodes.Count - 1);
            nodes.RemoveAt(nodes.Count - 1);
            nodes.RemoveAt(nodes.Count - 1);
            Triangles.TrimExcess();

            return (Triangles);
        }

        /// <summary>
        /// Virtual method to Regenerate the display lists based on the parameters for solid fill and edge drawing 
        /// </summary>
        /// <param name="renderingOptions">Rendering options to impose on the regenerated display list objects.</param>
        /// <exception cref="ArgumentNullException">Mesh member is null.</exception>
        public override void Regenerate(RenderingType RenderingOption)
        {
            if (Mesh != null)
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
                            RenderVertices(5);
                            break;
                        }
                    case RenderingType.Wireframe:
                        {
                            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                            GL.Color3(Mesh.Color);
                            RenderFaces();
                            break;
                        }
                    case RenderingType.Solid:
                        {
                            GL.Enable(EnableCap.CullFace);
                            GL.CullFace(CullFaceMode.Back);
                            GL.Enable(EnableCap.PolygonOffsetFill);
                            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                            GL.Color3(Mesh.Color);
                            RenderFaces();

                            GL.Disable(EnableCap.CullFace);
                            GL.Disable(EnableCap.PolygonOffsetFill);
                            break;
                        }
                    case RenderingType.Shaded:
                        {
                            float[] ambientColor = { 0.4f, 0.4f, 0.4f, 1.0f };
                            float[] lightColor0 = { 0.6f, 0.6f, 0.6f, 1.0f };
                            float[] lightPos0 = { -0.5f, 0.8f, 0.1f, 0.0f };

                            // Enable culling
                            GL.Enable(EnableCap.CullFace);
                            GL.CullFace(CullFaceMode.Back);
                            GL.Enable(EnableCap.ColorMaterial);
                            GL.Enable(EnableCap.Lighting);
                            GL.Enable(EnableCap.Light0);
                            GL.Enable(EnableCap.Normalize);
                            GL.ShadeModel(ShadingModel.Smooth);

                            GL.LightModel(LightModelParameter.LightModelAmbient, ambientColor);
                            GL.Light(LightName.Light0, LightParameter.Diffuse, lightColor0);
                            GL.Light(LightName.Light0, LightParameter.Position, lightPos0);

                            GL.Enable(EnableCap.PolygonOffsetFill);
                            GL.PolygonOffset(1f, 1f);

                            RenderFaces();

                            GL.Disable(EnableCap.PolygonOffsetFill);
                            GL.Disable(EnableCap.CullFace);
                            GL.Disable(EnableCap.Light0);
                            GL.Disable(EnableCap.Lighting);
                            GL.Disable(EnableCap.Normalize);
                            break;
                        }
                    default:
                        break;
                }

                EndList();
            }
            else
            {
                throw new ArgumentNullException("Mesh member is null.");
            }
        }

        #endregion

        #region Private Methods

        #region Draw Methods

        private void RenderVertices(float size)
        {
            if (Mesh != null)
            {
                foreach (Node node in Mesh.Nodes)
                {
                    GL.PointSize(size);
                    GL.Color3(Mesh.Color);

                    GL.Begin(BeginMode.Points);

                    // Vertex
                    if (Mesh.Normalized) GL.Normal3(node.Normal.X, node.Normal.Y, node.Normal.Z);
                    
                    GL.Vertex3(node.X - OffsetX, node.Y - OffsetY, (node.Z - OffsetZ) * Distoration);

                    GL.End();
                }
            }
            else
            {
                throw new ArgumentNullException("Mesh member is null.");
            }
        }

        /// <summary>
        /// Draw the mesh to the OpenGL display list
        /// </summary>
        private void RenderFaces()
        {
            if (Mesh != null)
            {
                GL.Color3(0.3f, 0.9f, 0.0f);

                foreach (Face face in Mesh.Faces)
                {
                    if (face.FaceType == Face.FType.Normal)
                    {
                        GL.Begin(BeginMode.Triangles);

                        // Vertex 1
                        if (Mesh.Normalized) GL.Normal3(Mesh.Nodes[face[0]].Normal.X, Mesh.Nodes[face[0]].Normal.Y, Mesh.Nodes[face[0]].Normal.Z);
                        GL.Vertex3(Mesh.Nodes[face[0]].X - OffsetX, Mesh.Nodes[face[0]].Y - OffsetY, (Mesh.Nodes[face[0]].Z - OffsetZ) * Distoration);

                        // Vertex 2
                        if (Mesh.Normalized) GL.Normal3(Mesh.Nodes[face[1]].Normal.X, Mesh.Nodes[face[1]].Normal.Y, Mesh.Nodes[face[1]].Normal.Z);
                        GL.Vertex3(Mesh.Nodes[face[1]].X - OffsetX, Mesh.Nodes[face[1]].Y - OffsetY, (Mesh.Nodes[face[1]].Z - OffsetZ) * Distoration);

                        // Vertex 3
                        if (Mesh.Normalized) GL.Normal3(Mesh.Nodes[face[2]].Normal.X, Mesh.Nodes[face[2]].Normal.Y, Mesh.Nodes[face[2]].Normal.Z);
                        GL.Vertex3(Mesh.Nodes[face[2]].X - OffsetX, Mesh.Nodes[face[2]].Y - OffsetY, (Mesh.Nodes[face[2]].Z - OffsetZ) * Distoration);

                        GL.End();
                    }
                }
            }
            else
            {
                throw new ArgumentNullException("Mesh member is null.");
            }
        }

        #endregion

        #region TIN Methods

        /// <summary>
        /// TIN felületmodell előállítása szabálytalan ponthalmazból
        /// </summary>
        private void GenerateTIN()
        {
            if (Mesh.Nodes.Count < 3) return;

            #region Sorba rendezés X komponens szerint

            //A terepmodellt alkotó pontokat a keleti (Y) koordináta szerint (EOV rendszer) növekvő sorrendbe rendezzük
            //A pontok 0-tól n-ig tartó sorszámot kapnak, ahol n a terepmodellt alkotó pontok száma (Mesh.Nodes.Count)
            //A „-1” sorszámot a végtelenben lévő elméleti pont kapja
            for (int i = 0; i < Mesh.Nodes.Count; i++)
            {
                for (int j = i; j > 0; j--)
                {
                    if (Mesh.Nodes[j].X < Mesh.Nodes[j - 1].X || (Mesh.Nodes[j].X == Mesh.Nodes[j - 1].X && Mesh.Nodes[j].Y < Mesh.Nodes[j - 1].Y))
                    {
                        Node temp = Mesh.Nodes[j];
                        Mesh.Nodes[j] = Mesh.Nodes[j - 1];
                        Mesh.Nodes[j - 1] = temp;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            #endregion

            //Töröljük a háromszögek listáját
            Mesh.Faces.Clear();

            //Kiszámoljuk a háromszögek számát
            int n = 2 * Mesh.Nodes.Count - 2;

            //A háromszögek számát beállítjuk a pontok kétszeresére
            Mesh.Faces.Capacity = n;

            for (int i = 0; i < n; i++)
            {
                Face newFace = new Face();
                Mesh.Faces.Add(newFace);

                //Kezdetben minden háromszög szomszédja a -1. háromszög
                newFace.FaceIndices[0] = -1;
                newFace.FaceIndices[1] = -1;
                newFace.FaceIndices[2] = -1;
            }

            int FaceUp, FaceDown, nFaceUp = -1, nFaceDown = -1;

            //Ha i értékét 0 és n-1 között végigfuttatjuk, akkor 2n db háromszöget kapunk
            for (int i = 0; i < Mesh.Nodes.Count - 1; i++)
            {
                FaceUp = i * 2;        //Fenti háromszög
                FaceDown = i * 2 + 1;  //Lenti háromszög

                //„Legyező” háromszögeket alakítunk ki: háromszögélt alkot az i. és az i+1. pont,
                //amelynek bal és jobb oldalához egyaránt hozzárendeljük a „végtelenben lévő” -1. pontot

                Mesh.Faces[FaceUp].NodeIndices[0] = -1;
                Mesh.Faces[FaceUp].NodeIndices[1] = i;
                Mesh.Faces[FaceUp].NodeIndices[2] = i + 1;

                Mesh.Faces[FaceDown].NodeIndices[0] = -1;
                Mesh.Faces[FaceDown].NodeIndices[1] = i + 1;
                Mesh.Faces[FaceDown].NodeIndices[2] = i;

                Mesh.ConnectTriangles(FaceUp, FaceDown);

                //Összefűzzük a háromszögeket a végtelenbe mutató közös él mentén
                if (i > 0)//If i > 1 Then
                {
                    Mesh.Faces[FaceUp].RotateRight();
                    Mesh.Faces[FaceDown].RotateLeft();
                    Mesh.ConnectTriangles(FaceUp, nFaceUp);
                    Mesh.ConnectTriangles(FaceDown, nFaceDown);
                }

                nFaceUp = FaceUp;
                nFaceDown = FaceDown;
            }

            for (int i = 0; i < Mesh.Faces.Count; i++)
            {
                DelaunayTest(i);
                Mesh.Faces[i].RotateLeft();
                DelaunayTest(i);
                Mesh.Faces[i].RotateLeft();
                DelaunayTest(i);
                Mesh.Faces[i].RotateLeft();
            }

            for (int i = 0; i < Mesh.Faces.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Mesh.Faces[i].FaceType = Face.FType.Normal;

                    //A -1. sorszámú pontot tartalmazó háromszögeket „árnyék” háromszögnek hívjuk
                    if (Mesh.Faces[i].NodeIndices[j] == -1)
                    {
                        Mesh.Faces[i].FaceType = Face.FType.Shadow;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Delaunay-vizsgálat
        /// </summary>
        /// <param name="Index">Index a háromszögre</param>
        private void DelaunayTest(int Index)
        {
            //A vizsgált élhez két szomszédos háromszög tartozik.
            //A két háromszög egy négyszöget alkot, melynek átlója a vizsgált él.
            //A feladat annak eldöntése, hogy az átló a megfelelő két csúcspontot köti össze, vagy az élt át kell billenteni

            if (Index < 0 || Index >= Mesh.Faces.Count) return;

            int NeighbourIndex = Mesh.Faces[Index].FaceIndices[0];

            if (NeighbourIndex < 0 || NeighbourIndex >= Mesh.Faces.Count) return;

            Mesh.ConnectTriangles(Index, NeighbourIndex);

            int NodeA = Mesh.Faces[Index].NodeIndices[1];
            int NodeB = Mesh.Faces[NeighbourIndex].NodeIndices[0];
            int NodeC = Mesh.Faces[Index].NodeIndices[2];
            int NodeD = Mesh.Faces[Index].NodeIndices[0];

            int FaceA = Mesh.Faces[NeighbourIndex].FaceIndices[1];
            int FaceB = Mesh.Faces[NeighbourIndex].FaceIndices[2];
            int FaceC = Mesh.Faces[Index].FaceIndices[1];
            int FaceD = Mesh.Faces[Index].FaceIndices[2];

            if (NodeB == -1 || NodeD == -1) return;

            bool flip = false;

            if (NodeA == -1)
            {
                //Ha „A” csúcspont a végtelenben van, az élt akkor kell átbillenteni, ha a „C” csúcspont „BD” éltől jobbra van
                flip = RightOf2D(Mesh.Nodes[NodeC], Mesh.Nodes[NodeB], Mesh.Nodes[NodeD]);
            }
            else if (NodeC == -1)
            {
                //Ha „C” csúcspont a végtelenben van, az élt akkor kell átbillenteni, ha az „A” csúcspont „BD” éltől balra van
                flip = LeftOf2D(Mesh.Nodes[NodeA], Mesh.Nodes[NodeB], Mesh.Nodes[NodeD]);
            }
            else
            {
                //Ha „A” és „C” csúcspont egyike sincs a végtelenben, akkor a Delaunay-vizsgálatot végezzük el:
                //ha „B” az „ACD” háromszög köré írható körön belül van, akkor a vizsgált élt át kell billenteni
                flip = InCircle2D(Mesh.Nodes[NodeB], Mesh.Nodes[NodeA], Mesh.Nodes[NodeC], Mesh.Nodes[NodeD]);
            }

            //Amennyiben a vizsgált élt át kell billenteni, akkor a négyszög oldalait alkotó élekre is lefuttatjuk az előző vizsgálatot
            if (flip)
            {
                Mesh.Faces[Index].NodeIndices[0] = NodeA;
                Mesh.Faces[Index].NodeIndices[1] = NodeB;
                Mesh.Faces[Index].NodeIndices[2] = NodeD;

                Mesh.Faces[NeighbourIndex].NodeIndices[0] = NodeC;
                Mesh.Faces[NeighbourIndex].NodeIndices[1] = NodeD;
                Mesh.Faces[NeighbourIndex].NodeIndices[2] = NodeB;

                Mesh.ConnectTriangles(Index, NeighbourIndex);
                Mesh.Faces[Index].RotateLeft();
                Mesh.ConnectTriangles(Index, FaceD);
                Mesh.Faces[Index].RotateLeft();
                Mesh.ConnectTriangles(Index, FaceA);

                Mesh.Faces[NeighbourIndex].RotateLeft();
                Mesh.ConnectTriangles(NeighbourIndex, FaceB);
                Mesh.Faces[NeighbourIndex].RotateLeft();
                Mesh.ConnectTriangles(NeighbourIndex, FaceC);

                DelaunayTest(FaceA);
                DelaunayTest(FaceC);
                DelaunayTest(FaceB);
                DelaunayTest(FaceD);
            }
        }

        /// <summary>
        /// Megadja, hogy a P pont az AB oldal jobb oldalán fekszik-e
        /// (óramutató járásával egyező körüljárás)
        /// </summary>
        /// <param name="P">A vizsgált pont</param>
        /// <param name="A">Az él első pontja</param>
        /// <param name="B">Az él második pontja</param>
        /// <returns>Igaz (true), ha P pont az AB él jobb oldalán van, különben hamis (false)</returns>
        private bool RightOf2D(Node P, Node A, Node B)
        {
            return (((A.X - P.X) * (B.Y - P.Y) - (A.Y - P.Y) * (B.X - P.X)) < 0);
        }

        /// <summary>
        /// Megadja, hogy a P pont az AB oldal bal oldalán fekszik-e
        /// (óramutató járásával ellentétes körüljárás)
        /// </summary>
        /// <param name="P">A vizsgált pont</param>
        /// <param name="A">Az él első pontja</param>
        /// <param name="B">Az él második pontja</param>
        /// <returns>Igaz (true), ha P pont az AB él bal oldalán van, különben hamis (false)</returns>
        private bool LeftOf2D(Node P, Node A, Node B)
        {
            return (((A.X - P.X) * (B.Y - P.Y) - (A.Y - P.Y) * (B.X - P.X)) > 0);
        }

        /// <summary>
        /// Pont a háromszögben teszt
        /// </summary>
        /// <param name="P">A vizsgált pont</param>
        /// <param name="A">Háromszög első csúcspontja</param>
        /// <param name="B">Háromszög második csúcspontja</param>
        /// <param name="C">Háromszög harmadik csúcspontja</param>
        /// <returns>Igaz (true), ha P pont az ABC háromszögön belül van, különben hamis (false)</returns>
        private bool Inside2D(Node P, Node A, Node B, Node C)
        {
            if (LeftOf2D(A, B, P))
            {
                if (LeftOf2D(B, C, P))
                {
                    if (LeftOf2D(C, A, P))
                    {
                        return (true);
                    }
                }
            }
            return (false);
        }

        /// <summary>
        /// A P pont az ABC háromszög köré írt körén belül van, ha az ABC háromszög körüljárása óramutató járásával ellentétes
        /// </summary>
        /// <param name="P">A vizsgált pont</param>
        /// <param name="A">A háromszög első csúcspontja</param>
        /// <param name="B">A háromszög második csúcspontja</param>
        /// <param name="C">A háromszög harmadik csúcspontja</param>
        /// <returns>Igaz (true), ha P pont az ABC háromszög köré írt körén belül van, különben hamis (false)</returns>
        private bool InCircle2D(Node P, Node A, Node B, Node C)
        {
            double X21 = B.X - A.X, Y21 = B.Y - A.Y;
            double X23 = B.X - C.X, Y23 = B.Y - C.Y;
            double X41 = P.X - A.X, Y41 = P.Y - A.Y;
            double X43 = P.X - C.X, Y43 = P.Y - C.Y;

            return ((Y41 * X23 + X41 * Y23) * (X43 * X21 - Y43 * Y21)) > ((Y43 * X21 + X43 * Y21) * (X41 * X23 - Y41 * Y23));
        }

        #endregion

        #endregion
    }
}
