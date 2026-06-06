using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GRV11;
using System.Drawing;

namespace MeshTopology
{
    public partial class FormMain : Form
    {
        private GR gr;
        private bool fresh = false;

        private float rotx = 0.0f;
        private float roty = 0.0f;
        private int lastx = 0;
        private int lasty = 0;
        private Vector camera = new Vector(0, 0, 0);
        private double zoom = 0.1;
        private float zNear = -18000f;
        private float zFar = +18000f;
        int mouseX;
        int mouseY;

        Mesh mesh = null;
        RSG rsg = null;

        double[] projMatrix = new double[16];
        double[] modelMatrix = new double[16];
        int[] viewport = new int[4];
        double ox;
        double oy;
        double oz;
        double px1, py1, pz1;
        double px2, py2, pz2;


        public FormMain()
        {
            InitializeComponent();

            //mesh = new Mesh();
            //mesh.LoadPoints("Tereppontok.txt");
        }


        private Vector ScreenToWorld(int x, int y)
        {
            return new Vector((float)(x - grControl.Width / 2) * zoom + camera.X, -(float)(y - grControl.Height / 2) * zoom + camera.Y);
        }

        private Vector WorldToScreen(float x, float y)
        {
            return new Vector((int)((x - camera.X) / zoom) + grControl.Width / 2, -(int)((y - camera.Y) / zoom) + grControl.Height / 2);
        }


        private void Display()
        {
            gr.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            gr.glClear(GR.GL_COLOR_BUFFER_BIT | GR.GL_DEPTH_BUFFER_BIT);

            gr.glLoadIdentity();

            gr.glRotatef(rotx, 1, 0, 0);
            gr.glRotatef(roty, 0, 0, 1);

            // draw grid
            gr.glLineWidth(1);
            gr.glBegin(GR.GL_LINES);
            gr.glColor3f(1, 1, 1);
            for (int i = -10; i <= 10; ++i)
            {
                gr.glVertex3f(i, -10, 0);
                gr.glVertex3f(i, 10, 0);
                gr.glVertex3f(10, i, 0);
                gr.glVertex3f(-10, i, 0);
            }
            gr.glEnd();

             //mesh.Draw(gr);
            rsg.DrawList(gr);
            gr.glCallList(2);
            gr.glCallList(3);

            gr.SwapBuffers(grControl.GetHDC());
        }

        private void Reshape()
        {
            if (gr != null)
            {
                gr.glViewport(0, 0, grControl.Width, grControl.Height);
                gr.glMatrixMode(GR.GL_PROJECTION);
                gr.glLoadIdentity();

                double W = (grControl.Width) * zoom / 2.0;
                double H = (grControl.Height) * zoom / 2.0;

                gr.glOrtho(camera.X - W, camera.X + W, camera.Y - H, camera.Y + H, zNear, zFar);

                gr.glMatrixMode(GR.GL_MODELVIEW);
                gr.glLoadIdentity();
            }
        }

        private void Motion(int x, int y, MouseButtons mb)
        {
            Vector delta = ScreenToWorld(x, y) - ScreenToWorld(lastx, lasty);

            int diffx = x - lastx;
            int diffy = y - lasty;

            lastx = x;
            lasty = y;

            if (mb == MouseButtons.Middle)
            {
                camera -= delta;
                fresh = true;
            }
            else
                if (mb == MouseButtons.Left)
                {
                    rotx += (float)0.5f * diffy;
                    roty += (float)0.5f * diffx;
                    fresh = true;
                }
                else
                    if (mb == MouseButtons.Right)
                    {
                        camera -= delta;
                        fresh = true;
                    }
        }

        private void grControl_OpenGLStarted(GRControl sender)
        {
            gr = grControl.GetGR();

            gr.glEnable(GR.GL_DEPTH_TEST);
            gr.glEnable(GR.GL_COLOR_MATERIAL);
            gr.glEnable(GR.GL_LIGHTING);
            gr.glEnable(GR.GL_LIGHT0);
            gr.glEnable(GR.GL_NORMALIZE);
            gr.glShadeModel(GR.GL_SMOOTH);
            Reshape();

            //mesh.Trinagulation(gr);
            rsg = new RSG();
            rsg.Terep(gr);
        }

        private void grControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            double zoom_p = zoom;

            if (e.Delta > 0)
            {
                if (zoom > float.Epsilon * 1000.0f)
                    zoom /= 1.1f;
            }
            else
            {
                if (zoom < float.MaxValue / 1000.0f)
                    zoom *= 1.1f;
            }

            Vector mouse = ScreenToWorld(e.X, e.Y);

            camera.X = mouse.X - (mouse.X - camera.X) * (zoom / zoom_p);
            camera.Y = mouse.Y + (camera.Y - mouse.Y) * (zoom / zoom_p);

            fresh = true;
        }

        private void grControl_Paint(object sender, PaintEventArgs e)
        {
            Display();
        }

        private void grControl_Resize(object sender, EventArgs e)
        {
            Reshape();
        }

