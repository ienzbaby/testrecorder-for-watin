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
    public partial class ucSleep : ucBaseEditor
    {
        public ucSleep()
        {
            InitializeComponent();
        }

        public override ActionBase Action
        {
            get
            {
                if (base.Action == null) return null;
                var action = (ActionSleep)base.Action;
                action.SleepTime = Convert.ToInt32(numSleep.Value);
                return base.Action;
            }
            set
            {
                var action = (ActionSleep)value;
                base.Action = action;

                numSleep.Value = action.SleepTime;
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
