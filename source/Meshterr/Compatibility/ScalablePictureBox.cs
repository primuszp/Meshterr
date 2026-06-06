using System.Drawing;
using System.Windows.Forms;

namespace QAlbum
{
    public class ScalablePictureBox : PictureBox
    {
        public ScalablePictureBox()
        {
            SizeMode = PictureBoxSizeMode.Zoom;
        }

        public Image Picture
        {
            get { return Image; }
            set { Image = value; }
        }
    }
}
