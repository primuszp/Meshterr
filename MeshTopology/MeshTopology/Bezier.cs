using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeshTopology
{
    class Bezier
    {
        private struct Barycentric
        {
            private double u;
            private double v;
            private double w;

            #region Public properties

            public double U
            {
                get { return u; }
                set { u = value; }
            }

            public double V
            {
                get { return v; }
                set { v = value; }
            }

            public double W
            {
                get { return w; }
            }

            #endregion

            public Barycentric(double u, double v)
            {
                this.u = u;
                this.v = v;
                this.w = 1 - u - v;
            }

            public static Barycentric operator +(Barycentric p1, Barycentric p2)
            {
                return (new Barycentric(p1.u + p2.u, p1.v + p2.v));
            }

            public static Barycentric operator -(Barycentric p1, Barycentric p2)
            {
                return new Barycentric(p1.u - p2.u, p1.v - p2.v);
            }

            public static Barycentric operator /(Barycentric p, double d)
            {
                return (new Barycentric(p.u / d, p.v / d));
            }

            public static Barycentric operator *(Barycentric p, double d)
            {
                return new Barycentric(p.u * d, p.v * d);
            }

            public static Barycentric operator *(double d, Barycentric p)
            {
                return new Barycentric(p.u * d, p.v * d);
            }
        }

        #region Public Properties

        public Vector P1
        {
            get { return p1; }
            set { p1 = value; }
        }

        public Vector P2
        {
            get { return p2; }
            set { p2 = value; }
        }

        public Vector P3
        {
            get { return p3; }
            set { p3 = value; }
        }

        public Vector N1
        {
            get { return n1; }
            set { n1 = value; }
        }

        public Vector N2
        {
            get { return n2; }
            set { n2 = value; }
        }

        public Vector N3
        {
            get { return n3; }
            set { n3 = value; }
        }

        public List<Vector> Normals
        {
            get { return normals; }
        }

        public List<Vector> Vertices
        {
            get { return vertices; }
        }

        #endregion

        /// <summary>
        /// Level of detail
        /// </summary>
        private int lod = 2;

        /// <summary>
        /// Vertices list
        /// </summary>
        private List<Vector> vertices = new List<Vector>();

        /// <summary>
        /// Normals list
        /// </summary>
        private List<Vector> normals = new List<Vector>();

        // Vertex coefficients:
        private Vector p1 = null;
        private Vector p2 = null;
        private Vector p3 = null;

        // Normal vectors:
        private Vector n1 = null;
        private Vector n2 = null;
        private Vector n3 = null;
        private Vector n110 = null;
        private Vector n011 = null;
        private Vector n101 = null;

        // Tangent coefficients:
        private Vector b210 = null;
        private Vector b120 = null;
        private Vector b021 = null;
        private Vector b012 = null;
        private Vector b102 = null;
        private Vector b201 = null;

        // Tangent coefficients:
        private Vector b111;

        #region Public Constructors

        /// <summary>
        /// Bezier Triangle Patch
        /// </summary>
        public Bezier()
        {
            this.p1 = new Vector(0, 0, 0);
            this.p2 = new Vector(0, 0, 0);
            this.p3 = new Vector(0, 0, 0);
            this.n1 = new Vector(0, 0, 0);
            this.n2 = new Vector(0, 0, 0);
            this.n3 = new Vector(0, 0, 0);
        }

        /// <summary>
        /// Bezier Triangle Patch Input Parameters
        /// </summary>
        /// <param name="p1">P1 Vertex Coefficient Vector</param>
        /// <param name="p2">P2 Vertex Coefficient Vector</param>
        /// <param name="p3">P3 Vertex Coefficient Vector</param>
        /// <param name="n1">N1 Normal Vector</param>
        /// <param name="n2">N2 Normal Vector</param>
        /// <param name="n3">N3 Normal Vector</param>
        public Bezier(Vector p1, Vector p2, Vector p3, Vector n1, Vector n2, Vector n3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.n1 = n1;
            this.n2 = n2;
            this.n3 = n3;
        }

        #endregion

        //public void GetSubFaces(Vector p1, Vector p2, Vector p3, Vector n1, Vector n2, Vector n3)
        //{
        //    vertices.Clear();
        //    normals.Clear();
        //    SubdivisionCalculation();
        //}

        public Vector GetPoint(double u, double v)
        {
            ControlPointsCalculation();
            return (BezierFunction(u, v, 1 - u - v));
        }

        public void PointCalculation()
        {
            vertices.Clear();
            normals.Clear();
            SubdivisionCalculation();
        }

        private Vector BezierFunction(double u, double v, double w)
        {
            return (p1 * w * w * w + p2 * u * u * u + p3 * v * v * v
            + b210 * 3.0 * w * w * u + b120 * 3.0 * w * u * u + b201 * 3.0 * w * w * v
            + b021 * 3.0 * u * u * v + b102 * 3.0 * w * v * v + b012 * 3.0 * u * v * v
            + b111 * 6.0 * w * u * v);
        }

        private Vector LinearlyNormals(double u, double v, double w)
        {
            return (Vector.Normalize(w * n1 + u * n2 + v * n3));
        }

        private Vector BezierNormals(double u, double v, double w)
        {
            return (Vector.Normalize(n1 * w * w + n2 * u * u + n3 * v * v + n110 * w * u + n011 * u * v + n101 * w * v));
        }

        //Generating by Recursive Subdivision
        private void Subdivision(Barycentric B0, Barycentric B1, Barycentric B2, int lod)
        {
            if (lod > 0)
            {
                Barycentric Ma = (B0 + B1) / 2.0;
                Barycentric Mb = (B1 + B2) / 2.0;
                Barycentric Mc = (B2 + B0) / 2.0;

                Subdivision(B0, Ma, Mc, lod - 1);
                Subdivision(Ma, B1, Mb, lod - 1);
                Subdivision(Mc, Mb, B2, lod - 1);
                Subdivision(Ma, Mb, Mc, lod - 1);
            }
            else
            {
                vertices.Add(BezierFunction(B0.U, B0.V, B0.W));
                vertices.Add(BezierFunction(B1.U, B1.V, B1.W));
                vertices.Add(BezierFunction(B2.U, B2.V, B2.W));

                //normals.Add(LinearlyNormals(B0.U, B0.V, B0.W));
                //normals.Add(LinearlyNormals(B1.U, B1.V, B1.W));
                //normals.Add(LinearlyNormals(B2.U, B2.V, B2.W));

                normals.Add(BezierNormals(B0.U, B0.V, B0.W));
                normals.Add(BezierNormals(B1.U, B1.V, B1.W));
                normals.Add(BezierNormals(B2.U, B2.V, B2.W));
            }
        }

        private void SubdivisionCalculation()
        {
            ControlPointsCalculation();
            Subdivision(new Barycentric(0.0, 0.0), new Barycentric(1.0, 0.0), new Barycentric(0.0, 1.0), lod);
        }

        private void ControlPointsCalculation()
        {
            double w12 = (p2 - p1) * n1;
            double w21 = (p1 - p2) * n2;
            double w23 = (p3 - p2) * n2;
            double w32 = (p2 - p3) * n3;
            double w31 = (p1 - p3) * n3;
            double w13 = (p3 - p1) * n1;

            b210 = (2.0 * p1 + p2 - w12 * n1) / 3.0;
            b120 = (2.0 * p2 + p1 - w21 * n2) / 3.0;
            b021 = (2.0 * p2 + p3 - w23 * n2) / 3.0;
            b012 = (2.0 * p3 + p2 - w32 * n3) / 3.0;
            b102 = (2.0 * p3 + p1 - w31 * n3) / 3.0;
            b201 = (2.0 * p1 + p3 - w13 * n1) / 3.0;

            Vector E = (b210 + b120 + b021 + b012 + b102 + b201) / 6.0;
            Vector V = (p1 + p2 + p3) / 3.0;

            b111 = E + (E - V) / 2.0;

            //Normál vektorok számítása

            double v12 = 2.0 * (((p2 - p1) * (n1 + n2)) / ((p2 - p1) * (p2 - p1)));
            double v23 = 2.0 * (((p3 - p2) * (n2 + n3)) / ((p3 - p2) * (p3 - p2)));
            double v31 = 2.0 * (((p1 - p3) * (n3 + n1)) / ((p1 - p3) * (p1 - p3)));

            Vector h110 = n1 + n2 - v12 * (p2 - p1);
            Vector h011 = n2 + n3 - v23 * (p3 - p2);
            Vector h101 = n3 + n1 - v31 * (p1 - p3);

            n110 = Vector.Normalize(h110);
            n011 = Vector.Normalize(h011);
            n101 = Vector.Normalize(h101);
        }
    }
}
