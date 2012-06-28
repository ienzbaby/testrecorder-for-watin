using System;
using System.Reflection;
using System.Web;
using System.Windows.Forms;
using IfacesEnumsStructsClasses;
using TestRecorder.Core;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using TestRecorder.Tools;

namespace TestRecorder
{
    public partial class frmMain : Form
    {
        //private bool controlKeyDown;
        private Queue<DateTime> qVisits = new Queue<DateTime>(1000);
        private List<VisitPage> lPages = new List<VisitPage>(1000);

        private IHTMLElement ElementLast;
        void cEXWB1_WBMouseMove(object sender, csExWB.HTMLMouseEventArgs e)
        {
            if (tsbClickElement.Checked)
            {
                cEXWB1.Cursor = Cursors.Cross;
                if (e.SrcElement != ElementLast)
                {
                    ElementLast = e.SrcElement;
                    HighlightElement(e.SrcElement);
                }
            }
        }

        void cEXWB1_WBLButtonDown(object sender, csExWB.HTMLMouseEventArgs e)
        {
            if (tsbClickElement.Checked)
            {
                cEXWB1.Cursor = Cursors.Cross;
                e.Handled = true;
            }
        }

        void cEXWB1_WBLButtonUp(object sender, csExWB.HTMLMouseEventArgs e)
        {
            if (tsbClickElement.Checked)
            {
                cmenuElement.Show(cEXWB1.PointToScreen(e.ClientPoint));
                cEXWB1.Cursor = Cursors.Cross;
                e.Handled = true;
            }
            else
            {
                xMouse = e.ClientX;
                yMouse = e.ClientY;
            }
        }
        void cEXWB1_TitleChange(object sender, csExWB.TitleChangeEventArgs e)
        {
            if (sender != _CurWB)
                return;
            Text = string.Format("Test Recorder BETA {0} - {1}", Assembly.GetExecutingAssembly().GetName().Version, e.title);
        }

        void cEXWB1_StatusTextChange(object sender, csExWB.StatusTextChangeEventArgs e)
        {
            if (sender != _CurWB)
                return;
            if (e.text.Length > 0) Status = e.text;
            tsStatus.Text = e.text;
        }

        void cEXWB1_CommandStateChange(object sender, csExWB.CommandStateChangeEventArgs e)
        {
            if (sender != _CurWB)
                return;
            try
            {
                if (e.command == CommandStateChangeConstants.CSC_NAVIGATEBACK)
                    tsBtnBack.Enabled = e.enable;
                else if (e.command == CommandStateChangeConstants.CSC_NAVIGATEFORWARD)
                    tsBtnForward.Enabled = e.enable;
            }
            catch (Exception ee)
            {
                if (_CurWB != null)
                    FormHelper.FrmLog.AppendToLog(_CurWB.Name + "_CommandStateChange\r\n" + ee);
                else
                    FormHelper.FrmLog.AppendToLog("cEXWB1_CommandStateChange\r\n" + ee);
            }
        }

        void cEXWB1_DocumentComplete(object sender, csExWB.DocumentCompleteEventArgs e)
        {
            try
            {
                if (e.istoplevel)
                {
                    var pWb = (csExWB.cEXWB)sender;
                    ToolStripButton btn = FindTab(pWb.Name);

                    btn.Text = pWb.GetTitle(true);
                    if (btn.Text.Length == 0)
                    {
                        btn.Text = Blank;
                    }
                    else if (btn.Text.Length > MaxTextLen)
                    {
                        btn.Text = btn.Text.Substring(0, MaxTextLen) + "...";
                    }
                    btn.ToolTipText = HttpUtility.UrlDecode(e.url);
                    this.CalcVisitStats(e);

                    // Web page is complete, so load the WatiN, DOM, and Source                    
                    WatiN.Core.Settings.Instance.AutoStartDialogWatcher = false;
                    lastelement = null;

                    LoadDOM(_CurWB.WebbrowserObject.Document);
                    LoadWatinTree(watinIE);

                    if (sender == _CurWB)
                    {
                        comboURL.Text = btn.ToolTipText;
                        pWb.SetFocus();
                    }
                }
            }
            catch (Exception ee)
            {
                if (_CurWB != null)
                    FormHelper.FrmLog.AppendToLog("Error:" + _CurWB.Name + "_DocumentComplete\r\n" + ee);
                else
                    FormHelper.FrmLog.AppendToLog("Error:cEXWBxx_DocumentComplete\r\n" + ee);
            }
        }
        private void CalcVisitStats(csExWB.DocumentCompleteEventArgs e)
        {
            if (e.url.Length == 0 || e.url.Equals(AboutBlank)) return;

            VisitPage page=lPages.Find(delegate(VisitPage p){
                return p.Page.Equals(e.url.ToLower());
            });
            if (page == null)
            {
                page = new VisitPage(e.url.ToLower());
                lPages.Add(page);
            }
            DateTime dtStart = DateTime.Now;
            if (qVisits.Count > 0) { dtStart = qVisits.Dequeue(); };
            DateTime dtEnd = DateTime.Now;

            page.LastStartTime = dtStart;
            page.LastEndTime = dtEnd;
            ++page.Count;
            page.Times = +(long)(dtEnd.Subtract(dtStart)).TotalMilliseconds;
        }

