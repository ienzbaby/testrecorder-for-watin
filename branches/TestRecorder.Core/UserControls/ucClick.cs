using System.Windows.Forms;
using TestRecorder.Core.Actions;
using TestRecorder.Core;

namespace TestRecorder.UserControls
{
    public partial class ucClick : ucBaseElement
    {
        public ucClick() : base()
        {
            InitializeComponent();
        }

        public override ActionBase Action
        {
            get
            {
                if (base.Action == null) return null;
                var action = (ActionClick) base.Action;
                action.Context.FindMechanism = GuiToObject();
                action.NoWait = chkNoWait.Checked;
                return action;
            }
            set
            {
                var action = (ActionClick)value;
                base.Action =action;
                
                chkNoWait.Checked =action.NoWait;
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
