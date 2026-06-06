using System;
using System.Runtime.Serialization;

namespace MeshTopology
{
    [Serializable()]
    class Face : IEquatable<Face>
    {
        #region Private Member Variables

        /// <summary>
        /// Index a háromszög első csomópontjára
        /// </summary>
        private int indexToNode1;

        /// <summary>
        /// Index a háromszög második csomópontjára
        /// </summary>
        private int indexToNode2;

        /// <summary>
        /// Index a háromszög harmadik csomópontjára
        /// </summary>
        private int indexToNode3;

        /// <summary>
        /// Index a háromszög első szomszédjára
        /// </summary>
        private int indexToFace1;

        /// <summary>
        /// Index a háromszög második szomszédjára
        /// </summary>
        private int indexToFace2;

        /// <summary>
        /// Index a háromszög harmadik szomszédjára
        /// </summary>
        private int indexToFace3;

        /// <summary>
        /// A háromszög normálvektora
        /// </summary>
        private Vector faceNormal;

        #endregion

        #region Constructors

        public Face()
            : this(-1, -1, -1)
        {
        }

        public Face(int indexToNode1, int indexToNode2, int indexToNode3)
        {
            this.indexToNode1 = indexToNode1;
            this.indexToNode2 = indexToNode2;
            this.indexToNode3 = indexToNode3;
        }

        public Face(int[] nodes)
        {
            this.indexToNode1 = nodes[0];
            this.indexToNode2 = nodes[1];
            this.indexToNode3 = nodes[2];
        }

        public Face(Face face)
        {
            this.indexToNode1 = face.IndexToNode1;
            this.indexToNode2 = face.IndexToNode2;
            this.indexToNode3 = face.IndexToNode3;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Index a háromszög első csomópontjára (tulajdonság)
        /// </summary>
        public int IndexToNode1
        {
            get { return indexToNode1; }
            set { indexToNode1 = value; }
        }

        /// <summary>
        /// Index a háromszög második csomópontjára (tulajdonság)
        /// </summary>
        public int IndexToNode2
        {
            get { return indexToNode2; }
            set { indexToNode2 = value; }
        }

        /// <summary>
        /// Index a háromszög harmadik csomópontjára (tulajdonság)
        /// </summary>
        public int IndexToNode3
        {
            get { return indexToNode3; }
            set { indexToNode3 = value; }
        }

        /// <summary>
        /// A háromszög csomópontjainak tulajdonsága                      
        /// </summary>         
        /// <param name="index">Index a háromszög csomópontjaira: index [0] -> 1st, [1] -> 2nd, [2] -> 3rd</param>
        /// <returns>Visszatér a csomópont indexével</returns>
        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: { return indexToNode1; }
                    case 1: { return indexToNode2; }
                    case 2: { return indexToNode3; }
                    default: throw new ArgumentException(THREE_COMPONENTS, "index");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: { indexToNode1 = value; break; }
                    case 1: { indexToNode2 = value; break; }
                    case 2: { indexToNode3 = value; break; }
                    default: throw new ArgumentException(THREE_COMPONENTS, "index");
                }
            }
        }

        /// <summary>
        /// A háromszög normálvektora
        /// </summary>
        public Vector FaceNormal
        {
            get { return faceNormal; }
            set { faceNormal = value; }
        }

        #endregion

        public override string ToString()
        {
            return ("[" + indexToNode1 + ", " + indexToNode2 + ", " + indexToNode3 + "]");
        }

        public Face Clone()
        {
            return (this);
        }

        /// <summary>
        /// Két háromszög azonos, ha ugyanazok a csomópontok alkotják öket
        /// </summary>
        /// <param name="other">Összehasonlítandó háromszög</param>
        /// <returns>Ha megegyeznek akkor (true) egyébként (false)</returns>
        public bool Equals(Face other)
        {
            return ((this.indexToNode1 == other.IndexToNode1) && (this.indexToNode2 == other.IndexToNode2) && (this.indexToNode3 == other.IndexToNode3));
        }

        #region Messages

        /// <summary>
        /// Az üzenet megjelenítésre kerül, ha nem megfelelő nagyságú mutatót alkalmazunk
        /// </summary>
        private const string THREE_COMPONENTS = "A mutató nem lehet nagyobb mint a háromszög csúcspontjainak száma!";

        #endregion

        //#region ISerializable Members
        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("nodes", nodes);
        //    info.AddValue("Edges", edges);
        //}
        //public Face(SerializationInfo info, StreamingContext context)
        //{
        //    nodes = (int[])info.GetValue("nodes", typeof(int[]));
        //    edges = (int[])info.GetValue("Edges", typeof(int[]));
        //}
        //#endregion
    }
}
