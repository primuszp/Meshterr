namespace Meshterr
{
    partial class RsgControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tBGrid = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lbProgress = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.gBPalette = new System.Windows.Forms.GroupBox();
            this.bnReCalc = new System.Windows.Forms.Button();
            this.nTbMax = new NumericTextBox.NumericTextBox();
            this.nTbMin = new NumericTextBox.NumericTextBox();
            this.bnApplay = new System.Windows.Forms.Button();
            this.pBPalette = new System.Windows.Forms.PictureBox();
            this.bnPalette = new System.Windows.Forms.Button();
            this.gBRegion = new System.Windows.Forms.GroupBox();
            this.nTbPsY = new NumericTextBox.NumericTextBox();
            this.nTbVMax = new NumericTextBox.NumericTextBox();
            this.nTbPsX = new NumericTextBox.NumericTextBox();
            this.nTbVMin = new NumericTextBox.NumericTextBox();
            this.lbPixelSize = new System.Windows.Forms.Label();
            this.lbVertical = new System.Windows.Forms.Label();
            this.nTbHMax = new NumericTextBox.NumericTextBox();
            this.lbHorizontal = new System.Windows.Forms.Label();
            this.nTbHMin = new NumericTextBox.NumericTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pBPreView = new System.Windows.Forms.PictureBox();
            this.gBLoad = new System.Windows.Forms.GroupBox();
            this.tBFilePath = new System.Windows.Forms.TextBox();
            this.bnLoad = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.scalablePictureBox = new QAlbum.ScalablePictureBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tBGrid.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.gBPalette.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBPalette)).BeginInit();
            this.gBRegion.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBPreView)).BeginInit();
            this.gBLoad.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tBGrid
            // 
            this.tBGrid.Controls.Add(this.tabPage1);
            this.tBGrid.Controls.Add(this.tabPage2);
            this.tBGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tBGrid.Location = new System.Drawing.Point(0, 0);
            this.tBGrid.Name = "tBGrid";
            this.tBGrid.SelectedIndex = 0;
            this.tBGrid.Size = new System.Drawing.Size(600, 410);
            this.tBGrid.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lbProgress);
            this.tabPage1.Controls.Add(this.progressBar1);
            this.tabPage1.Controls.Add(this.gBPalette);
            this.tabPage1.Controls.Add(this.gBRegion);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.gBLoad);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(592, 384);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Betöltés";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lbProgress
            // 
            this.lbProgress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbProgress.Location = new System.Drawing.Point(6, 355);
            this.lbProgress.Name = "lbProgress";
            this.lbProgress.Size = new System.Drawing.Size(120, 23);
            this.lbProgress.TabIndex = 4;
            this.lbProgress.Text = "Feldolgozás";
            this.lbProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(132, 355);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(454, 23);
            this.progressBar1.TabIndex = 5;
            // 
            // gBPalette
            // 
            this.gBPalette.Controls.Add(this.bnReCalc);
            this.gBPalette.Controls.Add(this.nTbMax);
            this.gBPalette.Controls.Add(this.nTbMin);
            this.gBPalette.Controls.Add(this.bnApplay);
            this.gBPalette.Controls.Add(this.pBPalette);
            this.gBPalette.Controls.Add(this.bnPalette);
            this.gBPalette.Location = new System.Drawing.Point(6, 211);
            this.gBPalette.Name = "gBPalette";
            this.gBPalette.Size = new System.Drawing.Size(294, 138);
            this.gBPalette.TabIndex = 3;
            this.gBPalette.TabStop = false;
            this.gBPalette.Text = "Paletta";
            // 
            // bnReCalc
            // 
            this.bnReCalc.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.bnReCalc.Location = new System.Drawing.Point(105, 99);
            this.bnReCalc.Name = "bnReCalc";
            this.bnReCalc.Size = new System.Drawing.Size(84, 25);
            this.bnReCalc.TabIndex = 10;
            this.bnReCalc.Text = "Min - Max";
            this.bnReCalc.UseVisualStyleBackColor = true;
            // 
            // nTbMax
            // 
            this.nTbMax.AllowControls = true;
            this.nTbMax.AllowDecimalSeparator = true;
            this.nTbMax.AllowDigits = true;
            this.nTbMax.AllowLowers = false;
            this.nTbMax.AllowSymbols = false;
            this.nTbMax.AllowUppers = false;
            this.nTbMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nTbMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.nTbMax.Location = new System.Drawing.Point(195, 101);
            this.nTbMax.Name = "nTbMax";
            this.nTbMax.Size = new System.Drawing.Size(80, 23);
            this.nTbMax.TabIndex = 9;
            // 
            // nTbMin
            // 
            this.nTbMin.AllowControls = true;
            this.nTbMin.AllowDecimalSeparator = true;
            this.nTbMin.AllowDigits = true;
            this.nTbMin.AllowLowers = false;
            this.nTbMin.AllowSymbols = false;
            this.nTbMin.AllowUppers = false;
            this.nTbMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nTbMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.nTbMin.Location = new System.Drawing.Point(19, 101);
            this.nTbMin.Name = "nTbMin";
            this.nTbMin.Size = new System.Drawing.Size(80, 23);
            this.nTbMin.TabIndex = 9;
            // 
            // bnApplay
            // 
            this.bnApplay.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.bnApplay.Location = new System.Drawing.Point(147, 25);
            this.bnApplay.Name = "bnApplay";
            this.bnApplay.Size = new System.Drawing.Size(128, 30);
            this.bnApplay.TabIndex = 1;
            this.bnApplay.Text = "&Alkalmaz";
            this.bnApplay.UseVisualStyleBackColor = true;
            this.bnApplay.Click += new System.EventHandler(this.bnApplay_Click);
            // 
            // pBPalette
            // 
            this.pBPalette.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pBPalette.Location = new System.Drawing.Point(19, 61);
            this.pBPalette.Name = "pBPalette";
            this.pBPalette.Size = new System.Drawing.Size(256, 32);
            this.pBPalette.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pBPalette.TabIndex = 3;
            this.pBPalette.TabStop = false;
            // 
            // bnPalette
            // 
            this.bnPalette.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.bnPalette.Location = new System.Drawing.Point(19, 25);
            this.bnPalette.Name = "bnPalette";
            this.bnPalette.Size = new System.Drawing.Size(128, 30);
            this.bnPalette.TabIndex = 0;
            this.bnPalette.Text = "&Paletta betöltése";
            this.bnPalette.UseVisualStyleBackColor = true;
            this.bnPalette.Click += new System.EventHandler(this.bnPalette_Click);
            // 
            // gBRegion
            // 
            this.gBRegion.Controls.Add(this.nTbPsY);
            this.gBRegion.Controls.Add(this.nTbVMax);
            this.gBRegion.Controls.Add(this.nTbPsX);
            this.gBRegion.Controls.Add(this.nTbVMin);
            this.gBRegion.Controls.Add(this.lbPixelSize);
            this.gBRegion.Controls.Add(this.lbVertical);
            this.gBRegion.Controls.Add(this.nTbHMax);
            this.gBRegion.Controls.Add(this.lbHorizontal);
            this.gBRegion.Controls.Add(this.nTbHMin);
            this.gBRegion.Location = new System.Drawing.Point(6, 67);
            this.gBRegion.Name = "gBRegion";
            this.gBRegion.Size = new System.Drawing.Size(294, 138);
            this.gBRegion.TabIndex = 1;
            this.gBRegion.TabStop = false;
            this.gBRegion.Text = "Befoglaló méretek";
            // 
            // nTbPsY
            // 
            this.nTbPsY.AllowControls = true;
            this.nTbPsY.AllowDecimalSeparator = true;
            this.nTbPsY.AllowDigits = true;
            this.nTbPsY.AllowLowers = false;
            this.nTbPsY.AllowSymbols = false;
            this.nTbPsY.AllowUppers = false;
            this.nTbPsY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nTbPsY.Location = new System.Drawing.Point(181, 93);
            this.nTbPsY.Name = "nTbPsY";
            this.nTbPsY.Size = new System.Drawing.Size(80, 20);
            this.nTbPsY.TabIndex = 8;
            // 
            // nTbVMax
            // 
            this.nTbVMax.AllowControls = true;
            this.nTbVMax.AllowDecimalSeparator = true;
            this.nTbVMax.AllowDigits = true;
            this.nTbVMax.AllowLowers = false;
            this.nTbVMax.AllowSymbols = false;
            this.nTbVMax.AllowUppers = false;
            this.nTbVMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nTbVMax.Location = new System.Drawing.Point(181, 67);
            this.nTbVMax.Name = "nTbVMax";
            this.nTbVMax.Size = new System.Drawing.Size(80, 20);
            this.nTbVMax.TabIndex = 7;
            // 
            // nTbPsX
            // 
            this.nTbPsX.AllowControls = true;
            this.nTbPsX.AllowDecimalSeparator = true;
            this.nTbPsX.AllowDigits = true;
            this.nTbPsX.AllowLowers = false;
            this.nTbPsX.AllowSymbols = false;
            this.nTbPsX.AllowUppers = false;
            this.nTbPsX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nTbPsX.Location = new System.Drawing.Point(95, 93);
            this.nTbPsX.Name = "nTbPsX";
            this.nTbPsX.Size = new System.Drawing.Size(80, 20);
            this.nTbPsX.TabIndex = 6;
            // 
            // nTbVMin
            // 
            this.nTbVMin.AllowControls = true;
            this.nTbVMin.AllowDecimalSeparator = true;
            this.nTbVMin.AllowDigits = true;
            this.nTbVMin.AllowLowers = false;
            this.nTbVMin.AllowSymbols = false;
            this.nTbVMin.AllowUppers = false;
            this.nTbVMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nTbVMin.Location = new System.Drawing.Point(95, 67);
            this.nTbVMin.Name = "nTbVMin";
            this.nTbVMin.Size = new System.Drawing.Size(80, 20);
            this.nTbVMin.TabIndex = 5;
            // 
            // lbPixelSize
            // 
            this.lbPixelSize.AutoSize = true;
            this.lbPixelSize.Location = new System.Drawing.Point(34, 95);
            this.lbPixelSize.Name = "lbPixelSize";
            this.lbPixelSize.Size = new System.Drawing.Size(58, 13);
            this.lbPixelSize.TabIndex = 4;
            this.lbPixelSize.Text = "Pixelméret:";
            // 
            // lbVertical
            // 
            this.lbVertical.AutoSize = true;
            this.lbVertical.Location = new System.Drawing.Point(27, 69);
            this.lbVertical.Name = "lbVertical";
            this.lbVertical.Size = new System.Drawing.Size(65, 13);
            this.lbVertical.TabIndex = 3;
            this.lbVertical.Text = "Függőleges:";
            // 
            // nTbHMax
            // 
            this.nTbHMax.AllowControls = true;
            this.nTbHMax.AllowDecimalSeparator = true;
            this.nTbHMax.AllowDigits = true;
            this.nTbHMax.AllowLowers = false;
            this.nTbHMax.AllowSymbols = false;
            this.nTbHMax.AllowUppers = false;
            this.nTbHMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nTbHMax.Location = new System.Drawing.Point(181, 41);
            this.nTbHMax.Name = "nTbHMax";
            this.nTbHMax.Size = new System.Drawing.Size(80, 20);
            this.nTbHMax.TabIndex = 2;
            // 
            // lbHorizontal
            // 
            this.lbHorizontal.AutoSize = true;
            this.lbHorizontal.Location = new System.Drawing.Point(34, 43);
            this.lbHorizontal.Name = "lbHorizontal";
            this.lbHorizontal.Size = new System.Drawing.Size(58, 13);
            this.lbHorizontal.TabIndex = 1;
            this.lbHorizontal.Text = "Vízszintes:";
            // 
            // nTbHMin
            // 
            this.nTbHMin.AllowControls = true;
            this.nTbHMin.AllowDecimalSeparator = true;
            this.nTbHMin.AllowDigits = true;
            this.nTbHMin.AllowLowers = false;
            this.nTbHMin.AllowSymbols = false;
            this.nTbHMin.AllowUppers = false;
            this.nTbHMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nTbHMin.Location = new System.Drawing.Point(95, 41);
            this.nTbHMin.Name = "nTbHMin";
            this.nTbHMin.Size = new System.Drawing.Size(80, 20);
            this.nTbHMin.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pBPreView);
            this.groupBox1.Location = new System.Drawing.Point(306, 67);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(280, 282);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Előnézet";
            // 
            // pBPreView
            // 
            this.pBPreView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pBPreView.Location = new System.Drawing.Point(12, 16);
            this.pBPreView.Name = "pBPreView";
            this.pBPreView.Size = new System.Drawing.Size(256, 256);
            this.pBPreView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pBPreView.TabIndex = 1;
            this.pBPreView.TabStop = false;
            // 
            // gBLoad
            // 
            this.gBLoad.Controls.Add(this.tBFilePath);
            this.gBLoad.Controls.Add(this.bnLoad);
            this.gBLoad.Location = new System.Drawing.Point(6, 6);
            this.gBLoad.Name = "gBLoad";
            this.gBLoad.Size = new System.Drawing.Size(580, 55);
            this.gBLoad.TabIndex = 0;
            this.gBLoad.TabStop = false;
            this.gBLoad.Text = "Adatforrás";
            // 
            // tBFilePath
            // 
            this.tBFilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tBFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tBFilePath.Location = new System.Drawing.Point(126, 17);
            this.tBFilePath.Name = "tBFilePath";
            this.tBFilePath.Size = new System.Drawing.Size(445, 26);
            this.tBFilePath.TabIndex = 1;
            // 
            // bnLoad
            // 
            this.bnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.bnLoad.Location = new System.Drawing.Point(54, 15);
            this.bnLoad.Name = "bnLoad";
            this.bnLoad.Size = new System.Drawing.Size(60, 30);
            this.bnLoad.TabIndex = 0;
            this.bnLoad.Text = "&Fájl";
            this.bnLoad.UseVisualStyleBackColor = true;
            this.bnLoad.Click += new System.EventHandler(this.bnLoad_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.scalablePictureBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(592, 384);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Teljes nézet";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // scalablePictureBox
            // 
            this.scalablePictureBox.AutoSize = true;
            this.scalablePictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scalablePictureBox.Location = new System.Drawing.Point(3, 3);
            this.scalablePictureBox.Name = "scalablePictureBox";
            this.scalablePictureBox.Size = new System.Drawing.Size(586, 378);
            this.scalablePictureBox.TabIndex = 4;
            // 
            // RsgControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tBGrid);
            this.DoubleBuffered = true;
            this.Name = "RsgControl";
            this.Size = new System.Drawing.Size(600, 410);
            this.tBGrid.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.gBPalette.ResumeLayout(false);
            this.gBPalette.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBPalette)).EndInit();
            this.gBRegion.ResumeLayout(false);
            this.gBRegion.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pBPreView)).EndInit();
            this.gBLoad.ResumeLayout(false);
            this.gBLoad.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tBGrid;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private QAlbum.ScalablePictureBox scalablePictureBox;
        private System.Windows.Forms.GroupBox gBLoad;
        private System.Windows.Forms.TextBox tBFilePath;
        private System.Windows.Forms.Button bnLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pBPreView;
        private System.Windows.Forms.GroupBox gBRegion;
        private System.Windows.Forms.GroupBox gBPalette;
        private System.Windows.Forms.PictureBox pBPalette;
        private System.Windows.Forms.Button bnPalette;
        private System.Windows.Forms.Button bnApplay;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lbProgress;
        private NumericTextBox.NumericTextBox nTbHMin;
        private NumericTextBox.NumericTextBox nTbPsY;
        private NumericTextBox.NumericTextBox nTbVMax;
        private NumericTextBox.NumericTextBox nTbPsX;
        private NumericTextBox.NumericTextBox nTbVMin;
        private System.Windows.Forms.Label lbPixelSize;
        private System.Windows.Forms.Label lbVertical;
        private NumericTextBox.NumericTextBox nTbHMax;
        private System.Windows.Forms.Label lbHorizontal;
        private System.Windows.Forms.Button bnReCalc;
        private NumericTextBox.NumericTextBox nTbMax;
        private NumericTextBox.NumericTextBox nTbMin;
    }
}
