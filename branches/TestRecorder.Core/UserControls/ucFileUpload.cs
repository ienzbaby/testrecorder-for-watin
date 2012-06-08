using System;
using System.Windows.Forms;
using TestRecorder.Core.Actions;

namespace TestRecorder.UserControls
{
    public partial class ucFileUpload : ucBaseElement
    {
        public ucFileUpload()
            : base()
        {
            InitializeComponent();
        }

        public override ActionBase Action
        {
            get
            {
                if (base.Action == null) return null;
                var action = (ActionFileDialog)base.Action;
                action.Context.FindMechanism = GuiToObject();
                action.Filename = txtFile.Text;
                return base.Action;
            }
            set
            {
                var action = (ActionFileDialog)value;
                base.Action = action;

                txtFile.Text = action.Filename;
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