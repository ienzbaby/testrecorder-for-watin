using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Windows.Forms;
using IfacesEnumsStructsClasses;
using TestRecorder.Core;
using TestRecorder.Core.Actions;
using TestRecorder.UserControls;
using WatiN.Core.Exceptions;
using System.Text;
using TestRecorder.Tools;

namespace TestRecorder
{
    /// <summary>
    /// A simple multi tab simulated webbrowser control which demonstrates the usage of csEXWB control with some extras.
    /// 
    /// The demo pretty much covers all the basics and most of the advanced functionality that the control offers. It also includes a complete DOM
    /// viewer, cache and cookie explorers, thumb navigation, document information viewwer, loading and displaying favorites in dynamic
    /// menu, Popup, authentication and find handlers, a functional HTML editor, ....
    /// 
    /// A bit of details:
    /// In the frmMain, each browser has a corresponding toolstripbutton acting as a tab and a menu item for tab switching which displays
    /// number of open tabs as well. In the frmThumbs, each browser has a corresponding label, and picturebox.
    /// 
    /// The name of all the webbrowser corresponding controls are identical! All use the webbrowser instance Name.
    /// Any new browser can be deleted but the first one which was placed on the form in design time. Kept for making things a bit easier!
    /// 
    /// The rest is a matter of synchronizing addition, removal, and switching among webbrowser instances. Simple enough.
    /// 
    /// All the images, except for progres_spinner.gif, used in this project are coming from IeToolbar.bmp image strip. They are in 
    /// SetupImages() method into a static imagelist which then can be shared by all project forms and controls.
    /// </summary>
    public partial class frmMain : Form
    {
        internal frmAuthenticate m_frmAuth = new frmAuthenticate();
        private static int Count =1;

        #region Local Variables
        private DialogResult HandleAlert = DialogResult.None;
        private DialogResult HandleConfirm = DialogResult.None; 
        //For mouse click location
        private int xMouse;
        private int yMouse;
        //public int mouseButtons;

        private ScriptManager wsManager ;
        private BrowserWindow watinIE;
        private bool highlightingElements;

        private const string AboutBlank = "about:blank";
        private const string Blank = "Blank";
        private csExWB.cEXWB _CurWB; //Current WB
        private int IdxCurTab;       //Current Tab index
        private int IdxCountWB = 1;  //WB Count
        private const int MaxTextLen = 15; //Maximum len of text displayed in tabs,...
        private ToolStripButton m_tsBtnFirstTab; //for reference
        private ToolStripButton m_tsBtnctxMnu; //For reference when rClicked on a toolstripbutton
        private string Status = string.Empty; //To capture file download name in FileDownload event
        //Images for statusbar, ....
        private Image ImgLock;
        private Image ImgUnLock;
        private Image ImgAniProgress;
        //Forms
        private readonly frmPopup m_frmPopup = new frmPopup();
        private readonly frmFind m_frmFind = new frmFind();
        private readonly frmCacheCookie m_frmCacheCookie = new frmCacheCookie();
        private readonly frmDocInfo m_frmDocInfo = new frmDocInfo();

        private readonly Microsoft.Win32.MouseHook hook = new Microsoft.Win32.MouseHook();
        private IHTMLElement keyEntryElement;
        private bool shiftKeyDown;
        private DateTime lastKeyTime = DateTime.MinValue;
        
        private IHTMLElement lastelement;
        private string originalColor = "white";
        private IHTMLElement textActiveElement;
        private Templates templatesAvailable;
        private readonly Timer scriptTimer = new Timer();
        private int currentIndex;

        private ResourceManager resmgr = new ResourceManager("frmMain", Assembly.GetExecutingAssembly());

        //Use for Element tree
        private TreeNode m_RootNode;
        private TreeNode m_ButtonRootNode;
        private TreeNode m_CheckBoxRootNode;
        private TreeNode m_FrameRootNode;
        private TreeNode m_HtmlDialogRootNode;
        private TreeNode m_ImageRootNode;
        private TreeNode m_LabelRootNode;
        private TreeNode m_LinkRootNode;
        private TreeNode m_RadioButtonRootNode;
        private TreeNode m_SelectListRootNode;
        private TreeNode m_TableCellRootNode;
        private TreeNode m_TableRowRootNode;
        private TreeNode m_TableRootNode;
        private TreeNode m_TextFieldRootNode;
        private ArrayList arrSelectItems = new ArrayList();

        private ucBaseEditor ucEditor;
        private ActionBase actionEditing;
        #endregion

        #region Form Events
        public frmMain()
        {
            InitializeComponent();
        }
        
        static void ApplicationThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            var frm = new frmException
            {
                lblError = { Text = e.Exception.Message },
                rtbStack = { Text = e.Exception.StackTrace }
            };

            if (frm.ShowDialog() == DialogResult.OK)
            {
                EmailHelper.SendMail("", frm.txtComments.Text, e.Exception, frm.chkCopy.Checked, false);
            }
        }
        /// <summary>
        /// 初始化加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Application.ThreadException += ApplicationThreadException;

