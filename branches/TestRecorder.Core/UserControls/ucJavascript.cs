using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using TestRecorder.Core.Actions;

namespace TestRecorder.UserControls
{
    public partial class ucJavascript : ucBaseEditor
    {
        public override ActionBase Action
        {
            get
            {
                if (base.Action == null) return null;
                var js = (JavascriptHandler)base.Action;
                js.Script = txtScript.Text;
                return js;
            }
            set
            {
                var action = (JavascriptHandler)value;
                base.Action = action;

                txtScript.Text = action.Script;
            }
        }


        public ucJavascript()
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