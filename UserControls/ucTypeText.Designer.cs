namespace TestRecorder.UserControls
{
    partial class ucTypeText
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucTypeText));
            this.txtTextToType = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDataReplacementType = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkValueOnly = new System.Windows.Forms.CheckBox();
            this.menuReplacement = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtTextToType
            // 
            this.txtTextToType.Location = new System.Drawing.Point(80, 7);
            this.txtTextToType.Name = "txtTextToType";
            this.txtTextToType.Size = new System.Drawing.Size(256, 20);
            this.txtTextToType.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Text To Type";
            // 
            // btnDataReplacementType
            // 
            this.btnDataReplacementType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDataReplacementType.ImageKey = "Database.bmp";
            this.btnDataReplacementType.ImageList = this.imageList1;
            this.btnDataReplacementType.Location = new System.Drawing.Point(340, 5);
            this.btnDataReplacementType.Margin = new System.Windows.Forms.Padding(2);
            this.btnDataReplacementType.Name = "btnDataReplacementType";
            this.btnDataReplacementType.Size = new System.Drawing.Size(18, 22);
            this.btnDataReplacementType.TabIndex = 11;
            this.btnDataReplacementType.UseVisualStyleBackColor = true;
            this.btnDataReplacementType.Click += new System.EventHandler(this.btnDataReplacementType_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(486, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(405, 7);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 12;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkValueOnly);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.txtTextToType);
            this.panel1.Controls.Add(this.btnDataReplacementType);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 80);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(580, 51);
            this.panel1.TabIndex = 14;
            // 
            // chkValueOnly
            // 
            this.chkValueOnly.AutoSize = true;
            this.chkValueOnly.Location = new System.Drawing.Point(80, 31);
            this.chkValueOnly.Name = "chkValueOnly";
            this.chkValueOnly.Size = new System.Drawing.Size(77, 17);
            this.chkValueOnly.TabIndex = 15;
            this.chkValueOnly.Text = "Value Only";
            this.chkValueOnly.UseVisualStyleBackColor = true;
            // 
            // menuReplacement
            // 
            this.menuReplacement.Name = "menuReplacement";
            this.menuReplacement.Size = new System.Drawing.Size(61, 4);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Fuchsia;
            this.imageList1.Images.SetKeyName(0, "Database.bmp");
            // 
            // ucTypeText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "ucTypeText";
            this.Size = new System.Drawing.Size(580, 154);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtTextToType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDataReplacementType;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ContextMenuStrip menuReplacement;
        private System.Windows.Forms.CheckBox chkValueOnly;
        private System.Windows.Forms.ImageList imageList1;
    }
}