        void cEXWB1_WBEvaluteNewWindow(object sender, csExWB.EvaluateNewWindowEventArgs e)
        {
            if ((e.flags & NWMF.HTMLDIALOG) == NWMF.HTMLDIALOG)
            {
                if (MessageBox.Show(Properties.Resources.showDialogBug, Properties.Resources.KnownBug, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
            else if ( tsbClickElement.Checked == false)
            {
                m_frmPopup.Show();
                wsManager.AddPopup(watinIE, wsManager.Settings.PopupIEName, e.url);
                m_frmPopup.SetURL(e.url);
                e.Cancel = true;
            }
        }

        void cEXWB1_NewWindow2(object sender, csExWB.NewWindow2EventArgs e)
        {
            //if (!m_frmPopup.Visible) { m_frmPopup.Show(this);}

            var pWB = (csExWB.cEXWB)sender;
            if ((pWB != null) && (!pWB.RegisterAsBrowser))
            {
                pWB.RegisterAsBrowser = true;
            }

            //m_frmPopup.AssignBrowserObject(ref e.browser);
        }

        void cEXWB1_NewWindow3(object sender, csExWB.NewWindow3EventArgs e)
        {
            //if (!m_frmPopup.Visible){   m_frmPopup.Show(this);}

            var pWB = (csExWB.cEXWB)sender;
            if ((pWB != null) && (!pWB.RegisterAsBrowser))
            {
                pWB.RegisterAsBrowser = true;
            }

            //m_frmPopup.AssignBrowserObject(ref e.browser);
        }

        void cEXWB1_ProgressChange(object sender, csExWB.ProgressChangeEventArgs e)
        {
            if (sender != _CurWB) return;

            if ((e.progress == -1) || (e.progressmax == e.progress))
            {
                tsProgress.Value = 0; // 100;
                tsProgress.Maximum = 0;
                return;
            }
            if (e.progressmax > 0 && e.progress > 0 && e.progress < e.progressmax)
            {
                tsProgress.Maximum = e.progressmax;
                tsProgress.Value = e.progress; //* 100) / tsProgress.Maximum;
            }
        }

        void cEXWB1_ScriptError(object sender, csExWB.ScriptErrorEventArgs e)
        {
            string wbname = (_CurWB != null) ? _CurWB.Name : "cEXWBxx";
            FormHelper.FrmLog.AppendToLog(wbname + "_ScriptError - Continuing to run scripts");
            FormHelper.FrmLog.AppendToLog("Error Message" + e.errorMessage + "\r\nLine Number" + e.lineNumber);
        }

        void cEXWB1_SetSecureLockIcon(object sender, csExWB.SetSecureLockIconEventArgs e)
        {
            if (sender != _CurWB) return;
            UpdateSecureLockIcon(e.securelockicon);
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        public const int WM_CLOSE = 0x10;

        void cEXWB1_WBDocHostShowUIShowMessage(object sender, csExWB.DocHostShowUIShowMessageEventArgs e)
        {
            // simple alert dialog
            if (e.type == 48)
            {
                if (HandleAlert == DialogResult.None)
                {
                    MessageBox.Show(e.text, e.caption);
                    wsManager.AddAlertHandler(watinIE);
                }
                else
                {
                    HandleAlert = DialogResult.None;
                }
            }
            // confirm dialog
            else if (e.type == 33)
            {
                if (HandleConfirm == DialogResult.None)
                {
                    DialogResult result = MessageBox.Show(e.text, e.caption, MessageBoxButtons.OKCancel);
                    wsManager.AddConfirmHandler(watinIE, result);
                    e.result = Convert.ToInt32(result);
                }
                else
                {
                    e.result = Convert.ToInt32(HandleConfirm);
                    HandleConfirm = DialogResult.None;
                }
            }
            e.handled = true;
        }

        void cEXWB1_WBSecurityProblem(object sender, csExWB.SecurityProblemEventArgs e)
        {
            //if (e.problem != WinInetErrors.HTTP_REDIRECT_NEEDS_CONFIRMATION)
            //{
            //    e.handled = true;
            //    e.retvalue = Hresults.E_ABORT;
            //}

            string wbname = (_CurWB != null) ? _CurWB.Name : "cEXWBxx";
            FormHelper.FrmLog.AppendToLog(wbname + "_WBSecurityProblem - Wininet Problem=" + e.problem);
        }

        void cEXWB1_WindowClosing(object sender, csExWB.WindowClosingEventArgs e)
        {
            //Ask, or read from users options what to do. For now
            e.Cancel = true;
            FormHelper.FrmLog.AppendToLog("frmMain_cEXWB1_WindowClosing");
        }

        void cEXWB1_WBKeyUp(object sender, csExWB.WBKeyUpEventArgs e)
        {
            if (e.keycode == Keys.ShiftKey)
            {
                shiftKeyDown = false;
            }
            else if (e.keycode == Keys.ControlKey)
            {
                //controlKeyDown = false;
            }
            else if (e.keycode == Keys.Enter)
            {
                e.handled = true;
                WriteKeys();
                wsManager.AddDirectionKey("{ENTER}");
            }
            else if (e.keycode == Keys.Tab)
            {
                //e.handled = true;
                //WriteKeys();
                //wscript.AddDirectionKey("{TAB}");
            }
            else if (e.keycode == Keys.Up)
            {
                e.handled = true;
                WriteKeys();
                wsManager.AddDirectionKey("{UP}");
            }
            else if (e.keycode == Keys.Down)
            {
                e.handled = true;
                WriteKeys();
                wsManager.AddDirectionKey("{DOWN}");
            }
            else if (e.keycode == Keys.Left)
            {
                e.handled = true;
                WriteKeys();
                wsManager.AddDirectionKey("{LEFT}");
            }
            else if (e.keycode == Keys.Right)
            {
                e.handled = true;
                WriteKeys();
                wsManager.AddDirectionKey("{RIGHT}");
            }
            else if (e.keycode == Keys.Back)
            {
                //e.handled = true;
                //WriteKeys();
                //wscript.AddDirectionKey("{BACKSPACE}");
            }
            else if (e.keycode == Keys.Delete)
            {
                e.handled = true;
                WriteKeys();
                wsManager.AddDirectionKey("{DEL}");
            }

            // update the current element line.
            // KeyEntryElement = cEXWB1.GetActiveElement();

        }

        void WriteKeys()
        {
            if (textActiveElement == null) return;
            // This means we're done with a cycle for TextActive keys.
            // Write it out
            try
            {
                bool result = textActiveElement.isTextEdit;
            }
            catch (UnauthorizedAccessException)
            {
                // on this case, it means that the element has been navigated "away"
                // so let's move on
                textActiveElement = null;
                return;
            }
            catch (Exception e)
            {
                throw (e);
            }
            wsManager.AddTyping(watinIE, textActiveElement);
        }

        void cEXWB1_WBKeyDown(object sender, csExWB.WBKeyDownEventArgs e)
        {
            if (e.virtualkey == Keys.Back || e.virtualkey == Keys.Delete
                || e.virtualkey == Keys.Up || e.virtualkey == Keys.Down
                || e.virtualkey == Keys.Left || e.virtualkey == Keys.Right)
            {
                return;
            }

            if (cEXWB1.GetActiveElement() != textActiveElement)
            {
                WriteKeys();
            }
            textActiveElement = cEXWB1.GetActiveElement();
            CloneActiveElementForKeys();

            if (e.keycode == Keys.ControlKey)
            {
                //controlKeyDown = true;
            }
            else if (e.keycode == Keys.ShiftKey)
            {
                shiftKeyDown = true;
            }
            else
            {
                if (e.keycode != Keys.Tab
                    && e.keycode != Keys.Back
                    && e.keycode != Keys.Delete
                    && e.keycode != Keys.Up
                    && e.keycode != Keys.Down
                    && e.keycode != Keys.Left
                    && e.keycode != Keys.Right)
                {
                    wsManager.AddKeys(textActiveElement, shiftKeyDown, e.keycode);
                    lastKeyTime = DateTime.Now;
                }
            }

            //Consume keys here, if needed
            try
            {
                if (e.virtualkey == Keys.ControlKey)
                {
                    switch (e.keycode)
                    {
                        case Keys.F:
                            m_frmFind.Show(this);
                            e.handled = true;
                            break;
                        case Keys.N:
                            AddNewBrowser(Blank, AboutBlank, string.Empty, true);
                            e.handled = true;
                            break;
                        case Keys.O:
                            AddNewBrowser(Blank, AboutBlank, string.Empty, true);
                            e.handled = true;
                            break;
                    }
                }
            }
            catch (Exception eex)
            {
                if (_CurWB != null)
                    FormHelper.FrmLog.AppendToLog(_CurWB.Name + "_WBKeyDown\r\n" + eex);
                else
                    FormHelper.FrmLog.AppendToLog("cEXWBxx_WBKeyDown\r\n" + eex);
            }
        }

        void cEXWB1_DownloadBegin(object sender)
        {
            if (sender != _CurWB)  return;

            if (this.qVisits.Count > 100) this.qVisits.Clear(); //====清除内存=====
            this.qVisits.Enqueue(DateTime.Now);
            tsProgress.Visible = true;
        }

        void cEXWB1_DownloadComplete(object sender)
        {

        }

        void cEXWB1_RefreshBegin(object sender)
        {
            /*
            try
            {
                csExWB.cEXWB pWB = (csExWB.cEXWB)sender;
                if( (CurWB != null) && (CurWB == pWB) )
                {
                    m_frmThumbs.SetLabelIcon(CurWB.Name, ImgAniProgress);
                }
            }
            catch (Exception eex)
            {
                if (CurWB != null)
                    AllForms.FrmLog.AppendToLog(CurWB.Name + "_RefreshBegin\r\n" + eex);
                else
                    AllForms.FrmLog.AppendToLog("cEXWBxx_RefreshBegin\r\n" + eex);
            }
            */
        }

        void cEXWB1_RefreshEnd(object sender)
        {
            //
        }

        void cEXWB1_FileDownload(object sender, csExWB.FileDownloadEventArgs e)
        {
            //Here is the easiest way to find out the download file name
            //Status is set in StatusTextChange event handler
            //After the user has clicked a downloadable link, we get a 
            //BeforeNavigate2 and then at least two calls to StatusTextChange
            //One containing a text such as below
            //Start downloading from site: http://www.codeproject.com/cs/media/cameraviewer/cv_demo.zip
            //and one that sends an empty string to clear the status text.
            //After the status calls, we should get this event fired.
            //AllForms.FrmLog.AppendToLog("cEXWBxx_FileDownload\r\n" + Status);

            //Here you can cancel the download and take over.
            //e.Cancel = true;
        }

        void cEXWB1_WBAuthenticate(object sender, csExWB.AuthenticateEventArgs e)
        {
            if (m_frmAuth.ShowDialogInternal(this) == DialogResult.OK)
            {
                //Default value of handled is false
                e.handled = true;
                //To pass a doamin as in a NTLM authentication scheme,
                //use this format: Username = Domain\username
                e.username = m_frmAuth.m_Username;
                e.password = m_frmAuth.m_Password;

                wsManager.AddNavigation(watinIE, watinIE.Url, e.username, e.password);

                //Default value of retValue is 0 or S_OK
            }
        }

        void cEXWB1_WBDragDrop(object sender, csExWB.WBDropEventArgs e)
        {
            if (e.dataobject == null)
                return;
            if (e.dataobject.ContainsText())
                FormHelper.FrmLog.AppendToLog("cEXWB1_WBDragDrop\r\n" + e.dataobject.GetText());
            else if (e.dataobject.ContainsFileDropList())
            {
                System.Collections.Specialized.StringCollection files = e.dataobject.GetFileDropList();
                if (files.Count > 1)
                    MessageBox.Show(Properties.Resources.CanNotDoMultiFileDrop);
                else
                {
                    if (_CurWB != null)
                        _CurWB.Navigate(files[0]);
                }
            }
            else
            {
                //Example of how to catch a dragdrop of a ListViewItem from frmCacheCookie form
                object obj = e.dataobject.GetData("WindowsForms10PersistentObject");
                if (obj != null && obj is ListViewItem)
                {
                    FormHelper.FrmLog.AppendToLog("cEXWB1_WBDragDrop\r\n" + ((ListViewItem)obj).Text);
                }

            }
        }
    }
}
