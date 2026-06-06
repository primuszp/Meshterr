namespace Meshterr
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
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.Label = new System.Windows.Forms.ToolStripStatusLabel();
            this.drawingArea = new Meshterr.DrawingArea();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fájlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rSGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RsgLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TinLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer
            // 
            // 
            // toolStripContainer.BottomToolStripPanel
            // 
            this.toolStripContainer.BottomToolStripPanel.Controls.Add(this.statusStrip);
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Controls.Add(this.drawingArea);
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(584, 318);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.Size = new System.Drawing.Size(584, 364);
            this.toolStripContainer.TabIndex = 0;
            this.toolStripContainer.Text = "toolStripContainer";
            // 
            // toolStripContainer.TopToolStripPanel
            // 
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.menuStrip);
            // 
            // statusStrip
            // 
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Label});
            this.statusStrip.Location = new System.Drawing.Point(0, 0);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(584, 22);
            this.statusStrip.TabIndex = 0;
            // 
            // Label
            // 
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(28, 17);
            this.Label.Text = "XYZ";
            // 
            // drawingArea
            // 
            this.drawingArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.drawingArea.Location = new System.Drawing.Point(0, 0);
            this.drawingArea.Name = "drawingArea";
            this.drawingArea.RenderOptions = Meshterr.RenderingType.Vertex;
            this.drawingArea.Size = new System.Drawing.Size(584, 318);
            this.drawingArea.TabIndex = 0;
            this.drawingArea.zFar = 1F;
            this.drawingArea.zNear = -1F;
            this.drawingArea.Zoom = 0.5F;
            // 
            // menuStrip
            // 
            this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fájlToolStripMenuItem,
            this.rSGToolStripMenuItem,
            this.tINToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(584, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fájlToolStripMenuItem
            // 
            this.fájlToolStripMenuItem.Name = "fájlToolStripMenuItem";
            this.fájlToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fájlToolStripMenuItem.Text = "&Fájl";
            // 
            // rSGToolStripMenuItem
            // 
            this.rSGToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RsgLoadToolStripMenuItem});
            this.rSGToolStripMenuItem.Name = "rSGToolStripMenuItem";
            this.rSGToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.rSGToolStripMenuItem.Text = "&RSG";
            // 
            // RsgLoadToolStripMenuItem
            // 
            this.RsgLoadToolStripMenuItem.Name = "RsgLoadToolStripMenuItem";
            this.RsgLoadToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.RsgLoadToolStripMenuItem.Text = "&Betöltés";
            this.RsgLoadToolStripMenuItem.Click += new System.EventHandler(this.RsgLoadToolStripMenuItem_Click);
            // 
            // tINToolStripMenuItem
            // 
            this.tINToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TinLoadToolStripMenuItem});
            this.tINToolStripMenuItem.Name = "tINToolStripMenuItem";
            this.tINToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.tINToolStripMenuItem.Text = "&TIN";
            // 
            // TinLoadToolStripMenuItem
            // 
            this.TinLoadToolStripMenuItem.Name = "TinLoadToolStripMenuItem";
            this.TinLoadToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.TinLoadToolStripMenuItem.Text = "&Betöltés";
            this.TinLoadToolStripMenuItem.Click += new System.EventHandler(this.TinLoadToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 364);
            this.Controls.Add(this.toolStripContainer);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Meshterr v0.1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer.ContentPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.PerformLayout();
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.StatusStrip statusStrip;
        private DrawingArea drawingArea;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fájlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rSGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RsgLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel Label;
        private System.Windows.Forms.ToolStripMenuItem tINToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TinLoadToolStripMenuItem;
    }
}