using System;
using System.Windows.Forms;
using IfacesEnumsStructsClasses;
using TestRecorder.Core;
using TestRecorder.Tools;

namespace TestRecorder
{
    public partial class frmPopup : Form
    {
        //private string PopupName = "";
        public PopupResultEvent OnPopupClose;
        private string Url = "";

        public frmPopup()
        {
            InitializeComponent();
        }

        private void frmPopup_Load(object sender, EventArgs e)
        {
            //cEXWB1.RegisterAsBrowser = true;
            cEXWB1.NavToBlank();
        }

        public void SetURL(string url)
        {
            this.Url = url;
            cEXWB1.Navigate(url);
        }
        private void frmPopup_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    if (this.OnPopupClose != null) this.OnPopupClose(DialogResult.Cancel,Url,cEXWB1.DocumentTitle);
                    if (cEXWB1 != null) cEXWB1.NavToBlank();
                    e.Cancel = true;
                    Hide();
                }
            }
            catch
            {
                // dump it
            }
        }
    }
}