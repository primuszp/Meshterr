namespace MeshTopology
{
    partial class FormMain
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.grControl = new GRV11.GRControl();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.gbSorMetszet = new System.Windows.Forms.GroupBox();
            this.tlPanelRight = new System.Windows.Forms.TableLayoutPanel();
            this.pbOszlopMetszet = new System.Windows.Forms.PictureBox();
            this.pbSorMetszet = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tlPanelLeft = new System.Windows.Forms.TableLayoutPanel();
            this.lbColumn = new System.Windows.Forms.ListBox();
            this.lbRow = new System.Windows.Forms.ListBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.stLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fájlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel = new System.Windows.Forms.Panel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.gbSorMetszet.SuspendLayout();
            this.tlPanelRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbOszlopMetszet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSorMetszet)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tlPanelLeft.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.panel.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // grControl
            // 
            this.grControl.BackColor = System.Drawing.Color.Black;
            this.grControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grControl.Location = new System.Drawing.Point(3, 3);
            this.grControl.Name = "grControl";
            this.grControl.Size = new System.Drawing.Size(568, 352);
            this.grControl.TabIndex = 0;
            this.grControl.Text = "OpenGL Control";
            this.grControl.OpenGLStarted += new GRV11.GRControl.DelegateOpenGLStarted(this.grControl_OpenGLStarted);
            this.grControl.DoubleClick += new System.EventHandler(this.grControl_DoubleClick);
            this.grControl.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.grControl_MouseWheel);
            this.grControl.Paint += new System.Windows.Forms.PaintEventHandler(this.grControl_Paint);
            this.grControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.grControl_MouseMove);
            this.grControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grControl_MouseDown);
            this.grControl.Resize += new System.EventHandler(this.grControl_Resize);
            // 
            // splitContainer
            // 
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Location = new System.Drawing.Point(0, 49);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.tabControl);
            this.splitContainer.Size = new System.Drawing.Size(729, 386);
            this.splitContainer.SplitterDistance = 142;
            this.splitContainer.SplitterWidth = 3;
            this.splitContainer.TabIndex = 1;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(582, 384);
            this.tabControl.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.grControl);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(574, 358);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Modelltér";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.gbSorMetszet);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(574, 358);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Metszet";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Enter += new System.EventHandler(this.tabPage2_Enter);
            // 
            // gbSorMetszet
            // 
            this.gbSorMetszet.Controls.Add(this.tlPanelRight);
            this.gbSorMetszet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbSorMetszet.Location = new System.Drawing.Point(129, 3);
            this.gbSorMetszet.Name = "gbSorMetszet";
            this.gbSorMetszet.Size = new System.Drawing.Size(442, 352);
            this.gbSorMetszet.TabIndex = 0;
            this.gbSorMetszet.TabStop = false;
            this.gbSorMetszet.Text = "Sor/Oszlop Metszet";
            // 
            // tlPanelRight
            // 
            this.tlPanelRight.ColumnCount = 1;
            this.tlPanelRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlPanelRight.Controls.Add(this.pbOszlopMetszet, 0, 1);
            this.tlPanelRight.Controls.Add(this.pbSorMetszet, 0, 0);
            this.tlPanelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlPanelRight.Location = new System.Drawing.Point(3, 16);
            this.tlPanelRight.Name = "tlPanelRight";
            this.tlPanelRight.RowCount = 2;
            this.tlPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlPanelRight.Size = new System.Drawing.Size(436, 333);
            this.tlPanelRight.TabIndex = 0;
            // 
            // pbOszlopMetszet
            // 
            this.pbOszlopMetszet.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbOszlopMetszet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbOszlopMetszet.Location = new System.Drawing.Point(3, 169);
            this.pbOszlopMetszet.Name = "pbOszlopMetszet";
            this.pbOszlopMetszet.Size = new System.Drawing.Size(430, 161);
            this.pbOszlopMetszet.TabIndex = 3;
            this.pbOszlopMetszet.TabStop = false;
            // 
            // pbSorMetszet
            // 
            this.pbSorMetszet.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbSorMetszet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbSorMetszet.Location = new System.Drawing.Point(3, 3);
            this.pbSorMetszet.Name = "pbSorMetszet";
            this.pbSorMetszet.Size = new System.Drawing.Size(430, 160);
            this.pbSorMetszet.TabIndex = 2;
            this.pbSorMetszet.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tlPanelLeft);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(126, 352);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Háló";
            // 
            // tlPanelLeft
            // 
            this.tlPanelLeft.ColumnCount = 1;
            this.tlPanelLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlPanelLeft.Controls.Add(this.lbColumn, 0, 1);
            this.tlPanelLeft.Controls.Add(this.lbRow, 0, 0);
            this.tlPanelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlPanelLeft.Location = new System.Drawing.Point(3, 16);
            this.tlPanelLeft.Name = "tlPanelLeft";
            this.tlPanelLeft.RowCount = 2;
            this.tlPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlPanelLeft.Size = new System.Drawing.Size(120, 333);
            this.tlPanelLeft.TabIndex = 0;
            // 
            // lbColumn
            // 
            this.lbColumn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbColumn.FormattingEnabled = true;
            this.lbColumn.Location = new System.Drawing.Point(3, 169);
            this.lbColumn.Name = "lbColumn";
            this.lbColumn.Size = new System.Drawing.Size(114, 160);
            this.lbColumn.TabIndex = 1;
            this.lbColumn.SelectedIndexChanged += new System.EventHandler(this.lbColumn_SelectedIndexChanged);
            // 
            // lbRow
            // 
            this.lbRow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbRow.FormattingEnabled = true;
            this.lbRow.Location = new System.Drawing.Point(3, 3);
            this.lbRow.Name = "lbRow";
            this.lbRow.Size = new System.Drawing.Size(114, 160);
            this.lbRow.TabIndex = 0;
            this.lbRow.SelectedIndexChanged += new System.EventHandler(this.lbRow_SelectedIndexChanged);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 435);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip.Size = new System.Drawing.Size(729, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // stLabel
            // 
            this.stLabel.Name = "stLabel";
            this.stLabel.Size = new System.Drawing.Size(118, 17);
            this.stLabel.Text = "toolStripStatusLabel1";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fájlToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(729, 24);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fájlToolStripMenuItem
            // 
            this.fájlToolStripMenuItem.Name = "fájlToolStripMenuItem";
            this.fájlToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fájlToolStripMenuItem.Text = "&Fájl";
            // 
            // panel
            // 
            this.panel.Controls.Add(this.toolStrip);
            this.panel.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel.Location = new System.Drawing.Point(0, 24);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(729, 25);
            this.panel.TabIndex = 4;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(729, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 40;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(729, 457);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FormMain";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.gbSorMetszet.ResumeLayout(false);
            this.tlPanelRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbOszlopMetszet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSorMetszet)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tlPanelLeft.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GRV11.GRControl grControl;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripStatusLabel stLabel;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox gbSorMetszet;
        private System.Windows.Forms.ToolStripMenuItem fájlToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tlPanelRight;
        private System.Windows.Forms.PictureBox pbOszlopMetszet;
        private System.Windows.Forms.PictureBox pbSorMetszet;
        private System.Windows.Forms.TableLayoutPanel tlPanelLeft;
        private System.Windows.Forms.ListBox lbColumn;
        private System.Windows.Forms.ListBox lbRow;
    }
}

