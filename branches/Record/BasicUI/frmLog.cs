using System;
using System.Windows.Forms;
using System.IO;

namespace TestRecorder
{
    public partial class frmLog : Form
    {
        public frmLog()
        {
            InitializeComponent();
        }

        public void AppendToLog(string text)
        {
            try
            {
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.AppendText(Environment.NewLine + text + Environment.NewLine);
                txtLog.SelectionStart = txtLog.Text.Length;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                txtLog.Text = string.Empty;
            }
        }

        private void tsBtnClear_Click(object sender, EventArgs e)
        {
            txtLog.Text = string.Empty;
        }

        private void frmLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void tsBtnSaveLog_Click(object sender, EventArgs e)
        {
            if (FormHelper.ShowStaticSaveDialogForText(this) == DialogResult.OK)
            {
                using (var sw = new StreamWriter(FormHelper.DlgSave.FileName))
                {
                    sw.Write(txtLog.Text);
                }
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}