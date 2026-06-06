using System;
using System.Runtime.Serialization;

namespace MeshTopology
{
    [Serializable()]
    class Edge : ISerializable , IEquatable<Edge>
    {
        #region Private Member Variables

        /// <summary>
        /// Index az él kezdőpontjára (Start Node)
        /// </summary>
        private int snode;
        
        /// <summary>
        /// Index az él végpontjára (End Node)
        /// </summary>
        private int enode;

        #endregion

        #region Public properties

        /// <summary>
        /// Az él kezdőpontjának tulajdonsága (Start Node)
        /// </summary>
        public int Snode
        {
            get { return snode; }
            set { snode = value; }
        }

        /// <summary>
        /// Az él végpontjának tulajdonsága (End Node)
        /// </summary>
        public int Enode
        {
            get { return enode; }
            set { enode = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Az él indexei null értéket vesznek fel, a végtelenbe mutatnak
        /// </summary>
        public Edge()
            : this(-1, -1)
        {
        }

        /// <summary>
        /// Az él indexei a megadot csomópontokra mutatnak
        /// </summary>
        /// <param name="snode">Az él kezdőpontjára mutató index</param>
        /// <param name="enode">Az él végpontjára mutató index</param>
        public Edge(int snode, int enode)
        {
            this.enode = enode;
            this.snode = snode;
        }

        /// <summary>
        /// Az él indexei a megadot él csomópontjaira mutatnak
        /// </summary>
        /// <param name="edge">A megadott él</param>
        public Edge(Edge edge)
        {
            this.snode = edge.Snode;
            this.enode = edge.Enode;
        }

        #endregion

        #region Functions

        /// <summary>
        /// Az él lemásolása
        /// </summary>
        /// <returns>Edge</returns>
        public Edge Clone()
        {
            return (new Edge(snode, enode));
        }

        /// <summary>
        /// Az él tulajdonságainak szöveges megjelenítése
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return String.Format("({0},{1})", snode, enode);
        }

        /// <summary>
        /// Két él megegyezik, ha kezdő- és végpontjaik megegyeznek
        /// </summary>
        /// <param name="other">Összehasonlítandó él</param>
        /// <returns>Ha megegyeznek akkor (true) egyébként (false)</returns>
        public bool Equals(Edge other)
        {
            return ((this.enode == other.snode) && (this.snode == other.enode)) ||
                   ((this.enode == other.enode) && (this.snode == other.snode));
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Start Node", snode);
            info.AddValue("End Node", enode);
        }
        public Edge(SerializationInfo info, StreamingContext context)
        {
            snode = info.GetInt32("Start Node");
            enode = info.GetInt32("End Node");
        }

        #endregion
    }
}
