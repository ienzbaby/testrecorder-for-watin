using System;
using System.Windows.Forms;

namespace TestRecorder
{
    public partial class frmDynamicConfirm : Form
    {
        private string m_Result = string.Empty;
        public string DisplayConfirm(IWin32Window win, string msg, string text, string btn1, string btn2, string btn3, string btn4)
        {
            //Reset all
            m_Result = string.Empty;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
            //Which btn to setup

            Text = !string.IsNullOrEmpty(text) ? text : Properties.Resources.ConfirmationRequired;

            txtDisplay.Text = msg;

            if (!string.IsNullOrEmpty(btn1))                
            {
                button1.Visible = true;
                button1.Text = btn1;
            }
            if (!string.IsNullOrEmpty(btn2))
            {
                button2.Visible = true;
                button2.Text = btn2;
            }
            if (!string.IsNullOrEmpty(btn3))
            {
                button3.Visible = true;
                button3.Text = btn3;
            }
            if (!string.IsNullOrEmpty(btn4))
            {
                button4.Visible = true;
                button4.Text = btn4;
            }

            ShowDialog(win);

            return m_Result;
        }

        public frmDynamicConfirm()
        {
            InitializeComponent();
            Load += frmDynamicConfirm_Load;
            FormClosing += frmDynamicConfirm_FormClosing;
            button1.Click += button_Click;
            button2.Click += button_Click;
            button3.Click += button_Click;
            button4.Click += button_Click;
        }

        void button_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                var btn = sender as Button;
                m_Result = btn.Text;
                Hide();
            }
        }

        void frmDynamicConfirm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        void frmDynamicConfirm_Load(object sender, EventArgs e)
        {
            
        }
    }
}