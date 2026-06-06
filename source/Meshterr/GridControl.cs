using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Meshterr
{
    public partial class RsgControl : UserControl
    {
        private Palette pal = null;
        private Bitmap bmp = null;
        private Rsg dem = null;

        public event EventHandler DemChanged;

        public Rsg Dem
        {
            get { return dem; }
        }

        public bool HasDem
        {
            get { return dem != null; }
        }

        public RsgControl()
        {
            InitializeComponent();
            bnApplay.Enabled = false;
            nTbMin.AllowSymbols = true;
            nTbMax.AllowSymbols = true;

            Bitmap palette = Resources.GetBitmap("szürkeskála");
            palette.RotateFlip(RotateFlipType.Rotate90FlipNone);

            pal = new Palette(palette);
            pBPalette.Image = Resources.GetBitmap("szürkeskála");
        }

        public bool ApplyCurrentSettings()
        {
            if (dem == null)
            {
                MessageBox.Show(this, "Előbb tölts be egy RSG, BMP, JPG vagy PNG alapú terepmodellt.", "Nincs betöltött modell", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            dem.RsgPalette = pal;

            if (dem.IsImageBased)
            {
                if (!TryReadSingle(nTbPsX.Text, out float pixelWidth) ||
                    !TryReadSingle(nTbPsY.Text, out float pixelHeight) ||
                    !TryReadSingle(nTbMin.Text, out float elevationMin) ||
                    !TryReadSingle(nTbMax.Text, out float elevationMax))
                {
                    MessageBox.Show(this, "A pixelméret és Min/Max mezők csak számértéket tartalmazhatnak.", "Hibás skála", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                try
                {
                    dem.ApplyImageScale(pixelWidth, pixelHeight, elevationMin, elevationMax);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    MessageBox.Show(this, ex.Message, "Hibás skála", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                LoadValueToBox(dem);
            }

            bmp = dem.BmpFromRsg();
            pBPreView.Image = scalablePictureBox.Picture = bmp;
            DemChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        private static bool TryReadSingle(string text, out float value)
        {
            return float.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value) ||
                   float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private void LoadValueToBox(Rsg dem)
        {
            nTbHMin.Text = dem.Box.Left.ToString(CultureInfo.CurrentCulture);
            nTbHMax.Text = dem.Box.Right.ToString(CultureInfo.CurrentCulture);
            nTbVMin.Text = dem.Box.Top.ToString(CultureInfo.CurrentCulture);
            nTbVMax.Text = dem.Box.Bottom.ToString(CultureInfo.CurrentCulture);
            nTbPsX.Text = dem.XDimension.ToString(CultureInfo.CurrentCulture);
            nTbPsY.Text = dem.YDimension.ToString(CultureInfo.CurrentCulture);
            nTbMin.Text = dem.MinElevation.ToString(CultureInfo.CurrentCulture);
            nTbMax.Text = dem.MaxElevation.ToString(CultureInfo.CurrentCulture);
        }

        private void SetDem(Rsg loadedDem, Bitmap preview)
        {
            dem = loadedDem;
            bmp = preview;
            bnApplay.Enabled = dem != null;
            DemChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ClearDem()
        {
            dem = null;
            bmp = null;
            bnApplay.Enabled = false;
            DemChanged?.Invoke(this, EventArgs.Empty);
        }

        private void bnLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialog == null)
            {
                return;
            }

            openFileDialog.Filter = "ERS - ER-Mapper raszter (*.ers)|*.ers|BMP - Windows bitkép|*.bmp|JPG fájl|*.jpg;*.jpeg|PNG fájl|*.png";
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string imageFileName = openFileDialog.FileName;
            tBFilePath.Text = imageFileName;

            try
            {
                Rsg loadedDem;
                Bitmap preview;

                switch (openFileDialog.FilterIndex)
                {
                    case 1:
                        loadedDem = new Rsg(pal);
                        loadedDem.LoadErsMap(openFileDialog.FileName);
                        preview = loadedDem.BmpFromRsg();
                        break;
                    default:
                        using (Bitmap image = new Bitmap(imageFileName))
                        {
                            loadedDem = new Rsg(pal, new Bitmap(image));
                        }
                        preview = loadedDem.BmpFromRsg();
                        break;
                }

                LoadValueToBox(loadedDem);
                SetDem(loadedDem, preview);
                pBPreView.Image = scalablePictureBox.Picture = bmp;
            }
            catch (Exception ex)
            {
                ClearDem();
                MessageBox.Show(this, ex.Message, "Betöltési hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bnPalette_Click(object sender, EventArgs e)
        {
            if (openFileDialog == null)
            {
                return;
            }

            openFileDialog.Filter = "Paletta (*.pal)|*.pal|BMP képfájl (*.bmp)|*.bmp|PNG képfájl (*.png)|*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FilterIndex == 1)
                {
                    pal.ReadPaletteFromTxt(openFileDialog.FileName);
                }
                else
                {
                    pal.ReadPaletteFromBmp(openFileDialog.FileName);
                }
            }

            Bitmap paletteBitmap = pal.PaletteToBitmap();
            paletteBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pBPalette.Image = paletteBitmap;
        }

        private void bnApplay_Click(object sender, EventArgs e)
        {
            ApplyCurrentSettings();
        }
    }
}
