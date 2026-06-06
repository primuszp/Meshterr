using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace MeshTopology
{
    class TIN
    {
        public static double xmin;
        public static double ymin;

         /// <summary>
        /// Delaunay-féle hárömszögesítés
        /// </summary>
        /// <param name="Vertices">A hálót alkotó pontok listája</param>
        /// <returns>A hálót alkotó háromszögek listája</returns>
        public static List<Face> Triangulation(List<Node> nodes)
        {
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
            nodes.Add(new Vector((xmid - 2 * dmax), (ymid - dmax)));
            nodes.Add(new Vector(xmid, (ymid + 2 * dmax)));
            nodes.Add(new Vector((xmid + 2 * dmax), (ymid - dmax)));

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
                    if (InCircle(nodes[i].Vertex, nodes[Triangles[j][0]].Vertex, nodes[Triangles[j][1]].Vertex, nodes[Triangles[j][2]].Vertex))
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

                    Triangles.Add(new Face(iEdge.Snode, iEdge.Enode, i));
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

            for (int i = Triangles.Count - 1; i >= 0; i--)
            {
                nodes[Triangles[i].IndexToNode1].LinkedFaces.Add(i);
                nodes[Triangles[i].IndexToNode2].LinkedFaces.Add(i);
                nodes[Triangles[i].IndexToNode3].LinkedFaces.Add(i);
            }

            return (Triangles);
        }

        /// <summary>
        /// Get the area of a triangle using 'Heron's formula'.
        /// </summary>
        /// <param name="a">The length of angle A</param>
        /// <param name="b">The length of angle B</param>
        /// <param name="c">The length of angle C</param>
        /// <returns>The area of the triangle.</returns>
        public double getAreaOfTriangle(double a, double b, double c)
        {
            double s = (a + b + c) / 2;
            return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }

        private static bool InCircle(Vector p, Vector p1, Vector p2, Vector p3)
        {
            //'// InCircle - p pont az abc háromszög köré írt körön belül van, ha abc
            //'óramutató járásával megegyező
            //'// paraméter: négy pont - vissza: igaz, ha p pont abc háromszög köré írt
            //'körön belül van, különben hamis
            //'BOOL InCircle(Point * A, Point * B, Point * C, Point * P)
            double X21 = p2.X - p1.X, Y21 = p2.Y - p1.Y;
            double X23 = p2.X - p3.X, Y23 = p2.Y - p3.Y;
            double X41 = p.X - p1.X, Y41 = p.Y - p1.Y;
            double X43 = p.X - p3.X, Y43 = p.Y - p3.Y;

            return ((Y41 * X23 + X41 * Y23) * (X43 * X21 - Y43 * Y21)) < ((Y43 * X21 + X43 * Y21) * (X41 * X23 - Y41 * Y23));
        }

        /// <summary>
        /// Megvizsgálja, hogy a p pont a háromszög köré írható körén belül vagy kívül helyezkedik el
        /// </summary>
        /// <param name="p">A vizsgált pont</param>
        /// <param name="p1">A háromszög első pontja</param>
        /// <param name="p2">A háromszög második pontja</param>
        /// <param name="p3">A háromszög harmadik pontja</param>
        /// <returns>Ha a körön belül fekszik (true) egyébként (false)</returns>
        /// <implementation>
        /// Ha a háromszög élén helyezkedik el a vizsgált pont, akkor a körön belül van 
        /// </implementation>
        private static bool InCircle2(Vector p, Vector p1, Vector p2, Vector p3)
        {
            if (Math.Abs(p1.Y - p2.Y) < double.Epsilon && Math.Abs(p2.Y - p3.Y) < double.Epsilon)
            {
                //A pontok egybeesnek egymással
                return (false);
            }

            double m01, m02;
            double mX1, mX2;
            double mY1, mY2;
            double circumCircleX, circumCircleY;

            if (Math.Abs(p2.Y - p1.Y) < double.Epsilon)
            {
                m02 = -(p3.X - p2.X) / (p3.Y - p2.Y);
                mX2 = (p2.X + p3.X) * 0.5;
                mY2 = (p2.Y + p3.Y) * 0.5;

                //Calculate CircumCircle center
                circumCircleX = (p2.X + p1.X) * 0.5;
                circumCircleY = m02 * (circumCircleX - mX2) + mY2;
            }
            else if (Math.Abs(p3.Y - p2.Y) < double.Epsilon)
            {
                m01 = -(p2.X - p1.X) / (p2.Y - p1.Y);
                mX1 = (p1.X + p2.X) * 0.5;
                mY1 = (p1.Y + p2.Y) * 0.5;

                //Calculate CircumCircle center
                circumCircleX = (p3.X + p2.X) * 0.5;
                circumCircleY = m01 * (circumCircleX - mX1) + mY1;
            }
            else
            {
                m01 = -(p2.X - p1.X) / (p2.Y - p1.Y);
                m02 = -(p3.X - p2.X) / (p3.Y - p2.Y);
                mX1 = (p1.X + p2.X) * 0.5;
                mX2 = (p2.X + p3.X) * 0.5;
                mY1 = (p1.Y + p2.Y) * 0.5;
                mY2 = (p2.Y + p3.Y) * 0.5;

                //Calculate CircumCircle center
                circumCircleX = (m01 * mX1 - m02 * mX2 + mY2 - mY1) / (m01 - m02);
                circumCircleY = m01 * (circumCircleX - mX1) + mY1;
            }

            double dx = p2.X - circumCircleX;
            double dy = p2.Y - circumCircleY;
            double rsqr = dx * dx + dy * dy; //double r = Math.Sqrt(rsqr); //Circumcircle radius
            dx = p.X - circumCircleX;
            dy = p.Y - circumCircleY;
            double drsqr = dx * dx + dy * dy;

            return (drsqr <= rsqr);
        }
    }
}
