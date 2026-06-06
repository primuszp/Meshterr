using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Meshterr
{
    class IsoLine : DisplayList
    {
        private Rsg rsg = null;
        private Tin tin = null;
        private double level;

        public IsoLine()
        {

        }

        public IsoLine(Rsg rsg, double level)
        {
            this.rsg = rsg;
            this.level = level;
        }

        public IsoLine(Tin tin, double level)
        {
            this.tin = tin;
            this.level = level;
        }


        public void RenderIsoLines2()
        {
            int offsetX = tin.OffsetX;
            int offsetY = tin.OffsetY;
            int offsetZ = tin.OffsetZ;

            foreach (Face item in tin.Mesh.Faces)
            {
                if (item.FaceType == Face.FType.Normal)
                {
                    List<Vector3d> points = Szintvonal(tin.Mesh.Nodes[item.IndexToNodeA].Vertex, tin.Mesh.Nodes[item.IndexToNodeB].Vertex, tin.Mesh.Nodes[item.IndexToNodeC].Vertex, level);

                    GL.Color3(Color.Orange);
                    GL.LineWidth(1);

                    GL.Begin(BeginMode.Lines);

                    for (int i = 0; i < points.Count; i++)
                    {
                        GL.Vertex3(points[i].X  - offsetX, points[i].Y  - offsetY, (points[i].Z - offsetZ) * tin.Distoration);
                    }

                    GL.End();
                }
            }

        }

        public void RenderIsoLines()
        {
            double offsetX = rsg.OffsetX;
            double offsetY = rsg.OffsetY;
            double offsetZ = rsg.OffsetZ;

            for (int y = 0; y < rsg.NrOfLines - 1; y++)
            {
                for (int x = 0; x < rsg.NrOfCellsPerLine - 1; x++)
                {
                    List<Vector3d> points = Szintvonal(new Vector3d(x, y, rsg.ZData[x, y]), new Vector3d(x, y + 1, rsg.ZData[x, y + 1]), new Vector3d(x + 1, y, rsg.ZData[x + 1, y]), level);

                    GL.Color3(Color.Brown);
                    GL.LineWidth(2.5f);

                    GL.Begin(BeginMode.Lines);

                    for (int i = 0; i < points.Count; i++)
                    {
                        GL.Vertex3(points[i].X * rsg.XDimension - offsetX, points[i].Y * rsg.YDimension - offsetY, (points[i].Z - offsetZ) * rsg.Distoration);
                    }

                    GL.End();

                    List<Vector3d> points2 = Szintvonal(new Vector3d(x, y + 1, rsg.ZData[x, y + 1]), new Vector3d(x + 1, y, rsg.ZData[x + 1, y]), new Vector3d(x + 1, y + 1, rsg.ZData[x + 1, y + 1]), level);
                    
                    GL.Begin(BeginMode.Lines);
                    for (int i = 0; i < points2.Count; i++)
                    {
                        GL.Vertex3(points2[i].X * rsg.XDimension - offsetX, points2[i].Y * rsg.YDimension - offsetY, (points2[i].Z - offsetZ) * rsg.Distoration);
                    }
                    GL.End();
                }
            }
        }

        private List<Vector3d> Szintvonal(Vector3d p0, Vector3d p1, Vector3d p2, double level)
        {
            List<Vector3d> points = new List<Vector3d>();

            double dZ0 = Math.Abs(p0.Z - p1.Z);
            double dZ1 = Math.Abs(p1.Z - p2.Z);
            double dZ2 = Math.Abs(p2.Z - p0.Z);

            if (dZ0 >= dZ1 && dZ0 >= dZ2)
            {
                isoLineCalc(p0, p1, p2, level, points);
            }
            else
                if (dZ1 >= dZ0 && dZ1 >= dZ2)
                {
                    isoLineCalc(p1, p2, p0, level, points);
                }
                else
                    if (dZ2 >= dZ0 && dZ2 >= dZ1)
                    {
                        isoLineCalc(p2, p0, p1, level, points);
                    }
            return (points);
        }

        private void isoLineCalc(Vector3d p0, Vector3d p1, Vector3d p2, double level, List<Vector3d> points)
        {
            double min = Math.Min(p0.Z, p1.Z);
            double max = Math.Max(p0.Z, p1.Z);
            double step = Math.Round(min / level, 0) * level;

            do
            {
                isoPointCalc(p0, p1, points, step);

                if (!isoPointCalc(p1, p2, points, step))
                {
                    isoPointCalc(p2, p0, points, step);
                }

                step = step + level;
            } while (step <= max);
        }

        private bool isoPointCalc(Vector3d p0, Vector3d p1, List<Vector3d> points, double step)
        {
            double t = tCalc(step, p0.Z, p1.Z);

            if (t >= 0.0 && t <= 1.0)
            {
                points.Add(CalcPoint(t, p0, p1));
                return (true);
            }
            else
            {
                return (false);
            }
        }

        private double tCalc(double z, double z0, double z1)
        {
            return ((z - z0) / (-z0 + z1));
        }

        private Vector3d CalcPoint(double t, Vector3d p0, Vector3d p1)
        {
            return (new Vector3d((1.0 - t) * p0 + t * p1));
        }

        /// <summary>
        /// Virtual method to Regenerate the display lists based on the parameters for solid fill and edge drawing 
        /// </summary>
        /// <param name="renderingOptions">Rendering options to impose on the regenerated display list objects.</param>
        /// <exception cref="ArgumentNullException">Mesh member is null.</exception>
        public override void Regenerate(RenderingType RenderingOption)
        {
            if (IsList())
            {
                Delete();
            }

            NewList(ListMode.Compile);

            GL.Enable(EnableCap.LineSmooth);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.DontCare);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.LineSmooth);

            if (rsg != null)
            {
                RenderIsoLines();
            }
            else if (tin != null)
            {
                RenderIsoLines2();
            }

            GL.Disable(EnableCap.LineSmooth);
            GL.Disable(EnableCap.Blend);

            EndList();
        }
    }
}
