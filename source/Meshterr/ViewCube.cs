using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Meshterr
{
    /// <summary>
    /// ViewCube – jobb felső sarokban elhelyezett 3D navigációs kocka.
    /// Kattintásra a nézet az adott laphoz / élhez snappel, animálva.
    /// </summary>
    internal static class ViewCube
    {
        // ── Konstansok ────────────────────────────────────────────────────────

        public const int SIZE   = 88;   // pixel méret
        public const int MARGIN = 10;   // pixel margó a saroktól

        // ── Standard nézetek lapindex szerint (rotx°, roty°) ─────────────────

        /// <summary>Kattintható lapok: (rotx, roty) célszögek.</summary>
        public static readonly (float Rx, float Ry)[] FaceTargets =
        {
            (  0f,   0f),   // 0  Top    (+Z)
            (180f,   0f),   // 1  Bottom (-Z)
            ( 90f,   0f),   // 2  Front  (-Y)
            ( 90f, 180f),   // 3  Back   (+Y)
            ( 90f, -90f),   // 4  Right  (+X)
            ( 90f,  90f),   // 5  Left   (-X)
        };

        // ── Lapgeometria ──────────────────────────────────────────────────────

        // Lapcsúcsok ±1 egységkockán, kívülről CCW sorrendben
        private static readonly Vector3[][] FaceVerts =
        {
            new[]{ new Vector3(-1,-1, 1), new Vector3( 1,-1, 1), new Vector3( 1, 1, 1), new Vector3(-1, 1, 1) }, // Top
            new[]{ new Vector3(-1, 1,-1), new Vector3( 1, 1,-1), new Vector3( 1,-1,-1), new Vector3(-1,-1,-1) }, // Bottom
            new[]{ new Vector3(-1,-1,-1), new Vector3( 1,-1,-1), new Vector3( 1,-1, 1), new Vector3(-1,-1, 1) }, // Front
            new[]{ new Vector3( 1, 1,-1), new Vector3(-1, 1,-1), new Vector3(-1, 1, 1), new Vector3( 1, 1, 1) }, // Back
            new[]{ new Vector3( 1,-1,-1), new Vector3( 1, 1,-1), new Vector3( 1, 1, 1), new Vector3( 1,-1, 1) }, // Right
            new[]{ new Vector3(-1, 1,-1), new Vector3(-1,-1,-1), new Vector3(-1,-1, 1), new Vector3(-1, 1, 1) }, // Left
        };

        // Ray-lap metszéshez: melyik tengely és milyen előjel
        private static readonly int[]   FaceAxis = { 2, 2, 1, 1, 0, 0 }; // 0=X,1=Y,2=Z
        private static readonly float[] FaceSign = { 1f,-1f,-1f, 1f, 1f,-1f };

        // ── Színek ────────────────────────────────────────────────────────────

        private static readonly Color[] FaceColor =
        {
            Color.FromArgb(230, 230, 230), // Top     – világosszürke
            Color.FromArgb(150, 150, 150), // Bottom  – sötétszürke
            Color.FromArgb(150, 205, 150), // Front   – zöld
            Color.FromArgb( 90, 140,  90), // Back    – sötétzöld
            Color.FromArgb(210, 140, 140), // Right   – piros
            Color.FromArgb(140,  90,  90), // Left    – sötétpiros
        };

        private static readonly Color[] HoverColor =
        {
            Color.FromArgb(255, 255, 200), // Top hover
            Color.FromArgb(200, 200, 200), // Bottom hover
            Color.FromArgb(180, 245, 180), // Front hover
            Color.FromArgb(120, 180, 120), // Back hover
            Color.FromArgb(255, 180, 180), // Right hover
            Color.FromArgb(180, 120, 120), // Left hover
        };

        // ── Terület-teszt ─────────────────────────────────────────────────────

        /// <summary>Az egér a ViewCube területén van-e? (Windows-koordináta: y=0 felül)</summary>
        public static bool IsInRegion(int mx, int my, int W, int H)
            => mx >= W - SIZE - MARGIN && mx < W - MARGIN
            && my >= MARGIN            && my < MARGIN + SIZE;

        // ── Renderelés ────────────────────────────────────────────────────────

        /// <summary>ViewCube kirajzolása a jobb felső sarokba.</summary>
        public static void Render(int W, int H, float rotx, float roty, int hoveredFace)
        {
            int vpX = W - SIZE - MARGIN;
            int vpY = H - SIZE - MARGIN;   // GL: from bottom-left

            // Depth törlése csak erre a területre (scissor)
            GL.Enable(EnableCap.ScissorTest);
            GL.Scissor(vpX, vpY, SIZE, SIZE);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Disable(EnableCap.ScissorTest);

            GL.Viewport(vpX, vpY, SIZE, SIZE);

            // ── Perspective projection ────────────────────────────────────────
            // 30°-os félszög (60° teljes FOV): minden kockacsúcs NDC ≤ 0.88 → nincs frustum clip
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            {
                const double near = 0.5, far = 10.0;
                double t = near * Math.Tan(30.0 * Math.PI / 180.0);
                GL.Frustum(-t, t, -t, t, near, far);
            }

            // ── Modelview: kamera (0,0,−3.5) → origó ─────────────────────────
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Translate(0.0, 0.0, -3.5);
            GL.Rotate(rotx, 1f, 0f, 0f);
            GL.Rotate(roty, 0f, 0f, 1f);

            GL.Disable(EnableCap.Lighting);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            // Lapok – kitöltve
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            for (int i = 0; i < 6; i++)
            {
                Color c = (i == hoveredFace) ? HoverColor[i] : FaceColor[i];
                GL.Color3(c);
                GL.Begin(BeginMode.Quads);
                foreach (var v in FaceVerts[i]) GL.Vertex3(v.X, v.Y, v.Z);
                GL.End();
            }

            // Élek – vonalak
            GL.Disable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.LineWidth(1.3f);
            GL.Color3(0.15f, 0.15f, 0.15f);
            for (int i = 0; i < 6; i++)
            {
                GL.Begin(BeginMode.Quads);
                foreach (var v in FaceVerts[i]) GL.Vertex3(v.X, v.Y, v.Z);
                GL.End();
            }
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            // ── Mátrixok visszaállítása ───────────────────────────────────────
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.Disable(EnableCap.CullFace);

            // Viewport visszaállítása
            GL.Viewport(0, 0, W, H);
        }

        // ── Hit-test (CPU ray casting) ────────────────────────────────────────

        /// <summary>
        /// Melyik lapot érinti az egér?  −1 = nincs találat.
        /// mx, my: Windows-koordináta (y=0 felül).
        /// </summary>
        public static int HitTest(int mx, int my, int W, int H, float rotx, float roty)
        {
            if (!IsInRegion(mx, my, W, H)) return -1;

            // Helyi koordináta a ViewCube viewportban
            float lx     = mx - (W - SIZE - MARGIN) + 0.5f;
            float lyTop  = my - MARGIN + 0.5f;
            float lyGl   = SIZE - lyTop;   // GL: y=0 alul

            // NDC: [−1, +1]
            float ndcX =  lx   / SIZE * 2f - 1f;
            float ndcY =  lyGl / SIZE * 2f - 1f;

            // Sugár eye-space irány (30°-os félszög, ugyanaz mint a Frustumban)
            float tan30 = (float)Math.Tan(30.0 * Math.PI / 180.0);
            Vector3 rayEye = Vector3.Normalize(new Vector3(ndcX * tan30, ndcY * tan30, -1f));

            // Sugár → model-space
            Vector3 camModel = InvRot(new Vector3(0f, 0f, 3.5f), rotx, roty);
            Vector3 dirModel = InvRot(rayEye,                     rotx, roty);

            // Ray – AABB lap metszés: legközelebbi lap kiválasztása
            float bestT    = float.MaxValue;
            int   bestFace = -1;
            for (int i = 0; i < 6; i++)
            {
                float ti = RayFaceT(camModel, dirModel, i);
                if (ti > 1e-4f && ti < bestT) { bestT = ti; bestFace = i; }
            }
            return bestFace;
        }

        // ── Segédmetódusok ────────────────────────────────────────────────────

        private static float RayFaceT(Vector3 orig, Vector3 dir, int face)
        {
            int   axis = FaceAxis[face];
            float val  = FaceSign[face];
            float o    = C(orig, axis);
            float d    = C(dir,  axis);
            if (MathF.Abs(d) < 1e-7f) return -1f;
            float t = (val - o) / d;
            if (t <= 0f) return -1f;
            Vector3 hit = orig + t * dir;
            int u = (axis + 1) % 3, v = (axis + 2) % 3;
            if (MathF.Abs(C(hit, u)) > 1.01f || MathF.Abs(C(hit, v)) > 1.01f) return -1f;
            return t;
        }

        private static float C(Vector3 v, int axis) => axis == 0 ? v.X : axis == 1 ? v.Y : v.Z;

        /// <summary>(Rx·Rz)^{−1} = Rz(−ry) · Rx(−rx)</summary>
        private static Vector3 InvRot(Vector3 v, float rotx, float roty)
        {
            float rx = -rotx * MathF.PI / 180f;
            float rz = -roty * MathF.PI / 180f;

            float cz = MathF.Cos(rz), sz = MathF.Sin(rz);
            float x1 = v.X * cz - v.Y * sz;
            float y1 = v.X * sz + v.Y * cz;

            float cx = MathF.Cos(rx), sx = MathF.Sin(rx);
            float y2 = y1 * cx - v.Z * sx;
            float z2 = y1 * sx + v.Z * cx;

            return new Vector3(x1, y2, z2);
        }
    }
}
