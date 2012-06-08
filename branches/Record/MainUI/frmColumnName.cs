using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TestRecorder
{
    public partial class frmColumnName : Form
    {
        public frmColumnName()
        {
            InitializeComponent();
        }

        private void txtColumnName_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Regex.IsMatch(Convert.ToChar(e.KeyValue).ToString(), @"[a-zA-Z]") 
                && e.KeyCode != Keys.Delete
                && e.KeyCode != Keys.Back) e.SuppressKeyPress = true;
        }
    }
}
