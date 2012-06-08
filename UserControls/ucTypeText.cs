using System.Windows.Forms;
using TestRecorder.Core.Actions;

namespace TestRecorder.UserControls
{
    public partial class ucTypeText : ucBaseElement
    {
        public ucTypeText()
        {
            InitializeComponent();
        }

        public override ActionBase Action
        {
            get
            {
                if (base.Action == null) return null;
                var action = (ActionTypeText) base.Action;
                action.Context.FindMechanism = GuiToObject();
                action.TextToType = txtTextToType.Text;
                action.ValueOnly = chkValueOnly.Checked;
                return action;
            }
            set
            {
                var action = (ActionTypeText) value;
                base.Action = action; 
                
                txtTextToType.Text = action.TextToType;
                chkValueOnly.Checked = action.ValueOnly;
                ObjectToGui(action);                
            }
        }
        private void btnDataReplacementType_Click(object sender, System.EventArgs e)
        {
            AddItemsToMenu(menuReplacement);
            menuReplacement.Show(btnDataReplacementType, 0, btnDataReplacementType.Height);
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
