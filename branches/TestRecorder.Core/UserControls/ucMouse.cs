using System;
using System.Windows.Forms;
using TestRecorder.Core.Actions;
using TestRecorder.Core;

namespace TestRecorder.UserControls
{
    public partial class ucMouse : ucBaseElement
    {
        public ucMouse() : base()
        {
            InitializeComponent();
        }

        public override ActionBase Action
        {
            get
            {
                if (base.Action == null) return null;
                var action = (ActionMouse)base.Action;
                action.Context.FindMechanism = GuiToObject();
                action.MouseFunction = (ActionMouse.MouseFunctions)Enum.Parse(typeof(ActionMouse.MouseFunctions), ddlMouseFunction.SelectedItem.ToString());
                return base.Action;
            }
            set
            {
                var action = (ActionMouse) value;
                base.Action = action;

                ddlMouseFunction.SelectedItem = action.MouseFunction.ToString();
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
