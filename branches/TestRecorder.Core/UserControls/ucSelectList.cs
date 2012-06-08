using System.Windows.Forms;
using TestRecorder.Core.Actions;
using TestRecorder.Core;

namespace TestRecorder.UserControls
{
    public partial class ucSelectList : ucBaseElement
    {
        public ucSelectList() : base()
        {
            InitializeComponent();
        }

        public override ActionBase Action
        {
            get
            {
                if (base.Action == null) return null;

                var action = (ActionSelectList)base.Action;
                action.Context.FindMechanism = GuiToObject();
                action.SelectedValue = txtSelection.Text;
                action.Regex = chkIsRegex.Checked;
                action.ByValue = chkByValue.Checked;
                return base.Action;
            }
            set
            {
                var action = (ActionSelectList) value;
                base.Action = action;

                txtSelection.Text = action.SelectedValue;
                chkIsRegex.Checked = action.Regex;
                chkByValue.Checked = action.ByValue;
                ObjectToGui(action);
            }
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