            SetGridHeader();
            
            WatiN.Core.Settings.MakeNewIeInstanceVisible = true;
            WatiN.Core.Settings.AutoStartDialogWatcher = false;
            
            watinIE =BrowserWindow.Instance(cEXWB1);
            
            wsManager = new ScriptManager(watinIE);
            

            wsManager.OnGridAdd += AddGridRowItem;
            wsManager.OnGridInsert += InsertGridRowItem;

            templatesAvailable = new Templates(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Templates"));
            
            wsManager.Settings = new AppSettings(Application.StartupPath + @"\settings.xml");
            Runner.Settings = wsManager.Settings;

            Runner.OnRunStarted += delegate(ActionBase action) {};
            Runner.OnRunCompleted += delegate(bool isAbort){  this.RunFinished(isAbort);};
            Runner.OnActionStatus += delegate(ActionBase action) { this.StatusChanged(action); };
            Runner.OnActionCounterIncremented += delegate(int counter) { this.SelectGridRow(counter); };
            Runner.OnBreakpoint += delegate(ActionBase action){ this.BreakpointReached(action); };
            Runner.OnSetCellBreakpoint += delegate(int rowIndex, BreakpointIndicators breakpointType) { this.SetCellBreakpoint(rowIndex, breakpointType); };
            Runner.OnRunJavascript += delegate() { this.LoadWatiNScript(); };
            
            Runner.OnRunResult += delegate(ActionBase action, bool result, string message)
            {
                FormHelper.FrmLog.AppendToLog("[" + action.GetType() + "][" + action.Description + "]出错了：" + message);
            };
            Runner.OnUpdateResult += delegate(bool result)
                {
                    if (result)
                    {
                        this.HandleAlert = DialogResult.OK;
                        this.HandleConfirm = DialogResult.OK;
                    }
                    else
                    {
                        this.HandleAlert = DialogResult.Cancel;
                        this.HandleConfirm = DialogResult.Cancel;
                    }
                };
            Runner.OnPopupResult += delegate(DialogResult result, string url, string title)
                {
                    if (result == DialogResult.OK)
                    {
                        m_frmPopup.Show();
                        m_frmPopup.SetURL(url);
                    }
                    else
                    {
                        m_frmPopup.Hide();
                    }
                };
           
            m_frmPopup.OnPopupClose += delegate(DialogResult result, string url, string title)
            {
                wsManager.AddClosePopup(watinIE);  //=======关闭弹出窗口====
            };

            this.helpProvider1.HelpNamespace = Application.StartupPath + @"\WatiNTestRecorder.chm";
           
            hook.Install();
            hook.MouseUp += HookMouseUp;

            SetupImages();
            this.tsbLoad.Image = this.imageList1.Images[0];

            cEXWB1.RegisterAsBrowser = true;

            m_tsBtnFirstTab = new ToolStripButton(cEXWB1.Name, ImgUnLock)
            {
                Name = cEXWB1.Name,
                Text = Blank,
                ToolTipText = AboutBlank,
                Checked = true
            };
            m_tsBtnFirstTab.MouseUp += TsWBTabsToolStripButtonCtxMenuHandler;
            tsWBTabs.Items.Add(m_tsBtnFirstTab);

            //Take note of current WB and first toolstripbutton index
            _CurWB = cEXWB1;
            IdxCurTab = tsWBTabs.Items.Count - 1;
            //Add menu
            var menu = new ToolStripMenuItem(Blank, ImgUnLock) { Name = cEXWB1.Name, Checked = true };

            m_frmFind.Icon = FormHelper.BitmapToIcon(30);
            m_frmFind.FindInPageEvent += FrmFindFindInPageEvent;

            //Start watching favorites folder
            fswFavorites.Path = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);

            //Set Run Invisible
            pnlRun.Visible = false;
            pnlRun.Dock = DockStyle.Fill;
            pnlRun.BringToFront();

            //Load favorites
            LoadFavoriteMenuItems();

            List<Template> templateList = templatesAvailable.GetList();
            foreach (Template template in templateList)
            {
                ToolStripItem item = tsbCompile.DropDownItems.Add(template.Name);
                item.Tag = template;
                item.Click += CompileCodeClick;
            }

            //NavToUrl("http://watintestrecord.sourceforge.net/version.php?version=" + Assembly.GetExecutingAssembly().GetName().Version);

            if (wsManager.Settings.SetMaxSize == true)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        void frmMain_Shown(object sender, System.EventArgs e)
        {
            gridSource[0, 2].Column.Width = splitContainer1.Panel1.Width - 95;
        }

        void frmMain_Resize(object sender, System.EventArgs e)
        {
            gridSource[0, 2].Column.Width = splitContainer1.Panel1.Width - 95;
        }

