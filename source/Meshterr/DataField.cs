
namespace Meshterr
{
    class DataField
    {
        #region Private Members

        private float[,] field = null;
        private float maximum = 0.0f;
        private float minimum = 0.0f;

        #endregion

        #region Public Properties

        public float[,] Field
        {
            get { return field; }
            set { field = value; }
        }

        public float Maximum
        {
            get { return maximum; }
            set { maximum = value; }
        }

        public float Minimum
        {
            get { return minimum; }
            set { minimum = value; }
        }

        #endregion

        #region Constructors

        public DataField()
        {
            field = new float[1, 1];
        }

        public DataField(float[,] field, float minimum, float maximum)
        {
            this.field = field;
            this.minimum = minimum;
            this.maximum = maximum;
        }

        #endregion

        public float Normal(int x, int y)
        {
            return (field[x, y] / maximum);
        }
    }
}
