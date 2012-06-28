using System;
using System.Windows.Forms;

namespace TestRecorder
{
    public partial class frmAuthenticate : Form
    {
        private bool m_AuthSuccess;
        public string m_Username = string.Empty;
        public string m_Password = string.Empty;

        private void ResetAllFields()
        {
            m_AuthSuccess = false;
            m_Username = string.Empty;
            m_Password = string.Empty;
            txtUserName.Text = string.Empty;
            txtPassword.Text = string.Empty;
            chkViewPassword.Text = Properties.Resources.ShowPassword;
            txtPassword.PasswordChar = '*';
        }

        public frmAuthenticate()
        {
            InitializeComponent();
        }

        public DialogResult ShowDialogInternal(IWin32Window owner)
        {
            ResetAllFields();
            ShowDialog(owner);
            return (m_AuthSuccess) ? DialogResult.OK : DialogResult.Cancel;
        }

        private void chkViewPassword_Click(object sender, EventArgs e)
        {
            if (chkViewPassword.Checked)
            {
                chkViewPassword.Text = Properties.Resources.HidePassword;
                txtPassword.PasswordChar = new char();
            }
            else
            {
                chkViewPassword.Text = Properties.Resources.ShowPassword;
                txtPassword.PasswordChar = '*';
            }
        }

        private void frmAuthenticate_Load(object sender, EventArgs e)
        {
            Icon = FormHelper.BitmapToIcon(14);
        }

        private void frmAuthenticate_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            m_AuthSuccess = true;
            m_Username = txtUserName.Text;
            m_Password = txtPassword.Text;
            Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //Just hide
            Hide();
        }
    }
}