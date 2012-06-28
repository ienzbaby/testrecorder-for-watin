using System.Windows.Forms;
using System;

namespace TestRecorder
{
    public partial class frmWaitTime : Form
    {
        public int WaitTime;

        public frmWaitTime()
        {
            InitializeComponent();
        }

        void frmWaitTime_Shown(object sender, System.EventArgs e)
        {
            cbWaitTime.Text = WaitTime.ToString();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            WaitTime = int.Parse(cbWaitTime.Text);
        }
     }
}
