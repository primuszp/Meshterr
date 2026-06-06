using System;
using System.Windows.Forms;
using OpenTK.Mathematics;

namespace Meshterr
{
    public partial class FormMain : Form
    {
        private Rsg rsg = null;

        public FormMain()
        {
            InitializeComponent();
            drawingArea.glControl.MouseMove += new MouseEventHandler(glControl_MouseMove);
        }

        private void RsgLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FormRsgLoad fRsgLoad = new FormRsgLoad())
            {
                if (fRsgLoad.ShowDialog() == DialogResult.OK)
                {
                    rsg = fRsgLoad.Dem;

                    BezierPatch bp = new BezierPatch();
                    rsg = bp.GetBezierRsg(rsg, 2);

                    rsg.Distoration = rsg.IsImageBased ? 1 : 3;

                    CalcNearFar(rsg);
                    drawingArea.FitToModel(rsg.Box.Width, rsg.Box.Height);

                    drawingArea.RenderOptions = RenderingType.Shaded;
                    drawingArea.AddObject(rsg);
                    IsoLine isol = new IsoLine(rsg, 10);
                    drawingArea.AddObject(isol);
                }
            }
        }

        private void TinLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tin tin = new Tin();

            tin.Mesh.Nodes = FileFormatIO.LoadXYZ("tereppontok.txt");
            tin.Box = FileFormatIO.Box;
            tin.MaxElevation = FileFormatIO.MaxElevation;
            tin.MinElevation = FileFormatIO.MinElevation;
            tin.OffsetX = (int)(FileFormatIO.Box.Left + FileFormatIO.Box.Width / 2);
            tin.OffsetY = (int)(FileFormatIO.Box.Top - FileFormatIO.Box.Height / 2);
            tin.OffsetZ = (int)(FileFormatIO.MinElevation + (FileFormatIO.MaxElevation - FileFormatIO.MinElevation) / 2);
            tin.Distoration = 5;

            tin.Triangulation();

            CalcNearFar(tin);
            drawingArea.FitToModel(tin.Box.Width, tin.Box.Height);

            drawingArea.RenderOptions = RenderingType.Wireframe;
            drawingArea.AddObject(tin);


            IsoLine isol = new IsoLine(tin, 2);       
            drawingArea.AddObject(isol);
        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            statusStrip.Items[0].Text = Correct(drawingArea.WorldPosition, rsg).ToString();
        }

        private Vector3d Correct(Vector3 world, Rsg dem)
        {
            if (rsg != null)
            {
                double x = Math.Round(rsg.Eastings + world.X + dem.OffsetX, 2);
                double y = Math.Round(rsg.Northings + world.Y + dem.OffsetY, 2);
                double z = Math.Round((world.Z / dem.Distoration) + dem.OffsetZ, 2);

                return (new Vector3d(x, y, z));
            }
            else return Vector3d.Zero;
        }

        private void CalcNearFar(Rsg dem)
        {
            double a = dem.Box.Width;
            double b = dem.Box.Height;
            double z = Math.Abs(dem.MaxElevation - dem.MinElevation) * dem.Distoration;
            double c = Math.Sqrt(a * a + b * b + z * z);

            drawingArea.zFar = (float)(c / 2);
            drawingArea.zNear = -drawingArea.zFar;
        }

        private void CalcNearFar(Tin dem)
        {
            double a = dem.Box.Width;
            double b = dem.Box.Height;
            double z = Math.Abs(dem.MaxElevation - dem.MinElevation) * dem.Distoration;
            double c = Math.Sqrt(a * a + b * b + z * z);

            drawingArea.zFar = (float)(c / 2);
            drawingArea.zNear = -drawingArea.zFar;
        }
    }
}
