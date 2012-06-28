using System;
using System.Windows.Forms;

namespace TestRecorder
{
    public partial class frmEditAction : Form
    {
        public frmEditAction()
        {
            InitializeComponent();

            ddlElementType.SelectedIndex = 0;
            ddlCompare.SelectedIndex = 0;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //((frmMain) Parent).CloseEditAction();
        }

        private void DBFieldShow_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null) contextMenuStrip1.Show(btn, 0, btn.Height);
        }
    }
}
