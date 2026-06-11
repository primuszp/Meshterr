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
        private sealed class MeshterrCamera
        {
            private float hw, hh;
            private float zn, zf;
            private float iez;
            private float tsx, tsy;

            private Matrix4 vtw = Matrix4.Identity;
            private Vector3 trn = Vector3.Zero;
            private Vector3 orbitPivot = Vector3.Zero;

            private float az = 0f;
            private float el = 0f;
            private double hvs = 2.0;
            private double vang = 0.0;
            private float sceneRadius = 2f;
            private int vpW = 640;
            private int vpH = 480;

            private Vector3 picked = Vector3.Zero;
            private readonly float[] depthWindow = new float[31 * 31];

            public MeshterrCamera()
            {
                RebuildRot();
                CalcViewVolume();
            }

            public float RotationSpeed { get; set; } = 0.5f;
            public bool IsOrthographic => vang == 0.0;
            public double Hvs => hvs;
            public Vector3 Pivot => orbitPivot;

            public void SetViewportSize(int width, int height)
            {
                vpW = Math.Max(1, width);
                vpH = Math.Max(1, height);
                CalcViewVolume();
            }

            public void SetSceneRadius(float radius)
            {
                sceneRadius = Math.Max(radius, 1f);
                CalcViewVolume();
            }

            public void FitToModel(double modelWidth, double modelHeight)
            {
                const double padding = 1.1;
                double aspect = (double)vpW / vpH;
                double halfWidth = Math.Max(modelWidth * padding * 0.5, 0.001);
                double halfHeight = Math.Max(modelHeight * padding * 0.5, 0.001);

                hvs = aspect >= 1.0
                    ? Math.Max(halfHeight, halfWidth / aspect)
                    : Math.Max(halfWidth, halfHeight * aspect);

                hvs = Math.Clamp(hvs, 0.001, 1_000_000.0);
                trn = Vector3.Zero;
                orbitPivot = Vector3.Zero;
                picked = Vector3.Zero;
                CalcViewVolume();
            }

            public float ZoomValue
            {
                get => (float)(1.0 / hvs);
                set
                {
                    hvs = 1.0 / Math.Clamp(value, 0.001f, 10000f);
                    CalcViewVolume();
                }
            }

            public bool PickDepthWindow(int mouseX, int mouseY, int windowSize = 11)
            {
                float sd = ReadClosestDepthWindow(mouseX, mouseY, windowSize);
                float viewZ = sd >= 0.9999f ? picked.Z : DepthToViewZ(sd);
                picked = ScreenToView(mouseX, mouseY, viewZ);
                return sd < 0.9999f;
            }

            public bool SetOrbitPivotFromScreen(int screenX, int screenY, int windowSize = 11)
            {
                float depth = ReadClosestDepthWindow(screenX, screenY, windowSize);
                if (depth < 0.9999f)
                {
                    orbitPivot = ViewToWorld(ScreenToView(screenX, screenY, DepthToViewZ(depth)));
                    return true;
                }

                float pivotViewZ = WorldToView(orbitPivot, vtw).Z;
                orbitPivot = ViewToWorld(ScreenToView(screenX, screenY, pivotViewZ));
                return false;
            }

            public void Rotate(float dx, float dy)
            {
                Vector3 pivotView = WorldToView(orbitPivot, vtw);

                az -= dx * RotationSpeed;
                el -= dy * RotationSpeed;
                el = Math.Clamp(el, -89f, 89f);
                RebuildRot();

                trn = orbitPivot - MulDir(pivotView, vtw);
            }

            public void Pan(int fromX, int fromY, int toX, int toY)
            {
                float viewZ = picked.Z;
                Vector3 from = ScreenToView(fromX, fromY, viewZ);
                Vector3 to = ScreenToView(toX, toY, viewZ);
                Vector3 worldDelta = MulDir(to - from, vtw);
                trn -= worldDelta;
                orbitPivot -= worldDelta;
            }

            public void Zoom(int mouseX, int mouseY, float factor)
            {
                float viewZ = picked.Z;
                Vector3 anchorWorld = ViewToWorld(ScreenToView(mouseX, mouseY, viewZ));
                hvs = Math.Clamp(hvs / factor, 0.001, 1_000_000.0);

                if (vang != 0.0)
                    viewZ /= factor;

                CalcViewVolume();
                trn = anchorWorld - MulDir(ScreenToView(mouseX, mouseY, viewZ), vtw);
                picked = ScreenToView(mouseX, mouseY, viewZ);
            }

            public void ToggleProjection(int mouseX, int mouseY)
            {
                float viewZ = picked.Z;
                Vector3 anchorWorld = ViewToWorld(ScreenToView(mouseX, mouseY, viewZ));

                if (vang == 0.0)
                {
                    double targetAngle = 45.0 * Math.PI / 180.0;
                    float tanHalf = (float)Math.Tan(0.5 * targetAngle);
                    hvs = Math.Clamp(hvs + viewZ * tanHalf, 0.001, 1_000_000.0);
                    vang = targetAngle;
                }
                else
                {
                    hvs = Math.Clamp(hvs * (1.0 - viewZ * iez), 0.001, 1_000_000.0);
                    vang = 0.0;
                }

                CalcViewVolume();
                trn = anchorWorld - MulDir(ScreenToView(mouseX, mouseY, viewZ), vtw);
            }

            public Vector3 GetWorldPosition(int screenX, int screenY)
            {
                float depth = ReadClosestDepthWindow(screenX, screenY, 1);
                if (depth >= 0.9999f)
                    return ViewToWorld(ScreenToView(screenX, screenY, picked.Z));

                return ViewToWorld(ScreenToView(screenX, screenY, DepthToViewZ(depth)));
            }

            public void ApplyFixedFunction()
            {
                Matrix4 projection = GetProjectionMatrix();
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(ref projection);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                if (iez != 0f)
                {
                    float ez = 1f / iez;
                    GL.Translate(-ez * tsx, -ez * tsy, -ez);
                }

                Matrix4 worldToViewRot = new Matrix4(
                    vtw.M11, vtw.M21, vtw.M31, 0f,
                    vtw.M12, vtw.M22, vtw.M32, 0f,
                    vtw.M13, vtw.M23, vtw.M33, 0f,
                    0f, 0f, 0f, 1f);

                GL.MultMatrix(ref worldToViewRot);
                GL.Translate(-trn.X, -trn.Y, -trn.Z);
            }

            public (float Rx, float Ry) ViewCubeAngles => (90f - el, -az);

            public void SetFromViewCubeAngles(float rx, float ry)
            {
                float pivotZ = WorldToView(orbitPivot, vtw).Z;
                az = -ry;
                el = Math.Clamp(90f - rx, -89f, 89f);
                RebuildRot();
                trn = orbitPivot - MulDir(new Vector3(0f, 0f, pivotZ), vtw);
            }

            private Matrix4 GetProjectionMatrix()
            {
                Matrix4 p = new Matrix4();

                if (iez == 0f)
                {
                    p.Row0 = new Vector4(1f / hw, 0f, 0f, 0f);
                    p.Row1 = new Vector4(0f, 1f / hh, 0f, 0f);
                    p.Row2 = new Vector4(-tsx / hw, -tsy / hh, -2f / (zn - zf), 0f);
                    p.Row3 = new Vector4(0f, 0f, (zn + zf) / (zn - zf), 1f);
                }
                else
                {
                    if (MathF.Abs(iez) < 1e-6f) iez = 1e-6f;
                    float ez = 1f / iez;
                    p.Row0 = new Vector4(ez / hw, 0f, 0f, 0f);
                    p.Row1 = new Vector4(0f, ez / hh, 0f, 0f);
                    p.Row2 = new Vector4(-ez * tsx / hw, -ez * tsy / hh, -(2f * ez - (zn + zf)) / (zn - zf), -1f);
                    p.Row3 = new Vector4(0f, 0f, -2f * (ez * (ez - (zn + zf)) + zn * zf) / (zn - zf), 0f);
                }

                return p;
            }

            private void CalcViewVolume()
            {
                hw = hh = (float)hvs;
                tsx = tsy = 0f;

                float depthExtent = MathF.Max(sceneRadius * 2f, (float)hvs * 8f);
                depthExtent = MathF.Max(depthExtent, MathF.Abs(WorldToView(orbitPivot, vtw).Z) + sceneRadius * 2f);
                depthExtent = MathF.Max(depthExtent, MathF.Abs(picked.Z) + sceneRadius * 2f);
                zn = depthExtent;
                zf = -depthExtent;

                if (vpW >= vpH)
                    hw *= (float)vpW / vpH;
                else
                    hh *= (float)vpH / vpW;

                if (vang == 0.0)
                {
                    iez = 0f;
                }
                else
                {
                    float eyeZ = Math.Min(hw, hh) / (float)Math.Tan(0.5 * vang);
                    iez = 1f / eyeZ;
                    float minNear = eyeZ * 0.05f;
                    if (zn > eyeZ - minNear)
                        zn = eyeZ - minNear;
                }
            }

            private Vector3 ScreenToView(int px, int py, float viewZ)
            {
                float pixToViewX = hw * 2f / vpW;
                float pixToViewY = hh * 2f / vpH;
                float x = px * pixToViewX - hw;
                float y = -(py * pixToViewY - hh);

                x += -x * viewZ * iez + tsx * viewZ;
                y += -y * viewZ * iez + tsy * viewZ;

                return new Vector3(x, y, viewZ);
            }

            private float ReadClosestDepthWindow(int mouseX, int mouseY, int windowSize)
            {
                if (mouseX < 0 || mouseY < 0 || mouseX >= vpW || mouseY >= vpH)
                    return 1f;

                int half = windowSize / 2;
                int glY = vpH - 1 - mouseY;
                int startX = Math.Max(0, mouseX - half);
                int startY = Math.Max(0, glY - half);
                int readW = Math.Min(windowSize, vpW - startX);
                int readH = Math.Min(windowSize, vpH - startY);

                if (readW <= 0 || readH <= 0)
                    return 1f;

                float sd = 1f;
                if (windowSize <= 1)
                {
                    float[] one = new float[1];
                    GL.ReadPixels(startX, startY, 1, 1, PixelFormat.DepthComponent, PixelType.Float, one);
                    return one[0];
                }

                GL.ReadPixels(startX, startY, readW, readH, PixelFormat.DepthComponent, PixelType.Float, depthWindow);
                for (int i = 0; i < readW * readH; i++)
                {
                    if (depthWindow[i] < 0.9999f && depthWindow[i] < sd)
                        sd = depthWindow[i];
                }

                return sd;
            }

            private float DepthToViewZ(float screenDepth)
            {
                float m33 = -(1f - zf * iez) / (zn - zf);
                return (screenDepth + m33 * zn) / (screenDepth * iez + m33);
            }

            private Vector3 ViewToWorld(Vector3 viewPoint) => trn + MulDir(viewPoint, vtw);

            private Vector3 WorldToView(Vector3 worldPoint, Matrix4 rotation)
            {
                Vector3 d = worldPoint - trn;
                return new Vector3(
                    d.X * rotation.M11 + d.Y * rotation.M12 + d.Z * rotation.M13,
                    d.X * rotation.M21 + d.Y * rotation.M22 + d.Z * rotation.M23,
                    d.X * rotation.M31 + d.Y * rotation.M32 + d.Z * rotation.M33);
            }

            private static Matrix4 BuildRot(float azimuth, float elevation)
            {
                float azR = azimuth * MathF.PI / 180f;
                float elR = elevation * MathF.PI / 180f;
                float cz = MathF.Cos(azR), sz = MathF.Sin(azR);
                float cx = MathF.Cos(elR), sx = MathF.Sin(elR);

                return new Matrix4(
                    cz, sz, 0f, 0f,
                    -sz * cx, cz * cx, sx, 0f,
                    sz * sx, -cz * sx, cx, 0f,
                    0f, 0f, 0f, 1f);
            }

            private void RebuildRot() => vtw = BuildRot(az, el);

            private static Vector3 MulDir(Vector3 v, Matrix4 m) => new Vector3(
                v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31,
                v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32,
                v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33);
        }

        private bool loaded = false;
        private bool fresh = true;
        private float znear = -1f;
        private float zfar = 1f;
        private int lastix = 0;
        private int lastiy = 0;
        private Vector3 worldPosition = Vector3.Zero;
        private readonly MeshterrCamera camera = new MeshterrCamera();

        private readonly List<IDisplayList> displayLists = new List<IDisplayList>();
        private RenderingType renderOptions = RenderingType.Shaded;

        private int vcHoveredFace = -1;
        private bool vcAnimating = false;
        private float vcAnimProgress = 1f;
        private float vcAnimStartRx;
        private float vcAnimStartRy;
        private float vcAnimTargetRx;
        private float vcAnimTargetRy;
        private const float VcAnimSteps = 25f;
        private bool vcDragActive = false;

        public float Zoom
        {
            get { return camera.ZoomValue; }
            set
            {
                camera.ZoomValue = value;
                fresh = true;
            }
        }

        public float zNear
        {
            get { return znear; }
            set
            {
                znear = value;
                UpdateSceneRadius();
                fresh = true;
            }
        }

        public float zFar
        {
            get { return zfar; }
            set
            {
                zfar = value;
                UpdateSceneRadius();
                fresh = true;
            }
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

        public IList<IDisplayList> DisplayLists => displayLists;
        public Vector3 WorldPosition => worldPosition;

        public DrawingArea()
        {
            InitializeComponent();
            glControl.TabStop = true;
            glControl.KeyDown += glControl_KeyDown;
            glControl.MouseUp += glControl_MouseUp;
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

        public void FitToModel(double modelWidth, double modelHeight)
        {
            if (modelWidth <= 0 || modelHeight <= 0 || glControl.Width <= 0 || glControl.Height <= 0)
                return;

            camera.SetViewportSize(glControl.Width, glControl.Height);
            camera.FitToModel(modelWidth, modelHeight);
            UpdateSceneRadius();
            fresh = true;
        }

        private void EnsureContext()
        {
            if (loaded) glControl.MakeCurrent();
        }

        private void UpdateSceneRadius()
        {
            camera.SetSceneRadius(Math.Max(Math.Abs(znear), Math.Abs(zfar)));
        }

        private void Reshape()
        {
            if (!loaded) return;

            EnsureContext();
            if (glControl.ClientSize.Height == 0)
                glControl.ClientSize = new Size(glControl.ClientSize.Width, 1);

            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            camera.SetViewportSize(glControl.Width, glControl.Height);
            camera.ApplyFixedFunction();
        }

        private void Render()
        {
            if (!loaded) return;

            EnsureContext();
            GL.ClearColor(Color.LightSkyBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            camera.ApplyFixedFunction();

            foreach (IDisplayList item in DisplayLists)
                item.Call();

            (float rx, float ry) = camera.ViewCubeAngles;
            ViewCube.Render(glControl.Width, glControl.Height, rx, ry, vcHoveredFace);
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
            GL.ClearDepth(1.0);
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

            EnsureContext();
            camera.PickDepthWindow(e.X, e.Y, 11);
            float zoomRatio = Math.Clamp((float)(camera.Hvs / Math.Max(Math.Abs(zfar), 1f)), 0.1f, 5f);
            float step = Math.Clamp(zoomRatio * 0.25f, 0.1f, 0.5f);
            float factor = e.Delta > 0 ? 1f + step : 1f / (1f + step);
            camera.Zoom(e.X, e.Y, factor);
            fresh = true;
        }

        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            lastix = e.X;
            lastiy = e.Y;
            vcDragActive = false;

            EnsureContext();
            glControl.Focus();

            if (e.Button == MouseButtons.Left)
            {
                (float rx, float ry) = camera.ViewCubeAngles;
                int hitFace = ViewCube.HitTest(e.X, e.Y, glControl.Width, glControl.Height, rx, ry);
                if (hitFace >= 0)
                {
                    vcDragActive = true;
                    VcSnapToFace(hitFace);
                    return;
                }

                camera.SetOrbitPivotFromScreen(e.X, e.Y, 11);
            }
            else if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle)
            {
                camera.PickDepthWindow(e.X, e.Y, 11);
            }
        }

        private void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            vcDragActive = false;
        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            int diffx = e.X - lastix;
            int diffy = e.Y - lastiy;

            if (e.Button == MouseButtons.None)
            {
                GetModellCoords(e);
                (float rx, float ry) = camera.ViewCubeAngles;
                int newHover = ViewCube.HitTest(e.X, e.Y, glControl.Width, glControl.Height, rx, ry);
                if (newHover != vcHoveredFace)
                {
                    vcHoveredFace = newHover;
                    fresh = true;
                }
            }
            else if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle)
            {
                camera.Pan(lastix, lastiy, e.X, e.Y);
                fresh = true;
            }
            else if (e.Button == MouseButtons.Left && !vcDragActive)
            {
                camera.Rotate(diffx, diffy);
                fresh = true;
            }

            lastix = e.X;
            lastiy = e.Y;
        }

        private void glControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Space) return;

            EnsureContext();
            Point p = glControl.PointToClient(Cursor.Position);
            if (p.X < 0 || p.Y < 0 || p.X >= glControl.Width || p.Y >= glControl.Height)
                p = new Point(glControl.Width / 2, glControl.Height / 2);

            camera.PickDepthWindow(p.X, p.Y, 11);
            camera.ToggleProjection(p.X, p.Y);
            fresh = true;
        }

        private void VcSnapToFace(int faceIndex)
        {
            (float rx, float ry) = ViewCube.FaceTargets[faceIndex];
            (float curRx, float curRy) = camera.ViewCubeAngles;
            vcAnimStartRx = curRx;
            vcAnimStartRy = curRy;
            vcAnimTargetRx = rx;
            vcAnimTargetRy = NormalizeAngleDiff(curRy, ry);
            vcAnimProgress = 0f;
            vcAnimating = true;
            fresh = true;
        }

        private static float NormalizeAngleDiff(float from, float to)
        {
            float diff = to - from;
            while (diff > 180f) diff -= 360f;
            while (diff < -180f) diff += 360f;
            return from + diff;
        }

        private static float SmoothStep(float t) => t * t * (3f - 2f * t);

        public void GetModellCoords(MouseEventArgs e)
        {
            if (!loaded) return;

            EnsureContext();
            worldPosition = camera.GetWorldPosition(e.X, e.Y);
        }

        private void Regenerate()
        {
            EnsureContext();
            foreach (IDisplayList item in displayLists)
                item.Regenerate(renderOptions);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (vcAnimating)
            {
                vcAnimProgress += 1f / VcAnimSteps;
                if (vcAnimProgress >= 1f)
                {
                    vcAnimProgress = 1f;
                    vcAnimating = false;
                }

                float t = SmoothStep(vcAnimProgress);
                float rx = vcAnimStartRx + t * (vcAnimTargetRx - vcAnimStartRx);
                float ry = vcAnimStartRy + t * (vcAnimTargetRy - vcAnimStartRy);
                camera.SetFromViewCubeAngles(rx, ry);
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
