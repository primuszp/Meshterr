using System;
using System.Windows.Forms;

namespace Meshterr
{
    public partial class FormRsgLoad : Form
    {
        public Rsg Dem { get; set; }

        public FormRsgLoad()
        {
            InitializeComponent();
            bnOK.Enabled = gridControl.HasDem;
            gridControl.DemChanged += gridControl_DemChanged;
        }

        private void gridControl_DemChanged(object sender, EventArgs e)
        {
            bnOK.Enabled = gridControl.HasDem;
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            if (!gridControl.ApplyCurrentSettings())
            {
                return;
            }

            Dem = gridControl.Dem;
            if (Dem == null)
            {
                MessageBox.Show(this, "Előbb tölts be egy RSG, BMP, JPG vagy PNG alapú terepmodellt.", "Nincs betöltött modell", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