        private void grControl_MouseMove(object sender, MouseEventArgs e)
        {
            Motion(e.X, e.Y, e.Button);
            mouseX = e.X;
            mouseY = e.Y;

            float[] pixel = new float[1];

            if (gr != null)
            {
                //mátrixok lekérdezése
                gr.glGetDoublev(GR.GL_PROJECTION_MATRIX, projMatrix);
                gr.glGetDoublev(GR.GL_MODELVIEW_MATRIX, modelMatrix);
                gr.glGetIntegerv(GR.GL_VIEWPORT, viewport);

                gr.glReadPixels(e.X , grControl.Height - e.Y, 1, 1, GR.GL_DEPTH_COMPONENT, GR.GL_FLOAT, pixel);
                gr.gluUnProject(e.X, grControl.Height - e.Y , pixel[0], modelMatrix, projMatrix, viewport, ref px2, ref py2, ref pz2);

                double ox = Math.Round(px2, 3);
                double oy = Math.Round(py2, 3);
                double oz = Math.Round(pz2, 3);

               /// Vector ZZ = rsg.GetHeight(px2, py2);

                //gr.glNewList(3, GR.GL_COMPILE);
                //gr.glColor3f(1.0f, 0.0f, 0.0f);
                //IntPtr quadric = gr.gluNewQuadric();
                //gr.glPushMatrix();
                //gr.glTranslated(ZZ.X, ZZ.Y, ZZ.Z);
                //gr.gluSphere(quadric, 10, 16, 10);
                //gr.glPopMatrix();
                //gr.glEndList();

                stLabel.Text = "X: " + ox.ToString() + " " + "Y: " + oy.ToString() + " " + "Z: " + oz.ToString() + " ";
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
         

             

            //Mesh mesh = new Mesh();
            //Terrain terrain = new Terrain();

            //terrain.LoadHeightData();

            //List<Face> lapok = mesh.Triangulation(terrain.getData());

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (fresh)
            {
                Reshape();
                grControl.Refresh();
                fresh = false;
            }
        }


        private void grControl_DoubleClick(object sender, EventArgs e)
        {
            float[] pixel = new float[1];


            //mátrixok lekérdezése
            gr.glGetDoublev(GR.GL_PROJECTION_MATRIX, projMatrix);
            gr.glGetDoublev(GR.GL_MODELVIEW_MATRIX, modelMatrix);
            gr.glGetIntegerv(GR.GL_VIEWPORT, viewport);

            gr.glReadPixels(mouseX, grControl.Height - mouseY, 1, 1, GR.GL_DEPTH_COMPONENT, GR.GL_FLOAT, pixel);

            //gr.gluUnProject(e.X, grControl.Height - e.Y, 0, modelMatrix, projMatrix, viewport, ref px1, ref py1, ref pz1);
            //gr.gluUnProject(e.X, grControl.Height - e.Y, 1, modelMatrix, projMatrix, viewport, ref px2, ref py2, ref pz2);
            gr.gluUnProject(mouseX, grControl.Height - mouseY, pixel[0], modelMatrix, projMatrix, viewport, ref px2, ref py2, ref pz2);


            //double dx = px1 - px2;
            //double dy = py1 - py2;
            //double dz = pz1 - pz2;
            //double sc = py2 / dy;
            //double ox = px2 - sc * dx;
            //double oy = py2 - sc * dy;
            //double oz = pz2 - sc * dz;

            double ox = Math.Round(px2, 2);
            double oy = Math.Round(py2, 2);
            double oz = Math.Round(pz2, 2);

            stLabel.Text = "X: " + ox.ToString() + " " + "Y: " + oy.ToString() + " " + "Z: " + oz.ToString();

            gr.glNewList(2, GR.GL_COMPILE);
            gr.glColor3f(1.0f, 0.0f, 0.0f);
            IntPtr quadric = gr.gluNewQuadric();
            gr.glPushMatrix();
            gr.glTranslated(ox, oy, oz);
            gr.gluSphere(quadric, 50, 16, 10);
            gr.glPopMatrix();
            gr.glEndList();
            fresh = true;
        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            if (lbRow.Items.Count == 0 && lbColumn.Items.Count == 0)
            {
                for (int x = 0; x < 100; x++)
                {
                    lbRow.Items.Add(x);
                }
                for (int y = 0; y < 180; y++)
                {
                    lbColumn.Items.Add(y);
                }
            }
        }

        private void lbRow_SelectedIndexChanged(object sender, EventArgs e)
        {
            Point[] points = rsg.RowSection(lbRow.SelectedIndex, pbSorMetszet.ClientRectangle);

            Image controlBitmap = new Bitmap(pbSorMetszet.Width, pbSorMetszet.Height, this.CreateGraphics());
            Graphics controlDC = Graphics.FromImage(controlBitmap);
            controlDC.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            controlDC.DrawLines(Pens.Black, points);

            pbSorMetszet.Image = controlBitmap;
        }

        private void lbColumn_SelectedIndexChanged(object sender, EventArgs e)
        {
            Point[] points = rsg.ColumnSection(lbColumn.SelectedIndex, pbOszlopMetszet.ClientRectangle);

            Image controlBitmap = new Bitmap(pbOszlopMetszet.Width, pbOszlopMetszet.Height, this.CreateGraphics());
            Graphics controlDC = Graphics.FromImage(controlBitmap);
            controlDC.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            controlDC.DrawLines(Pens.Black, points);

            pbOszlopMetszet.Image = controlBitmap;
        }

        private void grControl_MouseDown(object sender, MouseEventArgs e)
        {

        }

  


    }
}
