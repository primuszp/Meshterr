using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Meshterr
{
    public partial class DrawingArea : UserControl
    {
        #region Private Members

        private bool loaded=false;
        private bool fresh = true;
        private float zoom = 0.5f;
        private float rotx = 0.0f;
        private float roty = 0.0f;
        private float znear = -1f;
        private float zfar  = +1f;

        private double lastdx = 0;
        private double lastdy = 0;
        private double mouseX = 0;
        private double mouseY = 0;
        private double screenX = 0;
        private double screenY = 0;

        private int mouseScreenX = 0;
        private int mouseScreenY = 0;
        private int lastix = 0;
        private int lastiy = 0;

        private double[] projeMatrix = new double[16];
        private double[] modelMatrix = new double[16];
        private int[] viewportMatrix = new int[4];
        private Vector3 worldPosition = new Vector3();

        private List<IDisplayList> displayLists = new List<IDisplayList>();
        private RenderingType renderOptions = RenderingType.Shaded;

        #endregion

        #region Public Properties

        public float Zoom
        {
            get { return zoom; }
            set { zoom = value; }
        }

        public float zNear
        {
            get { return  znear; }
            set { znear = value; }
        }

        public float zFar
        {
            get { return zfar;  }
            set { zfar = value; }
        }

        /// <summary>
        /// Megjelenítés módja
        /// </summary>
        public RenderingType RenderOptions
        {
            get { return renderOptions; }
            set
            {
                renderOptions = value;
                // A Display lista legenerálása
                Regenerate();
                // Megjelenítés
                Render();
            }
        }

        /// <summary>
        /// A megjelenítendő objektumok listája
        /// </summary>
        public IList<IDisplayList> DisplayLists
        {
            get { return displayLists; }
        }

        /// <summary>
        /// Objektum hozzáadása a megjelenítési listához
        /// </summary>
        /// <param name="obj">Az IDisplayList interface-t megvalósító objektum</param>
        public void AddObject(IDisplayList obj)
        {
            EnsureContext();
            obj.Regenerate(RenderOptions);
            DisplayLists.Add(obj);
            fresh = true;
            glControl.Invalidate();
        }

        /// <summary>
        /// Objektum törlése a megjelenítési listáról
        /// </summary>
        /// <param name="obj">Az IDisplayList interface-t megvalósító objektum</param>
        public void DeleteObject(IDisplayList obj)
        {
            if (DisplayLists.Contains(obj))
            {
                DisplayLists.Remove(obj);
                fresh = true;
            }
        }

        public Vector3 WorldPosition
        {
            get
            {
                return (worldPosition);
            }
        }

        #endregion

        #region Constructors

        public DrawingArea()
        {
            InitializeComponent();
        }

        #endregion

        public void FitToModel(double modelWidth, double modelHeight)
        {
            if (modelWidth <= 0 || modelHeight <= 0 || glControl.Width <= 0 || glControl.Height <= 0)
            {
                return;
            }

            const double padding = 1.1;
            zoom = (float)Math.Min(glControl.Width / (modelWidth * padding), glControl.Height / (modelHeight * padding));

            if (zoom > 10000.0f)
            {
                zoom = 10000.0f;
            }
            else if (zoom < 0.005f)
            {
                zoom = 0.005f;
            }

            double viewWidth = glControl.Width / zoom;
            double viewHeight = glControl.Height / zoom;
            screenX = -viewWidth / 2.0;
            screenY = -viewHeight / 2.0;
            fresh = true;
        }

        private void EnsureContext()
        {
            if (!loaded)
            {
                return;
            }

            glControl.MakeCurrent();
        }

        private void Reshape()
        {
            if (!loaded)
            {
                return;
            }

            EnsureContext();

            if (glControl.ClientSize.Height == 0)
            {
                glControl.ClientSize = new Size(glControl.ClientSize.Width, 1);
            }

            float wc = (glControl.Width  - 1.0f) / zoom;
            float hc = (glControl.Height - 1.0f) / zoom;
            float pc = 0.5f / zoom;

            GL.Viewport(0, 0, glControl.Width, glControl.Height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            GL.Ortho(screenX - pc, screenX + wc + pc, screenY - pc, screenY + hc + pc, znear, zfar);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        private void Render()
        {
            if (!loaded)
            {
                return;
            }

            EnsureContext();

            GL.ClearColor(Color.LightSkyBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.PushMatrix();

            GL.Rotate(rotx, 1f, 0f, 0f);
            GL.Rotate(roty, 0f, 0f, 1f);

            //Draw grid
            GL.LineWidth(1);
            //GL.Color3(Color.White);

            //GL.Begin(BeginMode.Lines);
            //for (int i = -10; i <= 10; ++i)
            //{
            //    GL.Vertex3(i, -10, 0);
            //    GL.Vertex3(i, 10, 0);
            //    GL.Vertex3(10, i, 0);
            //    GL.Vertex3(-10, i, 0);
            //}
            //GL.End();

            foreach (IDisplayList Item in DisplayLists)
            {
                Item.Call();
            }

            //GL.PopMatrix();

            glControl.SwapBuffers();
        }

        private void Redisplay()
        {
            Reshape();
            Render();
        }

        private void glControl_Load(object sender, EventArgs e)
        {
            glControl.MakeCurrent();
            loaded = true;
            GL.Enable(EnableCap.DepthTest);
            Redisplay();
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            if (!loaded) return;
            Redisplay();
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded) return;
            Redisplay();
        }

        private void glControl_MouseWheel(object sender, MouseEventArgs e)
        {
            float temp_zoom = zoom;

            if (e.Button == MouseButtons.None)
            {
                if (e.Delta > 0)
                {
                    zoom = zoom * 1.2f;

                    if (zoom > 10000.0f)
                    {
                        zoom = 10000.0f;
                    }
                }
                else
                {
                    zoom = zoom / 1.2f;

                    if (zoom < 0.005f)
                    {
                        zoom = 0.005f;
                    }
                }

                screenX = mouseX - (mouseX - screenX) * (temp_zoom / zoom);
                screenY = mouseY - (mouseY - screenY) * (temp_zoom / zoom);

                fresh = true;
            }
        }

        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            lastdx = mouseX;
            lastdy = mouseY;
        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            int diffx = e.X - lastix;
            int diffy = e.Y - lastiy;

            lastix = e.X;
            lastiy = e.Y;

            if (e.Button == MouseButtons.None)
            {
                mouseScreenX = e.X;
                mouseScreenY = glControl.Height - e.Y - 1;
                mouseX = screenX + mouseScreenX / zoom;
                mouseY = screenY + mouseScreenY / zoom;
                GetModellCoords(e);
            }
            else if (e.Button == MouseButtons.Right)
            {
                screenX = lastdx - e.X / zoom;
                screenY = lastdy - (glControl.Height - e.Y) / zoom;
                fresh = true;
            }
            else
                if (e.Button == MouseButtons.Left)
                {
                    rotx += 0.5f * diffy;
                    roty += 0.5f * diffx;
                    fresh = true;
                }
        }

        /// <summary>
        /// OpenGL mátrixok lekérdezése
        /// </summary>
        private void GetOpenGLMatrices()
        {
            GL.GetDouble(GetPName.ProjectionMatrix, projeMatrix);
            GL.GetDouble(GetPName.ModelviewMatrix, modelMatrix);
            GL.GetInteger(GetPName.Viewport, viewportMatrix);
        }

        /// <summary>
        /// Világkoordináta lekérdezése
        /// </summary>
        /// <param name="e">MouseEventArgs</param>
        public void GetModellCoords(MouseEventArgs e)
        {
            if (!loaded)
            {
                return;
            }

            float[] pixel = new float[1];

            //Mátrixok lekérdezése
            GetOpenGLMatrices();

            GL.ReadPixels(e.X, viewportMatrix[3] - e.Y, 1, 1, PixelFormat.DepthComponent, PixelType.Float, pixel);

            Vector3 win = new Vector3(e.X, viewportMatrix[3] - e.Y, pixel[0]);

            GLMath.UnProject(win, modelMatrix, projeMatrix, viewportMatrix, out worldPosition);
        }

        /// <summary>
        /// Regenerate all the display list objects using the new rendering options.
        /// </summary>
        private void Regenerate()
        {
            EnsureContext();

            foreach (IDisplayList item in displayLists)
            {
                item.Regenerate(renderOptions);
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (fresh)
            {
                Redisplay();
                fresh = false;
            }
        }
    }
}
