using System;
using System.ComponentModel;
using System.Globalization;

namespace Meshterr
{
    public class BoundingBox
    {
        #region Private Member Variables

        private double x;
        private double y;

        private double width;
        private double height;

        #endregion

        #region Public Properties

        public static readonly BoundingBox Empty;

        public double X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        public double Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }

        public double Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        public double Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }

        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                if (this.Width > 0f)
                {
                    return (this.Height <= 0f);
                }
                return true;
            }
        }

        [Browsable(false)]
        public double Top
        {
            get
            {
                return this.Y;
            }
        }
 
        [Browsable(false)]
        public double Bottom
        {
            get
            {
                return (this.Y + this.Height);
            }
        }

        [Browsable(false)]
        public double Left
        {
            get
            {
                return this.X;
            }
        }

        [Browsable(false)]
        public double Right
        {
            get
            {
                return (this.X + this.Width);
            }
        }

        [Browsable(false)]
        public Vector3d Location
        {
            get
            {
                return new Vector3d(this.X, this.Y);
            }
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }

        #endregion

        #region Constructors

        public BoundingBox(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        #endregion

        #region Contains

        public bool Contains(Vector3d point)
        {
            return this.Contains(point.X, point.Y);
        }

        public bool Contains(BoundingBox bb)
        {
            return ((((this.X <= bb.X) && ((bb.X + bb.Width) <= (this.X + this.Width))) && (this.Y <= bb.Y)) && ((bb.Y + bb.Height) <= (this.Y + this.Height)));
        }

        public bool Contains(double x, double y)
        {
            return ((((this.X <= x) && (x < (this.X + this.Width))) && (this.Y <= y)) && (y < (this.Y + this.Height)));
        }

        #endregion

        #region Intersect

        public void Intersect(BoundingBox bb)
        {
            BoundingBox ef = Intersect(bb, this);
            this.X = ef.X;
            this.Y = ef.Y;
            this.Width = ef.Width;
            this.Height = ef.Height;
        }

        public static BoundingBox Intersect(BoundingBox a, BoundingBox b)
        {
            double x = Math.Max(a.X, b.X);
            double y = Math.Max(a.Y, b.Y);

            double mina = Math.Min((a.X + a.Width), (b.X + b.Width));
            double minb = Math.Min((a.Y + a.Height), (b.Y + b.Height));

            if ((mina >= x) && (minb >= y))
            {
                return new BoundingBox(x, y, mina - x, minb - y);
            }
            else
            {
                return Empty;
            }
        }

        public bool IntersectsWith(BoundingBox bb)
        {
            return ((((bb.X < (this.X + this.Width)) && (this.X < (bb.X + bb.Width))) && (bb.Y < (this.Y + this.Height))) && (this.Y < (bb.Y + bb.Height)));
        }

        #endregion

        #region Offset

        public void Offset(Vector3d offset)
        {
            this.Offset(offset.X, offset.Y);
        }

        public void Offset(double x, double y)
        {
            this.X += x;
            this.Y += y;
        }

        #endregion

        #region Public Static Methods

        public static BoundingBox FromLTRB(double left, double top, double right, double bottom)
        {
            return new BoundingBox(left, top, right - left, bottom - top);
        }

        public static BoundingBox Union(BoundingBox a, BoundingBox b)
        {
            double x = Math.Min(a.X, b.X);
            double y = Math.Min(a.Y, b.Y);
            double maxa = Math.Max((a.X + a.Width), (b.X + b.Width));
            double maxb = Math.Max((a.Y + a.Height), (b.Y + b.Height));
            return new BoundingBox(x, y, maxa - x, maxb - y);
        }

        public static bool operator ==(BoundingBox left, BoundingBox right)
        {
            return ((((left.X == right.X) && (left.Y == right.Y)) && (left.Width == right.Width)) && (left.Height == right.Height));
        }

        public static bool operator !=(BoundingBox left, BoundingBox right)
        {
            return !(left == right);
        }

        #endregion

        #region Public Override Methods

        public override bool Equals(object obj)
        {
            if (!(obj is BoundingBox))
            {
                return false;
            }
            BoundingBox bb = (BoundingBox)obj;
            return ((((bb.X == this.X) && (bb.Y == this.Y)) && (bb.Width == this.Width)) && (bb.Height == this.Height));
        }

        public override int GetHashCode()
        {
            return (int)(((((uint)this.X) ^ ((((uint)this.Y) << 13) | (((uint)this.Y) >> 0x13))) ^ ((((uint)this.Width) << 0x1a) | (((uint)this.Width) >> 6))) ^ ((((uint)this.Height) << 7) | (((uint)this.Height) >> 0x19)));
        }

        public override string ToString()
        {
            return ("{X=" + this.X.ToString(CultureInfo.CurrentCulture) + ",Y=" + this.Y.ToString(CultureInfo.CurrentCulture) + ",Width=" + this.Width.ToString(CultureInfo.CurrentCulture) + ",Height=" + this.Height.ToString(CultureInfo.CurrentCulture) + "}");
        }

        #endregion
    }
}