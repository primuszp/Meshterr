using System;
using System.Collections.Generic;

namespace MeshTopology
{
    class Node : IEquatable<Node>
    {
        #region Private Member Variables

        /// <summary>
        /// A csomópont azonosítója
        /// </summary>
        private int id;

        /// <summary>
        /// A csomópont leírása
        /// </summary>
        private string text;

        /// <summary>
        /// A csomópontban értelmezet vertex
        /// </summary>
        private Vector vertex;

        /// <summary>
        /// A csomópontban értelmezett normálvektor
        /// </summary>
        private Vector normal;

        /// <summary>
        /// A csomóponthoz kapcsolódó háromszögek listája
        /// </summary>
        private List<int> linkedFaces;

        #endregion

        #region Public Properties

        /// <summary>
        /// A csomópont azonosítója
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// A csomópont leírása
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// A csomópontban értelmezet vertex
        /// </summary>
        public Vector Vertex
        {
            get { return vertex; }
            set { vertex = value; }
        }

        /// <summary>
        /// A csomópontban értelmezet vertex X komponense
        /// </summary>
        public double X
        {
            get { return vertex.X; }
            set { vertex.X = value; }
        }

        /// <summary>
        /// A csomópontban értelmezet vertex Y komponense
        /// </summary>
        public double Y
        {
            get { return vertex.Y; }
            set { vertex.Y = value; }
        }

        /// <summary>
        /// A csomópontban értelmezet vertex Z komponense
        /// </summary>
        public double Z
        {
            get { return vertex.Z; }
            set { vertex.Z = value; }
        }

        /// <summary>
        /// A csomópontban értelmezett normálvektor
        /// </summary>
        public Vector Normal
        {
            get { return normal; }
            set { normal = value; }
        }

        /// <summary>
        /// A csomóponthoz kapcsolódó háromszögek listája
        /// </summary>
        /// <implementation>
        /// Csak olvasható tulajdonság
        /// </implementation>
        public List<int> LinkedFaces
        {
            get { return linkedFaces; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// A csomópont a (0,0,0) koordinátájú pontban jön létre
        /// </summary>
        public Node()
        {
            vertex = new Vector();
            normal = new Vector();
            linkedFaces = new List<int>();
        }

        /// <summary>
        /// A csomópont a (x,y,z) koordinátájú pontban jön létre
        /// </summary>
        public Node(double x, double y, double z)
        {
            vertex = new Vector(x, y, z);
            normal = new Vector();
            linkedFaces = new List<int>();
        }

        /// <summary>
        /// A csomópont pozícióját állíthatjuk be
        /// </summary>
        /// <param name="vertex">A csomópont geometriája (x,y,z)</param>
        public Node(Vector vertex)
        {
            this.vertex = vertex;
            this.normal = new Vector();
            linkedFaces = new List<int>();
        }

        /// <summary>
        /// A csomópont pozícióját és leírását állíthatjuk be
        /// </summary>
        /// <param name="id">Azonosító</param>
        /// <param name="vertex">A csomópont geometriája (x,y,z)</param>
        /// <param name="text">Leírás</param>
        public Node(int id, Vector vertex, string text)
        {
            this.id = id;
            this.vertex = vertex;
            this.normal = new Vector();
            linkedFaces = new List<int>();
            this.text = text;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// A csomóponthoz kapcsolódó háromszög hozzáadása
        /// </summary>
        /// <param name="index">A kapcsolódó háromszög indexe</param>
        /// <returns>Ha sikerült a listához hozzáadni akkor (true) egyébként (false)</returns>
        public bool AddFace(int index)
        {
            if (!linkedFaces.Contains(index))
            {
                linkedFaces.Add(index);
                return (true);
            }
            else
            {
                return (false);
            }
        }

        public static implicit operator Node(Vector vector)
        {
            return new Node(vector);
        }

        /// <summary>
        /// A csomópont szöveges megjelenítése
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return String.Format("(ID: {0}, Vertex:{1}, Normal:{2}, Text:{3})", id, vertex, normal, text);
        }

        /// <summary>
        /// Két csomópont egyenlő, ha a tulajdonságaik azonosak
        /// </summary>
        /// <param name="other">Összehasonlítandó csomópont</param>
        /// <returns>Ha megegyeznek akkor (true) egyébként (false)</returns>
        public bool Equals(Node other)
        {
            return ((this.id == other.ID) && (this.vertex == other.Vertex) && (this.normal == other.Normal) && (this.linkedFaces == other.LinkedFaces));
        }

        #endregion
    }
}
