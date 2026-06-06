using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MeshTopology
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            Vector p1 = new Vector(0, 0, 0);
            Vector p2 = new Vector(2, 0, 0);
            Vector p3 = new Vector(2, 2, 0);

            Vector p4 = new Vector(3, 6, 0);
            Vector p5 = new Vector(4, 4, 0);
            Vector p6 = new Vector(4, 8, 0);

            List<Vector> nodes = new List<Vector>();
            nodes.Add(p1);
            nodes.Add(p2);
            nodes.Add(p3);
            nodes.Add(p4);
            nodes.Add(p5);
            nodes.Add(p6);
        }
    }
}
