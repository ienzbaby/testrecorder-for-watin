namespace TestRecorder.UserControls
{
    partial class ucWait
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
            this.chkRegex = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ddlWaitType = new System.Windows.Forms.ComboBox();
            this.numTimeout = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAttribute = new System.Windows.Forms.TextBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkRegex
            // 
            this.chkRegex.AutoSize = true;
            this.chkRegex.Location = new System.Drawing.Point(195, 36);
            this.chkRegex.Name = "chkRegex";
            this.chkRegex.Size = new System.Drawing.Size(57, 17);
            this.chkRegex.TabIndex = 3;
            this.chkRegex.Text = "Regex";
            this.chkRegex.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(120, 104);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Wait Type";
            // 
            // ddlWaitType
            // 
            this.ddlWaitType.FormattingEnabled = true;
            this.ddlWaitType.Location = new System.Drawing.Point(182, 101);
            this.ddlWaitType.Name = "ddlWaitType";
            this.ddlWaitType.Size = new System.Drawing.Size(121, 21);
            this.ddlWaitType.TabIndex = 5;
            this.ddlWaitType.SelectedIndexChanged += new System.EventHandler(this.ddlWaitType_SelectedIndexChanged);
            // 
            // numTimeout
            // 
            this.numTimeout.Location = new System.Drawing.Point(182, 128);
            this.numTimeout.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numTimeout.Name = "numTimeout";
            this.numTimeout.Size = new System.Drawing.Size(54, 20);
            this.numTimeout.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(120, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Timeout";
            // 
            // txtAttribute
            // 
            this.txtAttribute.Location = new System.Drawing.Point(68, 8);
            this.txtAttribute.Name = "txtAttribute";
            this.txtAttribute.Size = new System.Drawing.Size(121, 20);
            this.txtAttribute.TabIndex = 8;
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(68, 34);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(121, 20);
            this.txtValue.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Attribute";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(459, 136);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(378, 136);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 12;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Value";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtAttribute);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.chkRegex);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txtValue);
            this.panel1.Location = new System.Drawing.Point(114, 149);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(255, 61);
            this.panel1.TabIndex = 15;
            // 
            // ucWait
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.ddlWaitType);
            this.Controls.Add(this.numTimeout);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ucWait";
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.numTimeout, 0);
            this.Controls.SetChildIndex(this.ddlWaitType, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkRegex;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ddlWaitType;
        private System.Windows.Forms.NumericUpDown numTimeout;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAttribute;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
    }
}
