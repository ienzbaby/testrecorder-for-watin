using System.Windows.Forms;
using TestRecorder.Core.Actions;

namespace TestRecorder.UserControls
{
    public partial class ucNavigate : ucBaseEditor
    {
        public override ActionBase Action
        {
            get
            {
                if (base.Action == null) return null;
                var nav = (ActionNavigate) base.Action;
                nav.URL = txtURL.Text;
                nav.Username = txtUsername.Text;
                nav.Password = txtPassword.Text;
                return nav;
            }
            set
            {
                var action = (ActionNavigate)value;
                base.Action = action;

                txtURL.Text = action.URL;
                txtUsername.Text = action.Username;
                txtPassword.Text = action.Password;
            }
        }

        public ucNavigate()
        {
            InitializeComponent();
        }
        public void btnOK_Click(object sender, System.EventArgs e)
        {
            if (OnCloseEdtion != null) OnCloseEdtion(DialogResult.OK);
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            if (OnCloseEdtion != null) OnCloseEdtion(DialogResult.Cancel);
        }
    }
}
