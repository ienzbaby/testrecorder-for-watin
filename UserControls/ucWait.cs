using System;
using System.Windows.Forms;
using TestRecorder.Core.Actions;

namespace TestRecorder.UserControls
{
    public partial class ucWait : ucBaseElement
    {
        public ucWait() : base()
        {
            InitializeComponent();

            ddlWaitType.Items.Add("Exists");
            ddlWaitType.Items.Add("Removed");
            ddlWaitType.Items.Add("AttributeValue");
        }

        public override ActionBase Action
        {
            get
            {
                if (base.Action == null) return null;
                var action = (ActionWait)base.Action;
                action.Context.FindMechanism=GuiToObject();
                action.WaitType =
                    (ActionWait.WaitTypes)
                    Enum.Parse(typeof (ActionWait.WaitTypes), ddlWaitType.SelectedItem.ToString());
                action.AttributeName = txtAttribute.Text;
                action.AttributeValue = txtValue.Text;
                action.AttributeRegex = chkRegex.Checked;
                action.WaitTimeout = Convert.ToInt32(numTimeout.Value);
                return base.Action;
            }
            set
            {
                var action = (ActionWait)value;
                base.Action = action;

                ddlWaitType.SelectedItem = action.WaitType.ToString();
                txtAttribute.Text = action.AttributeName;
                txtValue.Text = action.AttributeValue;
                chkRegex.Checked = action.AttributeRegex;
                if (action.WaitTimeout == 0) action.WaitTimeout = WatiN.Core.Settings.WaitUntilExistsTimeOut;
                numTimeout.Value = action.WaitTimeout;
                ObjectToGui(action);
            }
        }
        private void ddlWaitType_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel1.Visible = ddlWaitType.SelectedItem.ToString() == "AttributeValue";
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
