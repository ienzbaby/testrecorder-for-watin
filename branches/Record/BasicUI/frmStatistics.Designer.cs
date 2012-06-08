namespace TestRecorder
{
    partial class frmStatistics
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
            this.gvMain = new System.Windows.Forms.DataGridView();
            this.Page = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Count = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AvgTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastStartTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastEndTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gvMain)).BeginInit();
            this.SuspendLayout();
            // 
            // gvMain
            // 
            this.gvMain.AllowUserToAddRows = false;
            this.gvMain.AllowUserToDeleteRows = false;
            this.gvMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gvMain.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.gvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvMain.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Page,
            this.Count,
            this.AvgTime,
            this.LastStartTime,
            this.LastEndTime});
            this.gvMain.Location = new System.Drawing.Point(12, 12);
            this.gvMain.Name = "gvMain";
            this.gvMain.ReadOnly = true;
            this.gvMain.RowTemplate.Height = 23;
            this.gvMain.Size = new System.Drawing.Size(739, 242);
            this.gvMain.TabIndex = 0;
            // 
            // Page
            // 
            this.Page.DataPropertyName = "Page";
            this.Page.HeaderText = "Page";
            this.Page.Name = "Page";
            this.Page.ReadOnly = true;
            this.Page.Width = 250;
            // 
            // Count
            // 
            this.Count.DataPropertyName = "Count";
            this.Count.HeaderText = "Count";
            this.Count.Name = "Count";
            this.Count.ReadOnly = true;
            // 
            // AvgTime
            // 
            this.AvgTime.DataPropertyName = "AvgTime";
            this.AvgTime.HeaderText = "AvgTime";
            this.AvgTime.Name = "AvgTime";
            this.AvgTime.ReadOnly = true;
            // 
            // LastStartTime
            // 
            this.LastStartTime.DataPropertyName = "LastStartTime";
            this.LastStartTime.HeaderText = "LastStartTime";
            this.LastStartTime.Name = "LastStartTime";
            this.LastStartTime.ReadOnly = true;
            this.LastStartTime.Width = 120;
            // 
            // LastEndTime
            // 
            this.LastEndTime.DataPropertyName = "LastEndTime";
            this.LastEndTime.HeaderText = "LastEndTime";
            this.LastEndTime.Name = "LastEndTime";
            this.LastEndTime.ReadOnly = true;
            this.LastEndTime.Width = 120;
            // 
            // frmStatistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 266);
            this.Controls.Add(this.gvMain);
            this.Name = "frmStatistics";
            this.Text = "Page Statistics";
            this.Load += new System.EventHandler(this.frmStatistics_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmStatistics_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.gvMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gvMain;
        private System.Windows.Forms.DataGridViewTextBoxColumn Page;
        private System.Windows.Forms.DataGridViewTextBoxColumn Count;
        private System.Windows.Forms.DataGridViewTextBoxColumn AvgTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastStartTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastEndTime;

    }
}