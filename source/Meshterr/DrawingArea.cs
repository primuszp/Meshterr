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

        private bool loaded = false;
        private bool fresh  = true;
        private float zoom  = 0.5f;
        private float rotx  = 0.0f;
        private float roty  = 0.0f;
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

        private double[] projeMatrix   = new double[16];
        private double[] modelMatrix   = new double[16];
        private int[]    viewportMatrix = new int[4];
        private Vector3  worldPosition  = new Vector3();

        // ── Forgatási rendszer ────────────────────────────────────────────────
        // Modelview = T(pivotEyeX, pivotEyeY, 0) · Rx(rotx) · Rz(roty) · T(−pivotModel)
        //
        // Bal klikknél:
        //   pivotEyeX/Y = az egér eye-space koordinátái  (= mouseX/mouseY)
        //   pivotModel  = az egér alatti 3D felszíni pont (depth-buffer unproject)
        //
        // Így pivotModel MINDIG a (pivotEyeX, pivotEyeY) screen-pozícióra vetül,
        // tehát bármilyen szögből, bármilyen újabb klikknél nincs vizuális ugrás.
        private float   pivotEyeX  = 0f;
        private float   pivotEyeY  = 0f;
        private Vector3 pivotModel = Vector3.Zero;

        private List<IDisplayList> displayLists  = new List<IDisplayList>();
        private RenderingType      renderOptions = RenderingType.Shaded;

        // ── ViewCube ──────────────────────────────────────────────────────────
        private int   vcHoveredFace  = -1;   // -1 = nincs hover
        private bool  vcAnimating    = false;
        private float vcAnimProgress = 1f;   // 0=start, 1=kész
        private float vcAnimStartRx, vcAnimStartRy;
        private float vcAnimTargetRx, vcAnimTargetRy;
        private const float VcAnimSteps = 25f;  // lépések száma (25 × 15 ms ≈ 375 ms)

        // ViewCube-ból kezdett bal-drag blokkolása
        private bool vcDragActive = false;

        #endregion

        #region Public Properties

        public float Zoom
        {
            get { return zoom; }
            set { zoom = value; }
        }

        public float zNear
        {
            get { return znear; }
            set { znear = value; }
        }

        public float zFar
        {
            get { return zfar; }
            set { zfar = value; }
        }

        public RenderingType RenderOptions
        {
            get { return renderOptions; }
            set
            {
                renderOptions = value;
                Regenerate();
                Render();
            }
        }

        public IList<IDisplayList> DisplayLists
        {
            get { return displayLists; }
        }

        public void AddObject(IDisplayList obj)
        {
            EnsureContext();
            obj.Regenerate(RenderOptions);
            DisplayLists.Add(obj);
            fresh = true;
            glControl.Invalidate();
        }

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
            get { return worldPosition; }
        }

        #endregion

        #region Constructors

        public DrawingArea()
        {
            InitializeComponent();
        }

        #endregion

        // ── Kamera ────────────────────────────────────────────────────────────

        public void FitToModel(double modelWidth, double modelHeight)
        {
            if (modelWidth <= 0 || modelHeight <= 0 || glControl.Width <= 0 || glControl.Height <= 0)
                return;

            const double padding = 1.1;
            zoom = (float)Math.Min(
                glControl.Width  / (modelWidth  * padding),
                glControl.Height / (modelHeight * padding));

            zoom = Math.Clamp(zoom, 0.005f, 10000.0f);

            double viewWidth  = glControl.Width  / zoom;
            double viewHeight = glControl.Height / zoom;
            screenX = -viewWidth  / 2.0;
            screenY = -viewHeight / 2.0;

            // Pivot visszaállítása a jelenet közepére
            pivotEyeX  = 0f;
            pivotEyeY  = 0f;
            pivotModel = Vector3.Zero;

            fresh = true;
        }

        private void EnsureContext()
        {
            if (loaded) glControl.MakeCurrent();
        }

        // ── OpenGL pipeline ───────────────────────────────────────────────────

        private void Reshape()
        {
            if (!loaded) return;

            EnsureContext();

            if (glControl.ClientSize.Height == 0)
                glControl.ClientSize = new Size(glControl.ClientSize.Width, 1);

            float wc = (glControl.Width  - 1.0f) / zoom;
            float hc = (glControl.Height - 1.0f) / zoom;
            float pc = 0.5f / zoom;

            GL.Viewport(0, 0, glControl.Width, glControl.Height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(screenX - pc, screenX + wc + pc,
                     screenY - pc, screenY + hc + pc,
                     znear, zfar);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        private void Render()
        {
            if (!loaded) return;

            EnsureContext();

            GL.ClearColor(Color.LightSkyBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            // T(pivotEye) · Rx · Rz · T(−pivotModel)
            GL.Translate(pivotEyeX, pivotEyeY, 0f);
            GL.Rotate(rotx, 1f, 0f, 0f);
            GL.Rotate(roty, 0f, 0f, 1f);
            GL.Translate(-pivotModel.X, -pivotModel.Y, -pivotModel.Z);

            foreach (IDisplayList item in DisplayLists)
                item.Call();

            // ── ViewCube renderelése ──────────────────────────────────────────
            ViewCube.Render(glControl.Width, glControl.Height, rotx, roty, vcHoveredFace);

            glControl.SwapBuffers();
        }

        private void Redisplay()
        {
            Reshape();
            Render();
        }

        // ── Eseménykezelők ────────────────────────────────────────────────────

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
            if (e.Button != MouseButtons.None) return;

            float oldZoom = zoom;

            if (e.Delta > 0)
                zoom = Math.Min(zoom * 1.2f, 10000.0f);
            else
                zoom = Math.Max(zoom / 1.2f, 0.005f);

            // Zoom az egér pozíciója körül (screenX/Y korrekció)
            screenX = mouseX - (mouseX - screenX) * (oldZoom / zoom);
            screenY = mouseY - (mouseY - screenY) * (oldZoom / zoom);

            fresh = true;
        }

        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            lastdx = mouseX;
            lastdy = mouseY;
            vcDragActive = false;

            if (e.Button == MouseButtons.Left)
            {
                // ── ViewCube kattintás ────────────────────────────────────────
                int hitFace = ViewCube.HitTest(e.X, e.Y,
                                               glControl.Width, glControl.Height,
                                               rotx, roty);
                if (hitFace >= 0)
                {
                    vcDragActive = true;  // drag ne induljon el a ViewCube-ból
                    VcSnapToFace(hitFace);
                    return;
                }

                // ── Normál pivot frissítése ───────────────────────────────────
                // T(pivotEye)·Rx·Rz·T(−pivotModel) · pivotModel = (pivotEyeX, pivotEyeY, 0)
                // → a pivot screen-pozíciója MINDIG az egér aktuális pozíciója → nincs ugrás.
                int sy = glControl.Height - e.Y - 1;

                EnsureContext();
                GetOpenGLMatrices();

                float[] depth = new float[1];
                GL.ReadPixels(e.X, sy, 1, 1,
                              PixelFormat.DepthComponent, PixelType.Float, depth);

                if (depth[0] > 0.0f && depth[0] < 1.0f)
                {
                    Vector3 win = new Vector3(e.X, sy, depth[0]);
                    if (GLMath.UnProject(win, modelMatrix, projeMatrix, viewportMatrix, out Vector3 mp)
                        && IsFinite(mp))
                    {
                        pivotModel = mp;
                        pivotEyeX  = (float)(screenX + (double)e.X / zoom);
                        pivotEyeY  = (float)(screenY + (double)sy   / zoom);
                    }
                }
                // Üres területen: az előző pivot marad
            }
        }

        /// <summary>Animált nézet-snap a megadott lapra.</summary>
        private void VcSnapToFace(int faceIndex)
        {
            (float rx, float ry) = ViewCube.FaceTargets[faceIndex];
            vcAnimStartRx  = rotx;
            vcAnimStartRy  = roty;
            vcAnimTargetRx = rx;
            vcAnimTargetRy = NormalizeAngleDiff(roty, ry);  // legrövidebb úton
            vcAnimProgress = 0f;
            vcAnimating    = true;
            fresh = true;
        }

        /// <summary>Legrövidebb forgásirány: target ± 360° közül a ±180°-on belüli változat.</summary>
        private static float NormalizeAngleDiff(float from, float to)
        {
            float diff = to - from;
            while (diff >  180f) diff -= 360f;
            while (diff < -180f) diff += 360f;
            return from + diff;
        }

        /// <summary>Smoothstep easing: t ∈ [0,1] → [0,1]</summary>
        private static float SmoothStep(float t) => t * t * (3f - 2f * t);

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

                // ViewCube hover frissítése
                int newHover = ViewCube.HitTest(e.X, e.Y,
                                                glControl.Width, glControl.Height,
                                                rotx, roty);
                if (newHover != vcHoveredFace)
                {
                    vcHoveredFace = newHover;
                    fresh = true;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                screenX = lastdx - e.X / zoom;
                screenY = lastdy - (glControl.Height - e.Y) / zoom;
                fresh = true;
            }
            else if (e.Button == MouseButtons.Left && !vcDragActive)
            {
                rotx += 0.5f * diffy;
                roty += 0.5f * diffx;
                fresh = true;
            }
        }

        // ── Segédmetódusok ────────────────────────────────────────────────────

        private void GetOpenGLMatrices()
        {
            GL.GetDouble(GetPName.ProjectionMatrix,  projeMatrix);
            GL.GetDouble(GetPName.ModelviewMatrix,   modelMatrix);
            GL.GetInteger(GetPName.Viewport,         viewportMatrix);
        }

        public void GetModellCoords(MouseEventArgs e)
        {
            if (!loaded) return;

            GetOpenGLMatrices();

            float[] pixel = new float[1];
            GL.ReadPixels(e.X, viewportMatrix[3] - e.Y, 1, 1,
                          PixelFormat.DepthComponent, PixelType.Float, pixel);

            Vector3 win = new Vector3(e.X, viewportMatrix[3] - e.Y, pixel[0]);
            GLMath.UnProject(win, modelMatrix, projeMatrix, viewportMatrix, out worldPosition);
        }

        private static bool IsFinite(Vector3 v)
        {
            return float.IsFinite(v.X) && float.IsFinite(v.Y) && float.IsFinite(v.Z);
        }

        private void Regenerate()
        {
            EnsureContext();
            foreach (IDisplayList item in displayLists)
                item.Regenerate(renderOptions);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            // ViewCube animáció léptetése
            if (vcAnimating)
            {
                vcAnimProgress += 1f / VcAnimSteps;
                if (vcAnimProgress >= 1f)
                {
                    vcAnimProgress = 1f;
                    vcAnimating    = false;
                }
                float t = SmoothStep(vcAnimProgress);
                rotx  = vcAnimStartRx + t * (vcAnimTargetRx - vcAnimStartRx);
                roty  = vcAnimStartRy + t * (vcAnimTargetRy - vcAnimStartRy);
                fresh = true;
            }

            if (fresh)
            {
                Redisplay();
                fresh = false;
            }
        }
    }
}
