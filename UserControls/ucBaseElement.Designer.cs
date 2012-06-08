namespace TestRecorder.UserControls
{
    partial class ucBaseElement
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
            this.components = new System.ComponentModel.Container();
            this.gbFindElement = new System.Windows.Forms.GroupBox();
            this.gridElement = new SourceGrid.Grid();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnVerify = new System.Windows.Forms.Button();
            this.menuFindValue = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.abcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gbFindElement.SuspendLayout();
            this.panel2.SuspendLayout();
            this.menuFindValue.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbFindElement
            // 
            this.gbFindElement.Controls.Add(this.gridElement);
            this.gbFindElement.Controls.Add(this.panel2);
            this.gbFindElement.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbFindElement.Location = new System.Drawing.Point(0, 0);
            this.gbFindElement.Margin = new System.Windows.Forms.Padding(2);
            this.gbFindElement.Name = "gbFindElement";
            this.gbFindElement.Padding = new System.Windows.Forms.Padding(2);
            this.gbFindElement.Size = new System.Drawing.Size(580, 80);
            this.gbFindElement.TabIndex = 2;
            this.gbFindElement.TabStop = false;
            this.gbFindElement.Text = " Find Element ";
            // 
            // gridElement
            // 
            this.gridElement.AutoSize = true;
            this.gridElement.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.gridElement.ColumnsCount = 6;
            this.gridElement.CustomSort = true;
            this.gridElement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridElement.FixedRows = 1;
            this.gridElement.Location = new System.Drawing.Point(2, 15);
            this.gridElement.Margin = new System.Windows.Forms.Padding(2);
            this.gridElement.Name = "gridElement";
            this.gridElement.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridElement.RowsCount = 2;
            this.gridElement.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.gridElement.Size = new System.Drawing.Size(496, 63);
            this.gridElement.TabIndex = 15;
            this.gridElement.TabStop = true;
            this.gridElement.ToolTipText = "";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnVerify);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(498, 15);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(80, 63);
            this.panel2.TabIndex = 13;
            // 
            // btnVerify
            // 
            this.btnVerify.Location = new System.Drawing.Point(2, 5);
            this.btnVerify.Margin = new System.Windows.Forms.Padding(2);
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(76, 22);
            this.btnVerify.TabIndex = 4;
            this.btnVerify.Text = "Verify";
            this.btnVerify.UseVisualStyleBackColor = true;
            this.btnVerify.Visible = false;
            // 
            // menuFindValue
            // 
            this.menuFindValue.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.abcToolStripMenuItem,
            this.defToolStripMenuItem});
            this.menuFindValue.Name = "menuFindValue";
            this.menuFindValue.Size = new System.Drawing.Size(94, 48);
            // 
            // abcToolStripMenuItem
            // 
            this.abcToolStripMenuItem.Name = "abcToolStripMenuItem";
            this.abcToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.abcToolStripMenuItem.Text = "abc";
            // 
            // defToolStripMenuItem
            // 
            this.defToolStripMenuItem.Name = "defToolStripMenuItem";
            this.defToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.defToolStripMenuItem.Text = "def";
            // 
            // ucBaseElement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Controls.Add(this.gbFindElement);
            this.Name = "ucBaseElement";
            this.Size = new System.Drawing.Size(580, 220);
            this.gbFindElement.ResumeLayout(false);
            this.gbFindElement.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.menuFindValue.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnVerify;
        internal System.Windows.Forms.GroupBox gbFindElement;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ContextMenuStrip menuFindValue;
        private System.Windows.Forms.ToolStripMenuItem abcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defToolStripMenuItem;
        internal SourceGrid.Grid gridElement;
    }
}