        void CompileCodeClick(object sender, EventArgs e)
        {
            var template = ((ToolStripItem)sender).Tag as Template;
            string errors = wsManager.CompileScript(template, false);
            if (!string.IsNullOrEmpty(errors))
            {
                MessageBox.Show(errors, "Compilation Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Compilation successful", "Compilation successful", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
        }

        void HookMouseUp(object sender, Microsoft.Win32.MouseHookEventArgs e)
        {
            if (e.Control != null && e.Control.Name == "cEXWB1" && e.Button == MouseButtons.Right)
            {
                IHTMLElement pelem = cEXWB1.GetActiveElement();
                if (pelem != null && pelem.tagName.ToLower(CultureInfo.InvariantCulture) == "select")
                {
                    //ShowPropertyWindow(pelem, new Point(e.X, e.Y));
                }
                return;
            }

            if (e.Button == MouseButtons.Right) return;  //右键直接返回


            IHTMLElement activeElement;
            try
            {
                activeElement = cEXWB1.GetActiveElement();
            }
            catch (Exception)
            {
                return;
            }

            if (e.Control != null && e.Control.Name == "cEXWB1" && e.Control.Parent != null && e.Control.Parent.Name == "frmMain" && activeElement != null)
            {
                // Freddy Guime
                // We are doing this differently now.
                // The issue being that clicks capture will not relate to the correct
                // activeelement if a "change" on the activeelement has been made.

                // 1) Figure out if there was an "active" element before.
                // 2) Make Click record the "TextActiveElement" 

                if (activeElement != textActiveElement) WriteKeys();
                textActiveElement = activeElement;

                if (textActiveElement.tagName.ToLower(CultureInfo.InvariantCulture) == "body")
                {
                    Point clientPoint = cEXWB1.PointToClient(new Point(e.X, e.Y));
                    textActiveElement = cEXWB1.ElementFromPoint(true, clientPoint.X, clientPoint.Y);
                }
                else if (textActiveElement.tagName.ToLower(System.Globalization.CultureInfo.InvariantCulture) == "object")
                {
                    Point clientPoint = cEXWB1.PointToClient(new Point(e.X, e.Y));
                    //wscript.AddObjectClick(Watinie, textActiveElement, clientPoint);
                    return;
                }
                else
                {
                    Point clientPoint = cEXWB1.PointToClient(new Point(e.X, e.Y));
                    IHTMLElement eItem = cEXWB1.ElementFromPoint(true, clientPoint.X, clientPoint.Y);
                    if (eItem != textActiveElement
                        && eItem.GetType() != typeof(mshtml.HTMLFrameElementClass)
                        && eItem.GetType() != typeof(mshtml.HTMLIFrameClass))
                        textActiveElement = eItem;
                }
                wsManager.AddClick(watinIE, textActiveElement,highlightingElements);
            }
            else if (e.Control != null && e.Control.Name == "cEXWB1" && e.Control.Parent != null && e.Control.Parent.Name == "frmPopup")
            {
                //if (m_frmPopup.GetActiveElement() != null)
                //{
                //    wsManager.AddClick(((frmPopup)e.Control.Parent).Watinie, m_frmPopup.GetActiveElement());
                //}
            }
        }

        /// <summary>
        /// This function is called by the timer to figure out what has been "added"
        /// in text. Try to change this to realtime on certain keys.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerActiveElement_Tick(object sender, EventArgs e)
        {
            if (wsManager == null || watinIE == null ) return; //尚未初始化，直接返回

            if (lastKeyTime < DateTime.Now.AddMilliseconds(-1 * wsManager.Settings.TypingTime) && wsManager.HasKeys()==true)
            {
                if (textActiveElement != null)
                {
                    wsManager.AddTyping(watinIE, textActiveElement);
                }
                else if (keyEntryElement != null)
                {
                    wsManager.AddTyping(watinIE, keyEntryElement);
                }
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (wsManager.ActiveList != null &&  Runner.IsAlive())
            {
                if (scriptTimer != null)
                {
                    scriptTimer.Stop();
                }
                Runner.AbortAndJoin(10000);
            }

            if (wsManager.ActiveList != null && wsManager.UnsavedScript && wsManager.Settings.WarnWhenUnsaved)
            {
                if (MessageBox.Show(
                    Properties.Resources.UnsavedScript, 
                    Properties.Resources.UnsavedScript_title,
                    MessageBoxButtons.YesNo
                    ) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void CloneActiveElementForKeys()
        {
            if (textActiveElement == null) return;
            var domnode = (mshtml.IHTMLDOMNode)textActiveElement;
            keyEntryElement = (IHTMLElement)domnode.cloneNode(true);
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            hook.Dispose();
            hook.Uninstall();
            
            _CurWB.Dispose();
            _CurWB= null;

            if (ImgLock != null)
            {
                ImgLock.Dispose();
                ImgLock = null;
            }
            if (ImgUnLock != null)
            {
                ImgUnLock.Dispose();
                ImgUnLock = null;
            }
            if (ImgAniProgress != null)
            {
                ImgAniProgress.Dispose();
                ImgAniProgress = null;
            }
            /*hao:销毁资源*/
            System.Environment.Exit(System.Environment.ExitCode);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                return;

        }

        #endregion

        #region Local methods
        private csExWB.cEXWB FindBrowser(string name)
        {
            foreach (Control ctl in Controls)
            {
                if (ctl.Name == name)
                {
                    if (ctl is csExWB.cEXWB)
                        return (csExWB.cEXWB)ctl;
                }
            }

            return null;
        }
        private ToolStripButton FindTab(string name)
        {
            try
            {
                foreach (ToolStripItem item in tsWBTabs.Items)
                {
                    if (item.Name == name)
                    {
                        if (item is ToolStripButton)
                            return (ToolStripButton)item;
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
        private void AddNewBrowser(string tabText, string tabTooltip, string url, bool bringToFront)
        {
            //Copy flags
            var iDochostUIFlag = (int)(DOCHOSTUIFLAG.NO3DBORDER |DOCHOSTUIFLAG.FLAT_SCROLLBAR | DOCHOSTUIFLAG.THEME);
            var iDocDlCltFlag =  (int)(DOCDOWNLOADCTLFLAG.DLIMAGES |DOCDOWNLOADCTLFLAG.BGSOUNDS | DOCDOWNLOADCTLFLAG.VIDEOS);

            if (_CurWB != null)
            {
                iDochostUIFlag = _CurWB.WBDOCHOSTUIFLAG;
                iDocDlCltFlag = _CurWB.WBDOCDOWNLOADCTLFLAG;
            }

            csExWB.cEXWB pWB;

            int i = IdxCountWB + 1;
            string sname = "cEXWB" + i;

            try
            {
                var btn = new ToolStripButton(sname, ImgUnLock) { Name = sname };
                if (tabText.Length > 0)  btn.Text = tabText;
                if (tabTooltip.Length > 0)  btn.ToolTipText = tabTooltip;

                btn.AutoToolTip = true;
                btn.MouseUp += TsWBTabsToolStripButtonCtxMenuHandler;

                tsWBTabs.Items.Add(btn);

                //Create and setup browser
                pWB = new csExWB.cEXWB
                {
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left| AnchorStyles.Right,
                    Name = sname,
                    Location = cEXWB1.Location,
                    Size = cEXWB1.Size,
                    RegisterAsBrowser = true,
                    WBDOCDOWNLOADCTLFLAG = iDocDlCltFlag,
                    WBDOCHOSTUIFLAG = iDochostUIFlag
                };

                //Add events, using the same eventhandlers for all browsers
                pWB.TitleChange += cEXWB1_TitleChange;
                pWB.StatusTextChange += cEXWB1_StatusTextChange;
                pWB.CommandStateChange += cEXWB1_CommandStateChange;
                pWB.WBKeyDown += cEXWB1_WBKeyDown;
                pWB.WBEvaluteNewWindow += cEXWB1_WBEvaluteNewWindow;
                pWB.ProgressChange += cEXWB1_ProgressChange;

                //pWB.HTMLEvent += new csExWB.HTMLEventHandler(cEXWB1_HTMLEvent);
                //pWB.DownloadBegin += cEXWB1_DownloadBegin;
                //pWB.DownloadComplete += cEXWB1_DownloadComplete;

                pWB.ScriptError += cEXWB1_ScriptError;
                pWB.StatusTextChange += cEXWB1_StatusTextChange;
                pWB.WBDragDrop += cEXWB1_WBDragDrop;
                pWB.SetSecureLockIcon += cEXWB1_SetSecureLockIcon;
                pWB.WBSecurityProblem += cEXWB1_WBSecurityProblem;
                pWB.NewWindow2 += cEXWB1_NewWindow2;
                pWB.DocumentComplete += cEXWB1_DocumentComplete;
                pWB.NewWindow3 += cEXWB1_NewWindow3;
                pWB.WBKeyUp += cEXWB1_WBKeyUp;
                pWB.WindowClosing += cEXWB1_WindowClosing;
                pWB.WBDocHostShowUIShowMessage += cEXWB1_WBDocHostShowUIShowMessage;
                pWB.FileDownload += cEXWB1_FileDownload;
                pWB.WBAuthenticate += cEXWB1_WBAuthenticate;

                //Add to controls collection
                Controls.Add(pWB);

                //var menu = new ToolStripMenuItem(btn.Text, btn.Image) {Name = sname};

                if (bringToFront)
                {
                    //Uncheck last tab
                    ((ToolStripButton)tsWBTabs.Items[IdxCurTab]).Checked = false;
                    btn.Checked = true;

                    //Adjust current browser pointer
                    _CurWB = pWB;
                    //Adjust current tab index
                    IdxCurTab = tsWBTabs.Items.Count - 1;
                    //Reset and hide progressbar
                    tsProgress.Value = 0;
                    tsProgress.Maximum = 0;
                    tsProgress.Visible = false;
                    //Bring to front
                    pWB.BringToFront();
                }
                //Increase count
                IdxCountWB++;
                tsBtnOpenWBs.Text = IdxCountWB + " open tab(s)";

                if (url.Length > 0) pWB.Navigate(url);
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("AddNewBrowser error" + ee.Message);
                return;
            }

            return;
        }
        /// <summary>
        /// Removes an inactive browser without switching to another
        /// </summary>
        /// <param name="name"></param>
        private void RemoveBrowser2(string name)
        {
            try
            {
                //Do not remove the first browser          
                if ((IdxCountWB == 1) || (name == m_tsBtnFirstTab.Name))
                    return;

                csExWB.cEXWB pWB = FindBrowser(name);
                Controls.Remove(pWB);
                pWB.Dispose();

                ToolStripButton btn = FindTab(name);
                tsWBTabs.Items.Remove(btn);
                btn.Dispose();

                IdxCountWB--;
                tsBtnOpenWBs.Text = IdxCountWB + " open tab(s)";
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("RemoveBrowser2\r\n" + ee);
            }
            return;
        }

        /// <summary>
        /// Removes the current browser and switches to the one before it
        /// if one is available, else the first one is selected
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private void RemoveBrowser(string name)
        {
            //Do not remove the first browser          
            if ((IdxCountWB == 1) || (name == m_tsBtnFirstTab.Name))
                return;

            tsProgress.Value = 0;
            tsProgress.Maximum = 0;
            tsProgress.Visible = false;

            ToolStripButton btn = FindTab(name);
            ToolStripButton nexttab = null;
            try
            {
                //find the first available btn before this one and switch
                foreach (ToolStripItem item in tsWBTabs.Items)
                {
                    if (item.Name == btn.Name)
                    {
                        break;
                    }
                    if (item is ToolStripButton)
                        nexttab = (ToolStripButton)item;
                }
            }
            catch (Exception eRemoveBrowser)
            {
                FormHelper.FrmLog.AppendToLog("RemoveBrowser\r\n" + eRemoveBrowser);
            }

            try
            {
                tsWBTabs.Items.Remove(btn);
                btn.Dispose();
            }
            catch (Exception eRemoveBrowser1)
            {
                FormHelper.FrmLog.AppendToLog("RemoveBrowser1\r\n" + eRemoveBrowser1);
            }

            try
            {
                csExWB.cEXWB pWB = FindBrowser(name);
                Controls.Remove(pWB);
                pWB.Dispose();
            }
            catch (Exception eRemoveBrowser2)
            {
                FormHelper.FrmLog.AppendToLog("RemoveBrowser2\r\n" + eRemoveBrowser2);
            }

            try
            {
                if (nexttab == null)
                {
                    _CurWB = cEXWB1;
                    IdxCurTab = tsWBTabs.Items.IndexOf(m_tsBtnFirstTab);
                    //IdxCurMenu = 0;
                    nexttab = m_tsBtnFirstTab;
                }
                else
                {
                    _CurWB = FindBrowser(nexttab.Name);
                    IdxCurTab = tsWBTabs.Items.IndexOf(nexttab);
                }

                Text = _CurWB.GetTitle(true);
                if (Text.Length == 0)
                    Text = Blank;
                comboURL.Text = nexttab.ToolTipText;
                nexttab.Checked = true;
                _CurWB.BringToFront();

                _CurWB.SetFocus();

            }
            catch (Exception eRemoveBrowser4)
            {
                FormHelper.FrmLog.AppendToLog("RemoveBrowser4\r\n" + eRemoveBrowser4);
            }

            IdxCountWB--;
            tsBtnOpenWBs.Text = IdxCountWB + " open tab(s)";

            return;
        }

        private void SwitchTabs(string name, ToolStripButton btn)
        {
            try
            {
                csExWB.cEXWB pWB = FindBrowser(name);
                if (pWB == null)
                    return;

                //Uncheck last one
                if (IdxCountWB > 1)
                    ((ToolStripButton)tsWBTabs.Items[IdxCurTab]).Checked = false;
                IdxCurTab = tsWBTabs.Items.IndexOf(btn);

                _CurWB = pWB;
                tsBtnBack.Enabled = _CurWB.CanGoBack;
                tsBtnForward.Enabled = _CurWB.CanGoForward;
                _CurWB.BringToFront();
                _CurWB.SetFocus();
                Text = _CurWB.GetTitle(true);
                if (Text.Length == 0) Text = Blank;
                if (btn != null)
                {
                    btn.Checked = true;
                    btn.Text = Text;
                    if (btn.Text.Length > MaxTextLen)
                        btn.Text = btn.Text.Substring(0, MaxTextLen) + "...";
                    btn.ToolTipText = HttpUtility.UrlDecode(_CurWB.LocationUrl);
                    comboURL.Text = btn.ToolTipText;
                }

                //Reset and hide progressbar
                //If page is in the process of loading then the progressbar
                //will be adjusted
                tsProgress.Value = 0;
                tsProgress.Maximum = 0;
                tsProgress.Visible = false;

                //update SecureLockIcon state
                UpdateSecureLockIcon(_CurWB.SecureLockIcon);
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("SwitchingTabs\r\n" + ee);
            }

        }
        /// <summary>
        /// 安全认证
        /// </summary>
        /// <param name="slic"></param>
        private void UpdateSecureLockIcon(SecureLockIconConstants slic)
        {
            if (slic == SecureLockIconConstants.secureLockIconUnsecure)
            {
                tsSecurity.Image = ImgUnLock;
                tsSecurity.Text = "Not Secure";
            }
            else if (slic == SecureLockIconConstants.secureLockIcon128Bit)
            {
                tsSecurity.Image = ImgLock;
                tsSecurity.Text = "128 Bit";
            }
            else if (slic == SecureLockIconConstants.secureLockIcon40Bit)
            {
                tsSecurity.Image = ImgLock;
                tsSecurity.Text = "40 Bit";
            }
            else if (slic == SecureLockIconConstants.secureLockIcon56Bit)
            {
                tsSecurity.Image = ImgLock;
                tsSecurity.Text = "56 Bit";
            }
            else if (slic == SecureLockIconConstants.secureLockIconFortezza)
            {
                tsSecurity.Image = ImgLock;
                tsSecurity.Text = "Fortezza";
            }
            else if (slic == SecureLockIconConstants.secureLockIconMixed)
            {
                tsSecurity.Image = ImgUnLock;
                tsSecurity.Text = "Mixed";
            }
            else if (slic == SecureLockIconConstants.secureLockIconUnknownBits)
            {
                tsSecurity.Image = ImgUnLock;
                tsSecurity.Text = "UnknownBits";
            }
        }

        /// <summary>
        /// Loads all images from a image strip into a
        /// static imagelist which in turn can 
        /// be used by any form or control 
        /// capable of using images
        /// </summary>
        private void SetupImages()
        {
            try
            {
                Stream file1 = GetType().Assembly.GetManifestResourceStream("TestRecorder.Resources.progress_spinner.gif");
                if (file1 != null) ImgAniProgress = Image.FromStream(file1);

                Stream file = GetType().Assembly.GetManifestResourceStream("TestRecorder.Resources.IeToolbar.bmp");
                if (file != null)
                {
                    Image img = Image.FromStream(file);
                    FormHelper.ImgListMain.TransparentColor = Color.FromArgb(192, 192, 192);
                    FormHelper.ImgListMain.Images.AddStrip(img);
                }

                tsBtnBack.Image = FormHelper.ImgListMain.Images[0];
                tsBtnForward.Image = FormHelper.ImgListMain.Images[1];
                tsBtnStop.Image = FormHelper.ImgListMain.Images[2];
                tsBtnRefresh.Image = FormHelper.ImgListMain.Images[4];
                tsSplitBtnSearch.Image = FormHelper.ImgListMain.Images[30];
                tsBtnGo.Image = FormHelper.ImgListMain.Images[10];
                tsChkBtnGo.Image = FormHelper.ImgListMain.Images[18];

                tsBtnOpenWBs.Image = FormHelper.ImgListMain.Images[12];
                tsBtnAddWB.Image = FormHelper.ImgListMain.Images[16];
                tsChkBtnAddWB.Image = FormHelper.ImgListMain.Images[18];
                tsBtnRemoveWB.Image = FormHelper.ImgListMain.Images[39];
                tsBtnRemoveAllWBs.Image = FormHelper.ImgListMain.Images[40];

                ImgLock = FormHelper.ImgListMain.Images[13];
                ImgUnLock = FormHelper.ImgListMain.Images[32]; //normall ie

                tsLinksLblText.Image = FormHelper.ImgListMain.Images[20];

                tsFileMnuNew.Image = FormHelper.ImgListMain.Images[19];
                tsFileMnuOpen.Image = FormHelper.ImgListMain.Images[43];
                tsFileMnuSave.Image = FormHelper.ImgListMain.Images[21];
                tsFileMnuSaveDocument.Image = FormHelper.ImgListMain.Images[44];
                tsFileMnuSaveDocumentImage.Image = FormHelper.ImgListMain.Images[45];
                tsEditMnuCut.Image = FormHelper.ImgListMain.Images[23];
                tsEditMnuCopy.Image = FormHelper.ImgListMain.Images[24];
                tsEditMnuPaste.Image = FormHelper.ImgListMain.Images[25];
                tsEditMnuSelectAll.Image = FormHelper.ImgListMain.Images[28];
                tsEditMnuFindInPage.Image = FormHelper.ImgListMain.Images[30];
                tsFileMnuPrintPreview.Image = FormHelper.ImgListMain.Images[7];
                tsFileMnuPrint.Image = FormHelper.ImgListMain.Images[8];
                tsFileMnuExit.Image = FormHelper.ImgListMain.Images[37];

                tsHelpMnuHelpAbout.Image = FormHelper.ImgListMain.Images[33];
                tsHelpMnuHelpContents.Image = FormHelper.ImgListMain.Images[9];

                Icon = FormHelper.BitmapToIcon(41);
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("\r\nError=" + ee);
            }
        }

        private void NavToUrl(string sUrl)
        {
            try
            {
                _CurWB.Navigate(sUrl);
                wsManager.AddNavigation(watinIE, sUrl, "", "");
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("NavToUrl\r\n" + ee);
            }
        }

        #endregion

        #region Event handlers
        /// <summary>
        /// Handles click event of the drop down menu items of the
        /// toolstrip utton responsible to display number of open browsers
        /// and to offer a quick menu to select a browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsBtnOpenWBs_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                SwitchTabs(e.ClickedItem.Name, FindTab(e.ClickedItem.Name));
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("tsBtnOpenWBs_DropDownItemClicked\r\n" + ee);
            }
        }

        /// <summary>
        /// Handles toolstripbutton (tabs) click events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsWBTabs_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                if (e.ClickedItem.Name == tsBtnOpenWBs.Name)
                    return;

                var btn = (ToolStripButton)e.ClickedItem;
                if (e.ClickedItem.Name == tsBtnAddWB.Name)
                {
                    if (tsChkBtnAddWB.Checked)
                    {
                        AddNewBrowser(Blank, AboutBlank, "", false);
                    }
                    else
                        AddNewBrowser(Blank, AboutBlank, "", true);
                }
                else if (e.ClickedItem.Name == tsBtnRemoveWB.Name)
                {
                    RemoveBrowser((tsWBTabs.Items[IdxCurTab]).Name);
                }
                else if (e.ClickedItem.Name == tsBtnRemoveAllWBs.Name)
                {
                    tsMnuCloseAllWBs_Click(this, EventArgs.Empty);
                }
                else
                    SwitchTabs(e.ClickedItem.Name, btn);
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("tsWBTabs_ItemClicked\r\n" + ee);
            }
        }

        /// <summary>
        /// Handles close menu click event to remove a browser
        /// May not be the current browser in front
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsMnuCloseWB_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_tsBtnctxMnu == null)
                    return;
                //Is this the current one
                if (m_tsBtnctxMnu.Name == _CurWB.Name)
                {
                    RemoveBrowser(m_tsBtnctxMnu.Name);
                }
                else
                {
                    RemoveBrowser2(m_tsBtnctxMnu.Name);
                }
                m_tsBtnctxMnu = null;
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("tsMnuCloseWB_Click\r\n" + ee);
            }
        }

        /// <summary>
        /// Close all browsers except the first one from design time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsMnuCloseAllWBs_Click(object sender, EventArgs e)
        {
            if (IdxCountWB == 1)
                return;
            try
            {

                _CurWB = cEXWB1;
                _CurWB.BringToFront();
                _CurWB.SetFocus();

                m_tsBtnFirstTab.Checked = true;
                IdxCurTab = tsWBTabs.Items.IndexOf(m_tsBtnFirstTab);

                string text = _CurWB.GetTitle(true);
                if (text.Length == 0)
                    text = Blank;
                new ToolStripMenuItem(text, tsBtnOpenWBs.Image) { Name = m_tsBtnFirstTab.Name, Checked = true };

                tsBtnOpenWBs.Text = IdxCountWB + " open tab(s)";
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("tsMnuCloasAllWBs_Click\r\n" + ee);
            }
        }

        /// <summary>
        /// Update enable state of Edit menu items before displaying them
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsMnuEdit_DropDownOpening(object sender, EventArgs e)
        {
            try
            {
                tsEditMnuSelectAll.Enabled = _CurWB.IsCommandEnabled("SelectAll");
                tsEditMnuCopy.Enabled = _CurWB.IsCommandEnabled("Copy");
                tsEditMnuCut.Enabled = _CurWB.IsCommandEnabled("Cut");
                tsEditMnuPaste.Enabled = _CurWB.IsCommandEnabled("Paste");
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("tsMnuEdit_DropDownOpening\r\n" + ee);
            }
        }

        /// <summary>
        /// Call back to intercept find requests from frmFind
        /// </summary>
        /// <param name="findPhrase"></param>
        /// <param name="matchWholeWord"></param>
        /// <param name="matchCase"></param>
        /// <param name="downward"></param>
        /// <param name="highlightAll"></param>
        /// <param name="sColor"></param>
        void FrmFindFindInPageEvent(string findPhrase, bool matchWholeWord, bool matchCase, bool downward, bool highlightAll, string sColor)
        {
            try
            {
                if (highlightAll)
                {
                    int found = _CurWB.FindAndHightAllInPage(findPhrase, matchWholeWord, matchCase, sColor, "black");
                    MessageBox.Show(this, string.Format(Properties.Resources.FoundMatches, found), Properties.Resources.FindInPage, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (_CurWB.FindInPage(findPhrase, downward, matchWholeWord, matchCase, true) == false)
                        MessageBox.Show(this, string.Format(Properties.Resources.NoMoreMatchesFound, findPhrase), Properties.Resources.FindInPage, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("frmMain_m_frmFind_FindInPageEvent\r\n" + ee);
            }
        }

        #endregion

        #region TextSize

        /// <summary>
        /// Sets browser text size based on given parameter 0-4
        /// Adjust the chack state of textsize menu items
        /// </summary>
        /// <param name="iLevel">Text size level 0-4</param>
        private void SetZoomLevel(int iLevel)
        {
            tsViewMnuTextSizeLargest.Checked = false;
            tsViewMnuTextSizeLarger.Checked = false;
            tsViewMnuTextSizeMedium.Checked = false;
            tsViewMnuTextSizeSmaller.Checked = false;
            tsViewMnuTextSizeSmallest.Checked = false;

            switch (iLevel)
            {
                case 0:
                    tsViewMnuTextSizeLargest.Checked = true;
                    if (_CurWB != null)
                        _CurWB.TextSize = TextSizeWB.Largest;
                    break;
                case 1:
                    tsViewMnuTextSizeLarger.Checked = true;
                    if (_CurWB != null)
                        _CurWB.TextSize = TextSizeWB.Larger;
                    break;
                case 2:
                    tsViewMnuTextSizeMedium.Checked = true;
                    if (_CurWB != null)
                        _CurWB.TextSize = TextSizeWB.Medium;
                    break;
                case 3:
                    tsViewMnuTextSizeSmaller.Checked = true;
                    if (_CurWB != null)
                        _CurWB.TextSize = TextSizeWB.Smaller;
                    break;
                case 4:
                    tsViewMnuTextSizeSmallest.Checked = true;
                    if (_CurWB != null)
                        _CurWB.TextSize = TextSizeWB.Smallest;
                    break;
            }
        }

        /// <summary>
        /// Hanldes textsize menu item clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsMnuTextSizeClickHandler(object sender, EventArgs e)
        {
            if (sender == tsViewMnuTextSizeLargest)
                SetZoomLevel(0);
            else if (sender == tsViewMnuTextSizeLarger)
                SetZoomLevel(1);
            else if (sender == tsViewMnuTextSizeMedium)
                SetZoomLevel(2);
            else if (sender == tsViewMnuTextSizeSmaller)
                SetZoomLevel(3);
            else if (sender == tsViewMnuTextSizeSmallest)
                SetZoomLevel(4);
        }

        /// <summary>
        /// Updates the check state of the text size menu items before displaying them
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsMnuTextSize_DropDownOpening(object sender, EventArgs e)
        {
            tsViewMnuTextSizeLargest.Checked = false;
            tsViewMnuTextSizeLarger.Checked = false;
            tsViewMnuTextSizeMedium.Checked = false;
            tsViewMnuTextSizeSmaller.Checked = false;
            tsViewMnuTextSizeSmallest.Checked = false;
            if (cEXWB1.TextSize == TextSizeWB.Largest)
                tsViewMnuTextSizeLargest.Checked = true;
            else if (cEXWB1.TextSize == TextSizeWB.Larger)
                tsViewMnuTextSizeLarger.Checked = true;
            else if (cEXWB1.TextSize == TextSizeWB.Medium)
                tsViewMnuTextSizeMedium.Checked = true;
            else if (cEXWB1.TextSize == TextSizeWB.Smaller)
                tsViewMnuTextSizeSmaller.Checked = true;
            else if (cEXWB1.TextSize == TextSizeWB.Smallest)
                tsViewMnuTextSizeSmallest.Checked = true;
        }

        #endregion

        private void watiNTestRecorderHomePageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://watintestrecord.sourceforge.net/");
        }

        private void tsbTimer_Click(object sender, EventArgs e)
        {
            tsbTimer.Checked = !tsbTimer.Checked;
            wsManager.WaitTimerActive = tsbTimer.Checked;
            //wsManager.ClearTimer();
        }

        private void StatusChanged(ActionBase action)
        {
            if (InvokeRequired)
            {
                Invoke(new ActionStatusEvent(StatusChanged), action);
                return;
            }
        }

    
        private void perlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string code = wsManager.ActiveList.GetCode(new PerlCodeFormatter { FileDestination = false }, GetBrowserTypeSelected());
            Clipboard.SetText(code);
        }

        private void pythonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string code = wsManager.ActiveList.GetCode(new PythonCodeFormatter { FileDestination = false }, GetBrowserTypeSelected());
            Clipboard.SetText(code);
        }

        private void pHPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string code = wsManager.ActiveList.GetCode(new PhpCodeFormatter { FileDestination = false }, GetBrowserTypeSelected());
            Clipboard.SetText(code);
        }

        private void rubyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string code = wsManager.ActiveList.GetCode(new RubyCodeFormatter { FileDestination = false }, GetBrowserTypeSelected());
            Clipboard.SetText(code);
        }
      
        private void javascriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddActionByName(((ToolStripItem)sender).Text);
        }


    }
}