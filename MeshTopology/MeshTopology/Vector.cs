using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace MeshTopology
{
    [Serializable()]
    sealed class Vector : IComparable, IComparable<Vector>, IEquatable<Vector>, ISerializable
    {
        #region Private Member Variables

        /// <summary>
        /// A vektor X komponense
        /// /// </summary>
        private double x;

        /// <summary>
        /// A vektor Y komponense
        /// </summary>
        private double y;

        /// <summary>
        /// A vektor Z komponense
        /// </summary>
        private double z;

       /// <summary>
       /// A vektor hossza
       /// </summary>
        private double magnitude;

        #endregion

        #region Public Properties

        /// <summary>
        /// A vektor x komponensének tulajdonsága
        /// </summary>
        public double X
        {
            get { return x; }
            set 
            { 
                x = value;
                magnitude = Lenght();
            }
        }

        /// <summary>
        ///  A vektor y komponensének tulajdonsága
        /// </summary>
        public double Y
        {
            get { return y; }
            set 
            { 
                y = value;
                magnitude = Lenght();
            }
        }

        /// <summary>
        ///  A vektor z komponensének tulajdonsága
        /// </summary>
        public double Z
        {
            get { return z; }
            set 
            { 
                z = value;
                magnitude = Lenght();
            }
        }

        /// <summary>
        /// Indexelő a vektor komponenseire: vektor index [0] -> X, [1] -> Y és [2] -> Z
        /// </summary>
        /// <param name="index">Az index egy mutató a vektor komponenseire</param>
        /// <exception cref="System.ArgumentException">
        /// Kivétel történik, ha a mutató nagyobb mint a vektor komponenseinek száma (3)
        /// </exception>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: { return x; }
                    case 1: { return y; }
                    case 2: { return z; }
                    default: throw new ArgumentException(THREE_COMPONENTS, "index");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: { x = value; break; }
                    case 1: { y = value; break; }
                    case 2: { z = value; break; }
                    default: throw new ArgumentException(THREE_COMPONENTS, "index");
                }
                magnitude = Lenght();
            }
        }

        /// <summary>
        /// Tulajdonság a vektor háromelemű tömbként való értelmezéséhez
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// Kivétel történik, ha nem háromelemű (x,y,z) tömböt adunk meg
        /// </exception> 
        [XmlIgnore]
        public double[] Array
        {
            get
            {
                return new double[] { x, y, z };
            }
            set
            {
                if (value.Length == 3)
                {
                    x = value[0];
                    y = value[1];
                    z = value[2];
                    magnitude = Lenght();
                }
                else
                {
                    throw new ArgumentException(THREE_COMPONENTS);
                }
            }
        }

        /// <summary>
        /// A vektor hossza, csak olvasható tulajdonság
        /// </summary>
        public double Magnitude 
        {
            get
            {
                return magnitude;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// A vektor összes komponense nulla lesz
        /// </summary>
        public Vector()
            : this(0, 0, 0)
        {
        }

        /// <summary>
        /// A vektor z komponense mindig nulla lesz
        /// </summary>
        /// <param name="x">A vektor x komponense</param>
        /// <param name="y">A vektor y komponense</param>
        public Vector(double x, double y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
            this.magnitude = this.Lenght();
        }

        /// <summary>
        /// A vektor összes komponense a megadot értéket veszi fel
        /// </summary>
        /// <param name="x">A vektor x komponense</param>
        /// <param name="y">A vektor y komponense</param>
        /// <param name="z">A vektor z komponense</param>
        public Vector(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.magnitude = this.Lenght();
        }

        /// <summary>
        /// A vektort háromelemű tömbként tároljuk: [x,y,z]
        /// </summary>
        /// <param name="xyz">A vektor komponenseit tartalmazó tömb</param>
        public Vector(double[] xyz)
        {
            if (xyz.Length == 3)
            {
                this.x = xyz[0];
                this.y = xyz[1];
                this.z = xyz[2];
                this.magnitude = this.Lenght();
            }
            else
            {
                throw new ArgumentException(THREE_COMPONENTS);
            }
        }

        /// <summary>
        /// A vektor komponensei a megadott vektor komponenseivel lesz egyenlő
        /// </summary>
        /// <param name="vector">A megadott vektor</param>
        public Vector(Vector vector)
        {
            this.x = vector.X;
            this.y = vector.Y;
            this.z = vector.Z;
            this.magnitude = this.Lenght();
        }

        #endregion

        #region Public Static Methods

        private static double DegreeToRadian(double angle)
        {
            return (Math.PI * angle / 180.0);
        }

        private static double RadianToDegree(double angle)
        {
            return (angle * (180.0 / Math.PI));
        }

        /// <summary>
        /// Két vektor vektoriális szorzata (crossproduct)
        /// </summary>
        /// <param name="vector1">Első összetevő</param>
        /// <param name="vector2">Második összetevő</param>
        /// <returns>Vektor</returns>
        /// <implementation>
        /// A vektoriális szorzás nem kommutatív
        /// </implementation>
        public static Vector CrossProduct(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.y * vector2.z - vector1.z * vector2.y, vector1.z * vector2.x - vector1.x * vector2.z, vector1.x * vector2.y - vector1.y * vector2.x);
        }

        /// <summary>
        /// Két vektor skaláris szorzata (dot product)
        /// </summary>
        /// <param name="vector1">Első összetevő</param>
        /// <param name="vector2">Második összetevő)</param>
        /// <returns>Skalár</returns>
        public static double DotProduct(Vector vector1, Vector vector2)
        {
            return (vector1.x * vector2.x + vector1.y * vector2.y + vector1.z * vector2.z);
        }

        /// <summary>
        /// Három vektor vegyes szorzata (mixed product)
        /// </summary>
        /// <param name="vector1">Első összetevő</param>
        /// <param name="vector2">Második összetevő</param>
        /// <param name="vector3">Harmadik összetevő</param>
        /// <returns>Skalár</returns>
        /// <implementation>
        /// A vegyes szorzat nem kommutatív
        /// </implementation>
        public static double MixedProduct(Vector vector1, Vector vector2, Vector vector3)
        {
            return DotProduct(CrossProduct(vector1, vector2), vector3);
        }

        /// <summary>
        /// Két vektor között értelmezett szög fokban
        /// </summary>
        /// <param name="vector1">Első összetevő</param>
        /// <param name="vector2">Második összetevő</param>
        /// <returns>Szög</returns>
        public static double Angle(Vector vector1, Vector vector2)
        {
            return (180.0 / Math.PI) * Math.Acos(Normalize(vector1) * Normalize(vector2));
        }

        /// <summary>
        /// Két vektor között értelmezett távolság
        /// </summary>
        /// <param name="vector1">Első összetevő</param>
        /// <param name="vector2">Második összetevő</param>
        /// <returns>Skalár</returns>
        public static double Distance(Vector vector1, Vector vector2)
        {
            return Math.Sqrt((vector1.x - vector2.x) * (vector1.x - vector2.x) + (vector1.y - vector2.y) * (vector1.y - vector2.y) + (vector1.z - vector2.z) * (vector1.z - vector2.z));
        }

        /// <summary>
        /// A vektor normáltja
        /// </summary>
        /// <param name="vector">Vektor</param>
        /// <returns>Normált vektor</returns>
        public static Vector Normalize(Vector vector)
        {
            if (vector.magnitude == 0.0)
            {
                return (new Vector(0, 0, 0));
            }
            else
            {
                return (new Vector(vector.x / vector.magnitude, vector.y / vector.magnitude, vector.z / vector.magnitude));
            }
        }

        /// <summary>
        /// Y-tengely körüli forgatás (Yaw)
        /// </summary>
        /// <param name="vector">Az elforgatandó vektor</param>
        /// <param name="degree">A forgatás szöge fokban</param>
        /// <returns>Az Y-tengely körül elforgatott vektor</returns>
        public static Vector Yaw(Vector vector, double degree)
        {
            double x = (vector.z * Math.Sin(degree)) + (vector.x * Math.Cos(degree));
            double y = vector.y;
            double z = (vector.z * Math.Cos(degree)) - (vector.x * Math.Sin(degree));
            return new Vector(x, y, z);
        }

        /// <summary>
        /// X-tengely körüli forgatás (Pitch)
        /// </summary>
        /// <param name="vector">Az elforgatandó vektor</param>
        /// <param name="degree">A forgatás szöge fokban</param>
        /// <returns>Az X-tengely körül elforgatott vektor</returns>
        public static Vector Pitch(Vector vector, double degree)
        {
            double x = vector.x;
            double y = (vector.y * Math.Cos(degree)) - (vector.z * Math.Sin(degree));
            double z = (vector.y * Math.Sin(degree)) + (vector.z * Math.Cos(degree));
            return new Vector(x, y, z);
        }

        /// <summary>
        /// Z-tengely körüli forgatás (Roll)
        /// </summary>
        /// <param name="Node">Az elforgatandó vektor</param>
        /// <param name="degree">A forgatás szöge fokban</param>
        /// <returns>A Z-tengely körül elforgatott vektor</returns>
        public static Vector Roll(Vector vector, double degree)
        {
            double x = (vector.x * Math.Cos(degree)) - (vector.y * Math.Sin(degree));
            double y = (vector.x * Math.Sin(degree)) + (vector.y * Math.Cos(degree));
            double z = vector.z;
            return new Vector(x, y, z);
        }

        #endregion

        #region Public Methods

        #region Operators

        /// <summary>
        /// Két vektor összege
        /// </summary>
        /// <param name="vector1">Első összetevő</param>
        /// <param name="vector2">Második összetevő</param>
        /// <returns>Összegvektor vagy eredő</returns>
        public static Vector operator +(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.x + vector2.x, vector1.y + vector2.y, vector1.z + vector2.z);
        }

        /// <summary>
        /// Két vektor különbsége
        /// </summary>
        /// <param name="vector1">Első összetevő</param>
        /// <param name="vector2">Második összetevő</param>
        /// <returns>Különbségvektor</returns>
        public static Vector operator -(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.x - vector2.x, vector1.y - vector2.y, vector1.z - vector2.z);
        }

        /// <summary>
        /// Vektor szorzása skalárral
        /// </summary>
        /// <param name="vector">Első összetevő</param>
        /// <param name="scalar">Második összetevő</param>
        /// <returns>Egy vektor skalárszorosa</returns>
        public static Vector operator *(Vector vector, double scalar)
        {
            return new Vector(vector.x * scalar, vector.y * scalar, vector.z * scalar);
        }

        /// <summary>
        /// Vektor szorzása skalárral
        /// </summary>
        /// <param name="scalar">Első összetevő</param>
        /// <param name="vector">Második összetevő</param>
        /// <returns>Egy vektor skalárszorosa</returns>
        public static Vector operator *(double scalar, Vector vector)
        {
            return (vector * scalar);
        }

        /// <summary>
        /// Vektor osztása skalárral
        /// </summary>
        /// <param name="vector">Első összetevő</param>
        /// <param name="scalar">Második összetevő</param>
        /// <returns>Egy vektor és skalár hányadosa</returns>
        public static Vector operator /(Vector vector, double scalar)
        {
            return new Vector(vector.x / scalar, vector.y / scalar, vector.z / scalar);
        }

        /// <summary>
        /// Vektor tükrözése
        /// </summary>
        /// <param name="vector">A vektor amit tükrözünk</param>
        /// <returns>A tükrözött vektor</returns>
        public static Vector operator -(Vector vector)
        {
            return new Vector(-vector.x, -vector.y, -vector.z);
        }

        /// <summary>
        /// Vektor megerősítése
        /// </summary>
        /// <param name="vector">A vektor amit megerősítünk</param>
        /// <returns>A megerősített vektor</returns>
        public static Vector operator +(Vector vector)
        {
            return new Vector(+vector.x, +vector.y, +vector.z);
        }

        /// <summary>
        /// Két vektor skaláris szorzata (dot product)
        /// </summary>
        /// <param name="vector1">Első összetevő</param>
        /// <param name="vector2">Második összetevő</param>
        /// <returns>Skalár</returns>
        public static double operator *(Vector vector1, Vector vector2)
        {
            return (vector1.x * vector2.x + vector1.y * vector2.y + vector1.z * vector2.z);
        }

        /// <summary>
        /// Két vektor vektoriális szorzata (crossproduct)
        /// </summary>
        /// <param name="vector1">Első összetevő</param>
        /// <param name="vector2">Második összetevő</param>
        /// <returns>Vektor</returns>
        /// <implementation>
        /// A vektoriális szorzás nem kommutatív
        /// </implementation>
        public static Vector operator %(Vector vector1, Vector vector2)
        {
            return new Vector(vector1.y * vector2.z - vector1.z * vector2.y, vector1.z * vector2.x - vector1.x * vector2.z, vector1.x * vector2.y - vector1.y * vector2.x);
        }

        #endregion
        
        #region Functions

        /// <summary>
        /// A meglévő vektornak egy eltérő vektorral képzett vektoriális szorzata (crossproduct)
        /// </summary>
        /// <param name="other">Vektor</param>
        /// <returns>Vektor</returns>
        public Vector CrossProduct(Vector other)
        {
            return CrossProduct(this, other);
        }

        /// <summary>
        /// A meglévő vektornak egy eltérő vektorral képzett skaláris szorzata (dot product)
        /// </summary>
        /// <param name="other">Vektor</param>
        /// <returns>Skalár</returns>
        public double DotProduct(Vector other)
        {
            return DotProduct(this, other);
        }

        /// <summary>
        /// A meglévő vektor és egy másik vektor között értelmezett szög fokban
        /// </summary>
        /// <param name="other">Eltérő vektor</param>
        /// <returns>Szög</returns>
        public double Angle(Vector other)
        {
            return Angle(this, other);
        }

        /// <summary>
        /// A meglévő vektor és egy másik vektor között értelmezett távolság
        /// </summary>
        /// <param name="other">A meglévő vektortól eltérő másik vektor</param>
        /// <returns>Skalár</returns>
        public double Distance(Vector other)
        {
            return Distance(this, other);
        }

        /// <summary>
        /// Egységvektor
        /// </summary>
        public void Normalize()
        {
            if (magnitude == 0.0)
            {
                return;
            }
            else
            {
                x = x / magnitude;
                y = y / magnitude;
                z = z / magnitude;
            }
        }

        /// <summary>
        /// A vektor minden komponense negatív lesz
        /// </summary>
        public void Negate()
        {
            this.x = -this.x;
            this.y = -this.y;
            this.z = -this.z;
        }

        /// <summary>
        /// Y-tengely körüli forgatás (Yaw)
        /// </summary>
        /// <param name="degree">A forgatás szöge fokban</param>
        /// <returns>Az Y-tengely körül elforgatott vektor</returns>
        public void Yaw(double degree)
        {
            x = Yaw(this, degree).x;
            y = Yaw(this, degree).y;
            z = Yaw(this, degree).z;
        }

         /// <summary>
        /// X-tengely körüli forgatás (Pitch)
        /// </summary>
        /// <param name="degree">A forgatás szöge fokban</param>
        /// <returns>Az X-tengely körül elforgatott vektor</returns>
        public void Pitch(double degree)
        {
            x = Pitch(this, degree).x;
            y = Pitch(this, degree).y;
            z = Pitch(this, degree).z;
        }

        /// <summary>
        /// Z-tengely körüli forgatás (Roll)
        /// </summary>
        /// <param name="degree">A forgatás szöge fokban</param>
        /// <returns>A Z-tengely körül elforgatott vektor</returns>
        public void Roll(double degree)
        {
            x = Roll(this, degree).x;
            y = Roll(this, degree).y;
            z = Roll(this, degree).z;
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// A vektor hosszát adja meg
        /// </summary>
        private double Lenght()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        #endregion

        #region Interface Implementations

        #region CompareTo

        /// <summary>
        /// A vektorok hossza alapján történik az összehasonlítás rendezéskor
        /// </summary>
        /// <param name="other">A vektor amivel összehasonlítjuk a példányt</param>
        /// <returns>
        /// +1, ha a példány vektor hossza nagybb  mint amivel összehasonlítjuk
        /// -1, ha a példány vektor hossza kisebb  mint amivel összehasonlítjuk
        ///  0, ha a példány vektor hossza egyenlő mint amivel összehasonlítjuk
        /// </returns>
        public int CompareTo(Vector other)
        {
            if (this < other)
            {
                return (-1);
            }
            
            if (this > other)
            {
                return (+1);
            }

            return (0);
        }

        public int CompareTo(Object obj)
        {
            if (obj is Vector)
            {
                return CompareTo((Vector)obj);
            }
            else
            {
                //Az obj nem Vector típus, kivétel dobással jelezzük
                throw new ArgumentException("Csak Vektor típus hasonlítható össze!");
            }
        }

        #endregion

        #region Equals

        public static bool Equals(Vector vector1, Vector vector2)
        {
            return ((vector1.X.Equals(vector2.X) && vector1.Y.Equals(vector2.Y)) && vector1.Z.Equals(vector2.Z));
        }

        public bool Equals(Vector other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is Vector))
            {
                return false;
            }
            Vector other = (Vector)obj;
            return Equals(this, other);
        }

        #endregion

        #region GetHashCode

        public override int GetHashCode()
        {
            return ((this.x.GetHashCode() ^ this.y.GetHashCode()) ^ this.z.GetHashCode());
        }

        #endregion

        #region Logic Operators

        /// <summary>
        /// Összehasonlítja két vektor hosszát (kisebb mint)
        /// </summary>
        /// <param name="v1">Vektor amit hasonlítunk</param>
        /// <param name="v2">Vektor amihez hasonlítunk</param>
        /// <returns>Az érték (true), ha v1 kisebb mint v2, egyébként (false)</returns>
        public static bool operator <(Vector v1, Vector v2)
        {
            return (v1.Magnitude < v2.Magnitude);
        }

        /// <summary>
        /// Összehasonlítja két vektor hosszát (nagyobb mint)
        /// </summary>
        /// <param name="v1">Vektor amit hasonlítunk</param>
        /// <param name="v2">Vektor amihez hasonlítunk</param>
        /// <returns>Az érték (true), ha v1 nagyobb mint v2, egyébként (false)</returns>
        public static bool operator >(Vector v1, Vector v2)
        {
            return (v1.Magnitude > v2.Magnitude);
        }

        /// <summary>
        /// Összehasonlítja két vektor hosszát (kisebb vagy egyenlő mint)
        /// </summary>
        /// <param name="v1">Vektor amit hasonlítunk</param>
        /// <param name="v2">Vektor amihez hasonlítunk</param>
        /// <returns>Az érték (true), ha v1 kisebb vagy egyenlő mint v2, egyébként (false)</returns>
        public static bool operator <=(Vector v1, Vector v2)
        {
            return (v1.Magnitude <= v2.Magnitude);
        }

        /// <summary>
        /// Összehasonlítja két vektor hosszát (nagyobb vagy egyenlő mint)
        /// </summary>
        /// <param name="v1">Vektor amit hasonlítunk</param>
        /// <param name="v2">Vektor amihez hasonlítunk</param>
        /// <returns>Az érték (true), ha v1 nagyobb vagy egyenlő mint v2, egyébként (false)</returns>
        public static bool operator >=(Vector v1, Vector v2)
        {
            return (v1.Magnitude >= v2.Magnitude);
        }

        public static bool operator ==(Vector me, Vector other)
        {
            return ((me.X == other.X) && (me.Y == other.Y) && (me.Z == other.Z));
        }

        public static bool operator !=(Vector me, Vector other)
        {
            return !(me == other);
        }

        #endregion

        #region ToString

        /// <summary>
        /// A vektor belső állapotának szöveges megjelenítése
        /// </summary>
        /// <returns>A vektort reprezentáló szöveg (string)</returns>
        public override string ToString()
        {
            return String.Format("({0},{1},{2})", x, y, z);
        }

        #endregion

        #endregion

        #region Static Values

        /// <summary>
        /// A tolarancia mértéke amin belül még két vektor azonosnak vehető
        /// </summary>
        private static double EqualityTolerence = Double.Epsilon;

        /// <summary>
        /// Az üzenet megjelenítésre kerül, ha nem megfelelő elemszámú tömböt alkalmazunk
        /// </summary>
        private static string THREE_COMPONENTS = "A tömbnek pontosan három komponenst kell tartalmaznia (x,y,z)";

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context) 
        {
            info.AddValue("X", x);
            info.AddValue("Y", y);
            info.AddValue("Z", z);
        }
        public Vector(SerializationInfo info, StreamingContext context) 
        {
            x = info.GetDouble("X");
            y = info.GetDouble("Y");
            z = info.GetDouble("Z");
        }

        #endregion
    }
}
