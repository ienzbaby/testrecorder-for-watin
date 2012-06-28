using System;
using System.Windows.Forms;
using IfacesEnumsStructsClasses;
using System.Data;
using System.Collections.Generic;
using TestRecorder.Tools;

namespace TestRecorder
{
    public partial class frmStatistics : Form
    {
        private string m_CurOp = string.Empty;
        public frmStatistics()
        {
            InitializeComponent();
        } 
        private void frmStatistics_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
        public void BindData(List<VisitPage> lPages)
        {
            BindingCollection<VisitPage> sortPages = new BindingCollection<VisitPage>(lPages);
            this.gvMain.AutoGenerateColumns = false;
            this.gvMain.DataSource =sortPages;
            this.gvMain.Refresh();
        }
        private void frmStatistics_Load(object sender, EventArgs e)
        {
            //Add some icons
            Icon = FormHelper.BitmapToIcon(5);
        }
    }
}