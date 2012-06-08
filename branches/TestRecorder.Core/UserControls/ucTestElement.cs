using System;
using System.Windows.Forms;
using TestRecorder.Core.Actions;

namespace TestRecorder.UserControls
{
    public partial class ucTestElement : ucBaseElement
    {
        public ucTestElement() : base()
        {
            InitializeComponent();
        }

        public override ActionBase Action
        {
            get
            {
                if (base.Action == null) return null;
                var action = (ActionTestElement)base.Action;
                action.Context.FindMechanism = GuiToObject();
                action.TestToPerform =
                    (ActionTestElement.AvailableTests)
                    Enum.Parse(typeof(ActionTestElement.AvailableTests), ddlTestToPerform.SelectedItem.ToString());
                action.TestingProperty = txtProperty.Text;
                action.TestingValue = txtValue.Text;
                action.ExceptionMessage = txtMessage.Text;
                return base.Action;
            }
            set
            {
                var action = (ActionTestElement) value;
                base.Action = action; 
                
                txtProperty.Text = action.TestingProperty;
                txtValue.Text = action.TestingValue;
                txtMessage.Text = action.ExceptionMessage;
                ddlTestToPerform.SelectedItem = action.TestToPerform.ToString();
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
