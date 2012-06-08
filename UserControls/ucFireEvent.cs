using System.Windows.Forms;
using TestRecorder.Core.Actions;
using TestRecorder.Core;

namespace TestRecorder.UserControls
{
    public partial class ucFireEvent : ucBaseElement
    {
        public ucFireEvent() : base()
        {
            InitializeComponent();
        }

        public override ActionBase Action
        {
            get
            {
                if (base.Action == null) return null;
                var action = (ActionFireEvent)base.Action;
                action.Context.FindMechanism=GuiToObject();
                action.EventName = txtEventName.Text;
                action.NoWait = chkNoWait.Checked;

                action.EventParameters.Clear();
                foreach (var item in lbParameters.Items)
                {
                    string[] arrItem = item.ToString().Split("=".ToCharArray());
                    action.EventParameters.Add(arrItem[0], arrItem[1]);
                }
                return base.Action;
            }
            set
            {
                var action = (ActionFireEvent)value;
                base.Action = action;

                txtEventName.Text = action.EventName;
                chkNoWait.Checked = action.NoWait;
                for (int i = 0; i < action.EventParameters.Count; i++)
                {
                    string key = action.EventParameters.GetKey(i);
                    lbParameters.Items.Add(key + "=" + action.EventParameters[key]);
                }
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

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            lbParameters.Items.Add(txtParameterName.Text + "=" + txtParameterValue.Text);
        }

        private void btnDelete_Click(object sender, System.EventArgs e)
        {
            if (this.lbParameters.Items.Count > 0)
            {
                int i = this.lbParameters.SelectedIndex;
                this.lbParameters.Items.RemoveAt(i);
            }
        }
    }
}
