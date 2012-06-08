using System;
using System.Windows.Forms;
using TestRecorder.Core.Actions;

namespace TestRecorder.UserControls
{
    public partial class ucKey : ucBaseElement
    {
        public ucKey() : base()
        {
            InitializeComponent();
        }

        public override ActionBase Action
        {
            get
            {
                if (base.Action == null) return null;
                var action = (ActionKey) base.Action;
                action.Context.FindMechanism = GuiToObject();
                action.KeyToPress = Convert.ToChar(txtKeyCharacter.Text);
                action.KeyFunction = (ActionKey.KeyFunctions) Enum.Parse(typeof (ActionKey.KeyFunctions), ddlKeyFunction.SelectedItem.ToString());
                return base.Action;
            }
            set
            {
                var action = (ActionKey) value;
                base.Action = action; 
                
                txtKeyCharacter.Text = action.KeyToPress.ToString();
                ddlKeyFunction.SelectedItem = action.KeyFunction.ToString();
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
