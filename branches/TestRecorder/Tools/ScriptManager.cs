using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using IfacesEnumsStructsClasses;
using Microsoft.VisualBasic;
using TestRecorder.Core;
using TestRecorder.Core.Actions;

namespace TestRecorder.Tools
{
    /// <summary>
    /// WaitN 脚本，只用来管理脚本，不运行脚本
    /// </summary>
    public sealed class ScriptManager
    {
        public GridAddEvent OnGridAdd;
        public GridInsertEvent OnGridInsert;


        public string CurrentName = "";
        public bool WaitTimerActive;
        
        public bool Recording;
        public bool UnsavedScript;

        public BrowserTypes BrowserTypeTarget;
        public int InsertPosition=-1;  //-1时不能插入

        //public bool ShouldInsertActions;减少一个控制变量
        
        public AppSettings Settings;
       
        public ActionList ActiveList = new ActionList();
        public List<ActionList> ActionListSet = new List<ActionList>();
        private LoadContext CurrentLoadContext = LoadContext.Instance();

        //private DateTime WaitTimer;
        private IHTMLElement FileActiveElement;
        private BrowserWindow FileElementBrowser;
        private System.Timers.Timer TimerFileDialog;
        private bool BlnFileDialogFound;

        private StringBuilder sbKeys = new StringBuilder();
        private int lastTypeHashCode = 0;
        private string lastTypeValue = "";

        /// <summary>
        /// 是否触发了键盘输入事件
        /// </summary>
        /// <returns></returns>
        public bool HasKeys()
        {
            return sbKeys.Length > 0;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ScriptManager(BrowserWindow theie)
        {
            Settings = new AppSettings("");
            ActionListSet.Add(ActiveList);
        }
        private void AddAction(ActionBase action)
        {
            if (!Recording) return;

            if (InsertPosition > -1) //插入
            {
                InsertAction(InsertPosition, action);
                InsertPosition++;
            }
            else
            {
                ActiveList.Add(action);
                if (this.OnGridAdd != null) this.OnGridAdd(action);
            }

            this.UnsavedScript = true;
        }

        private void InsertAction(int index, ActionBase action)
        {
            if (!Recording) return;

            if (index < 0) index = 0;

            ActiveList.Insert(index, action);
            if (this.OnGridInsert != null) this.OnGridInsert(index + 1, action);
        }
        /// <summary>
        /// 查找测试用例
        /// </summary>
        /// <param name="testName"></param>
        /// <returns></returns>
        public ActionList FindTestByName(string testName)
        {
            foreach (ActionList test in ActionListSet)
            {
                if (test.TestName == testName)
                {
                    return test;
                }
            }
            return null;
        }

        public int GetTestIndex(string testName)
        {
            for (int index = 0; index < ActionListSet.Count; index++)
            {
                if (ActionListSet[index].TestName == testName)
                {
                    return index;
                }
            }
            return -1;
        }


        public void AddEvent(BrowserWindow browser, IHTMLElement activeElement, string eventName)
        {
            ActionContext context = this.GetActionContext(browser, activeElement);
            var action = new ActionFireEvent(context) { EventName = eventName };
            action.SetFindMethod(browser, activeElement);
            AddAction(action);
        }

        public void AddTestElement(BrowserWindow browser, IHTMLElement activeElement)
        {
            ActionContext context = this.GetActionContext(browser, activeElement);
            var action = new ActionTestElement(context);
            action.SetFindMethod(browser, activeElement);
            AddAction(action);
        }
        private ActionContext GetActionContext(BrowserWindow browser, IHTMLElement element)
        {
            ActionContext context = new ActionContext(browser,Settings.FindPattern);
            context.PageContext = this.CurrentLoadContext;
           
            if (browser != null
                && element != null
                && element.document != null)
            {
                mshtml.HTMLDocumentClass curDoc = element.document as mshtml.HTMLDocumentClass;
                mshtml.DispHTMLDocument curDoc2 = element.document as mshtml.DispHTMLDocument;
                if (curDoc != null && curDoc.body.outerHTML.GetHashCode() == browser.Body.OuterHtml.GetHashCode())
                {

                }
                else if (curDoc != null && curDoc.body.outerHTML.GetHashCode() == browser.Body.OuterHtml.GetHashCode())
                {

                }
                else if (browser.Frames.Count > 0)
                {
                    foreach (var frame in browser.Frames)
                    {
                        if (curDoc != null && frame.Url == curDoc.url) //===============通过地址栏或内容的哈希码判断================
                        {
                            context.FrameUrl = frame.Url;
                        }
                        else if (curDoc2 != null)
                        {
                            context.FrameUrl = frame.Url;
                        }
                        else
                        {
                            FormHelper.FrmLog.AppendToLog("未知的文档类型：" + Information.TypeName(element.document));
                        }
                    }
                }
            }
            return context;
        }
        public void AddHighlight(BrowserWindow browser, IHTMLElement activeElement)
        {
            // don't add the click if we're just highlighting elements 
            if ( Recording == false) return;

            ActionContext context = this.GetActionContext(browser, activeElement);
            var action = new ActionHighlight(context);
            action.SetFindMethod(browser, activeElement);

            if (action.GetFilterNum() < 1) return; //=======无意义的Add操作=====
            
            AddAction(action);
        }
        public void AddClick(BrowserWindow browser, IHTMLElement activeElement,bool hightLight)
        {
            // don't add the click if we're just highlighting elements 
            if (hightLight==true || Recording == false) return;

            ActionContext context = this.GetActionContext(browser, activeElement);
            var action = new ActionClick(context);
            action.SetFindMethod(browser,activeElement);

            if (action.GetFilterNum() < 1) return; 

            if (action.ElementType == ElementTypes.FileUpload)
            {
                WatchFileUploadBox(browser, activeElement);
            }
            else if (action.ElementType == ElementTypes.RadioButton)
            {
                var radio = new ActionRadio(context)
                {
                    Checked = (activeElement.outerHTML.ToLower().Contains("checked") == false)
                };
                AddAction(radio);
            }
            else if (action.ElementType == ElementTypes.CheckBox)
            {
                var checkbox = new ActionCheckbox(context)
                {
                   Checked = (activeElement.outerHTML.ToLower().Contains("checked") == false)
                };
                AddAction(checkbox);
            }
            else if (action.ElementType == ElementTypes.SelectList || action.ElementType == ElementTypes.Frame)
            {
                // do nothing with the click
            }
            else
            {
                AddAction(action);
            }
        }

        public ActionWait AddWait(BrowserWindow browser, IHTMLElement activeElement, ActionWait.WaitTypes waitType)
        {
            ActionContext context = this.GetActionContext(browser, activeElement);
            var wait = new ActionWait(context) {WaitType = waitType};
            wait.SetFindMethod(browser, activeElement);
            AddAction(wait);
            return wait;
        }

        public void AddSleep(int sleepCount)
        {
            ActionContext context = this.GetActionContext(null, null);
            var sleep = new ActionSleep(context){SleepTime = sleepCount};
            AddAction(sleep);
        }
        
        /// <summary>
        /// 文本框输入,中文输入有问题
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="activeElement"></param>
        public void AddTyping(BrowserWindow browser, IHTMLElement activeElement)
        {
            if (sbKeys.Length<=0 || activeElement == null) return; //返回

            string value = GetValueOfElement(activeElement);
            string lastValue = this.GetValueOfLast(activeElement);

            if (value.Length == 0  || value.Equals(lastValue) ) return; //返回

            ActionTypeText typeText = null;
            try
            {
                ActionContext context = this.GetActionContext(browser, activeElement);
                typeText = new ActionTypeText(context);
                typeText.SetFindMethod(browser, activeElement);
                typeText.TextToType = value;
                typeText.Overwrite = true;

                this.lastTypeValue = value;
                this.lastTypeHashCode = activeElement.GetHashCode();//=====更新最后状态=====
            }
            catch (Exception)
            {
                return;
            }
            sbKeys.Length = 0;
            AddAction(typeText);
        }
        public void AddSelectListItem(BrowserWindow browser, IHTMLElement activeElement, bool byValue)
        {
            ActionContext context = this.GetActionContext(browser, activeElement);
            
            var selectlist = new ActionSelectList(context) { ByValue = byValue };
            selectlist.SetFindMethod(browser, activeElement);
            
            var selectedElement = activeElement as IHTMLSelectElement;
            if (selectedElement == null) return;
            
            var optionElements = selectedElement.options as mshtml.HTMLSelectElementClass;
            if (optionElements == null) return;

            foreach (IHTMLOptionElement option in optionElements)
            {
                if (option.selected)
                {
                    selectlist.SelectedValue = option.value;
                    selectlist.SelectedText = option.text;
                    break;
                }
            }

            selectlist.SelectedValue = selectedElement.value;

            AddAction(selectlist);
        }

        public void AddNavigation(BrowserWindow browser, string url, string username, string password)
        {
            ////Context.ActivePage = browser.DeterminePage(),
            ActionContext context = this.GetActionContext(browser, null);
            var nav = new ActionNavigate(context)
            {
                URL = url,
                Username = username,
                Password = password
            };

            if (username != "" && ActiveList.Count > 0)
            {
                ActiveList.RemoveAt(ActiveList.Count - 1);
            }
            AddAction(nav);
        }
        
        public void AddDirectionKey(string directionKey)
        {
            ActionContext context = this.GetActionContext(null, null);
            AddAction(new ActionDirectionKey(context,directionKey));
        }

        public void AddBack(BrowserWindow browser)
        {
            ActionContext context = this.GetActionContext(browser, null);
            AddAction(new ActionBack(context));
        }

        public void AddForward(BrowserWindow browser)
        {
            ActionContext context = this.GetActionContext(browser, null);
            AddAction(new ActionForward(context));
        }

        public void AddRefresh(BrowserWindow browser)
        {
            ActionContext context = this.GetActionContext(browser, null);
            AddAction(new ActionRefresh(context));
        }

        public void AddAlertHandler(BrowserWindow browser)
        {
            ActionContext context = this.GetActionContext(browser, null);
            // put the handler before the thing triggering it
            int index = ActiveList.Count - 1;
            if (index < 0) index = 0;
            InsertAction(index, new ActionAlertHandler(context));
        }
        public void AddJavascriptHandler(BrowserWindow browser)
        {
            ActionContext context = this.GetActionContext(browser, null);
            // put the handler before the thing triggering it
            int index = ActiveList.Count - 1;
            if (index < 0) index = 0;
            InsertAction(index, new JavascriptHandler(context));
        }

        //public void AddConfirmHandler(DialogResult dialogResult)
        public void AddConfirmHandler(BrowserWindow browser,DialogResult result)
        {
            // put the handler before the thing triggering it
            int index = ActiveList.Count - 1;
            if (index < 0) index = 0;
            var confirm = new ActionConfirmHandler(this.GetActionContext(browser, null));
            confirm.Result = (result==DialogResult.OK);
            InsertAction(index, confirm);
        }

        public void AddLoginDialog(string username, string password)
        {
            // add login to last navigation object
        }

        public void AddPopup(BrowserWindow browser,string ieName, string url)
        {
            ActionContext context = this.GetActionContext(browser, null);
            var action = new ActionOpenPopup(context);
            action.BrowserTitle = ieName;
            action.BrowserURL = url;
            AddAction(action);
        }

        public void AddClosePopup(BrowserWindow window)
        {
            ActionContext context = this.GetActionContext(window, null);
            var win = new ActionCloseWindow(context);
            AddAction(win);
        }

        public void AddKeys(IHTMLElement activeElement,bool shifted, Keys keycode)
        {
            string strKey = keycode.ToString();
            
            switch (keycode)
            {
                case Keys.Space: strKey = " "; break;
                case Keys.Enter: 
                case Keys.Tab: 
                case Keys.Up: 
                case Keys.Down: 
                case Keys.Left:
                case Keys.Right: 
                case Keys.Back: 
                case Keys.Delete: strKey = ""; break;
            }
            
            if (shifted && Regex.IsMatch(strKey, @"\AD\d\z"))
            {
                strKey = Convert.ToChar(keycode).ToString();
                switch (strKey)
                {
                    case "1": strKey = "!"; break;
                    case "2": strKey = "@"; break;
                    case "3": strKey = "#"; break;
                    case "4": strKey = "$"; break;
                    case "5": strKey = "%"; break;
                    case "6": strKey = "^"; break;
                    case "7": strKey = "&"; break;
                    case "8": strKey = "*"; break;
                    case "9": strKey = "("; break;
                    case "0": strKey = ")"; break;
                }
            }
            else if (!shifted && Regex.IsMatch(strKey, @"\AD\d\z"))
            {
                strKey = Convert.ToChar(keycode).ToString();
            }
            else if (Regex.IsMatch(strKey, @"\ANumPad\d\z"))
            {
                strKey = strKey.Replace("NumPad", "");
            }
            else if (!shifted && strKey == @"\")
            {
                strKey = "\\";
            }
            else if (!shifted && Regex.IsMatch(strKey, @"\AOem\w+\z"))
            {
                switch (strKey)
                {
                    case "Oemtilde": strKey = "`"; break;
                    case "OemMinus": strKey = "-"; break;
                    case "Oemplus": strKey = "="; break;
                    case "OemOpenBrackets": strKey = "["; break;
                    case "Oem6": strKey = "]"; break;
                    case "Oem1": strKey = ";"; break;
                    case "Oem7": strKey = "'"; break;
                    case "Oemcomma": strKey = ","; break;
                    case "OemPeriod": strKey = "."; break;
                    case "OemQuestion": strKey = "/"; break;
                    case "Oem5": strKey = @"\"; break;
                }
            }
            else if (shifted && Regex.IsMatch(strKey, @"\AOem\w+\z"))
            {
                switch (strKey)
                {
                    case "Oemtilde": strKey = "~"; break;
                    case "OemMinus": strKey = "_"; break;
                    case "Oemplus": strKey = "+"; break;
                    case "OemOpenBrackets": strKey = "{"; break;
                    case "Oem6": strKey = "}"; break;
                    case "Oem1": strKey = ":"; break;
                    case "Oem7": strKey = "\\\""; break;
                    case "Oemcomma": strKey = "<"; break;
                    case "OemPeriod": strKey = ">"; break;
                    case "OemQuestion": strKey = "?"; break;
                    case "Oem5": strKey = "|"; break;
                }
            }
           

            //if (shifted)  sbKeys.Append(strKey) sbKeys.Append(strKey.ToLower());

            if (activeElement != null)
            {
                this.lastTypeHashCode = activeElement.GetHashCode();
                this.sbKeys.Append(strKey);
            }
            else
            {
                this.lastTypeHashCode = 0;
                this.sbKeys.Length = 0;
            }
        }
        /// <summary>
        /// 上次输入的值
        /// </summary>
        /// <param name="activeElement"></param>
        /// <returns></returns>
        private string GetValueOfLast(IHTMLElement activeElement)
        {
            if (activeElement.GetHashCode() == this.lastTypeHashCode)
            {
                return lastTypeValue;
            }
            else
            {
                this.lastTypeHashCode = activeElement.GetHashCode();
                this.lastTypeValue = "";
            }
            return this.lastTypeValue;
        }
        private string GetValueOfElement(IHTMLElement activeElement)
        {
            string elementAttr = "";
            try
            {
                if (activeElement != null)
                {
                    string type = activeElement.getAttribute("type", 0).ToString();
                    if (type.ToLower() == "text" || type.ToLower() == "password" || activeElement.tagName.ToLower() == "textarea")
                    {
                        elementAttr = activeElement.getAttribute("value", 0).ToString();
                    }
                }
            }
            catch (Exception)
            {

            }
            return elementAttr;
        }
        private string GetCodeFile(Template selectedTemplate)
        {
            var nvcCode = new NameValueCollection();
            selectedTemplate.ResetFormatter();
            selectedTemplate.CodeFormatter.FileDestination = true;

            foreach (ActionList actionList in ActionListSet)
            {
                nvcCode.Add("test",actionList.GetCode(selectedTemplate.CodeFormatter, BrowserTypeTarget));
            }

            string code = selectedTemplate.PrepareScript(nvcCode);
            return code;
        }

        public void SaveCode(string filename, Template selectedTemplate)
        {
            string code = GetCodeFile(selectedTemplate);
            try
            {
                File.WriteAllText(filename, code);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save code:" + Environment.NewLine + ex.Message, "Save Failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SaveXML(string filename)
        {
            var writer = new XmlTextWriter(filename, Encoding.UTF8) {Formatting = Formatting.Indented};
            writer.WriteStartDocument();

            writer.WriteStartElement("WatinTests");

            CurrentLoadContext.SaveToXml(writer);

            foreach (ActionList actionList in ActionListSet)
            {
                actionList.SaveXml(writer);
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// Loads code from a file, must be in native format
        /// </summary>
        /// <param name="Filename">Filename to load from</param>
        public void LoadFromXML(BrowserWindow Browser, string Filename)
        {
            if (!File.Exists(Filename))
            {
                MessageBox.Show(
                    string.Format(Properties.Resources.FileDoesNotExist, Filename),
                    Properties.Resources.FileError,
                    MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                return;
            }

            if (this.UnsavedScript)
            {
                if (MessageBox.Show(
                    Properties.Resources.EraseAndContinueLoading,
                    Properties.Resources.Confirmation,
                    MessageBoxButtons.YesNo
                    ) == DialogResult.No)
                    return;
            }

            try
            {
                var doc = new XmlDocument();
                doc.Load(Filename);
                XmlNode root = doc.DocumentElement;
                if (root == null) return;

                XmlNode pageListNode = root.SelectSingleNode("PageList");
                CurrentLoadContext.LoadFromXML(pageListNode, Browser);

                XmlNodeList nodeList = root.SelectNodes("Test");
                if (nodeList == null) return;
                ActionListSet.Clear();
                foreach (XmlNode node in nodeList)
                {
                    ActionListSet.Add(this.LoadActionList(Browser,node));
                }

                if (ActionListSet.Count > 0)
                {
                    this.ActiveList = ActionListSet[0];
                    this.CurrentName = this.ActiveList.TestName;

                    foreach (ActionBase _act in ActiveList)
                    {
                        if (this.OnGridAdd != null) this.OnGridAdd(_act);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private ActionList LoadActionList(BrowserWindow Browser, XmlNode parentNode)
        {
            ActionList list = new ActionList();
            list.TestName= parentNode.Attributes["Name"].Value;
            try
            {
                XmlNodeList nodeList = parentNode.SelectNodes("Action");
                foreach (XmlNode node in nodeList)
                {
                    ActionContext context = this.GetActionContext(Browser, null);
                    ActionBase action = context.StringToObject(node.Attributes["ActionType"].Value);
                    if (action == null)
                    {
                        MessageBox.Show("Action " + node.Name + " could not be mapped-- application needs updating.", "Load Map Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }
                    action.SetContext(context);
                    action.LoadFromXml( node);
                    
                    list.Add(action);
                }

                XmlNode nodeData = parentNode.SelectSingleNode("ReplacementData");
                if (nodeData != null) list.LoadDataFromXml(nodeData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return list;
        }
       

        /// <summary>
        /// Uses Win32 calls to find whether the foreground window is a file dialog
        /// </summary>
        /// <returns>true if it is a file dialog</returns>
        private bool FileDialogFound()
        {
            IntPtr win = GetForegroundWindow();
            long lstyle = GetWindowStyle(win);

            if (lstyle.ToString("X") == "96CC20C4" || lstyle.ToString("X") == "96CC02C4")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets a timer to watch for a file dialog box
        /// </summary>
        /// <param name="browser">Browser running this dialog</param>
        /// <param name="activeElement">Element to check after dialog is found</param>
        private void WatchFileUploadBox(BrowserWindow browser, IHTMLElement activeElement)
        {
            BlnFileDialogFound = false;
            FileElementBrowser = browser;
            FileActiveElement = activeElement;
            TimerFileDialog = new System.Timers.Timer(1000);
            TimerFileDialog.Elapsed += TimerFileDialogElapsed;
            TimerFileDialog.Enabled = true;
        }

        /// <summary>
        /// Timer event for the file dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerFileDialogElapsed(object sender, ElapsedEventArgs e)
        {
            ActionContext context = this.GetActionContext(null, null);
            if (!BlnFileDialogFound)
            {
                if (FileDialogFound())
                {
                    BlnFileDialogFound = true;
                }
                else
                {
                    return;
                }
            }

            if (FileDialogFound())
            {
                return;
            }

            TimerFileDialog.Enabled = false;

            var fileDialog = new ActionFileDialog (context);
            fileDialog.SetFindMethod(FileElementBrowser, FileActiveElement);
            fileDialog.Filename = this.GetElementAttr(FileActiveElement, "value");
            if (fileDialog.Filename == "") return;
            AddAction(fileDialog);

            BlnFileDialogFound = false;
        }
        private string GetElementAttr(IHTMLElement element, string AttributeName)
        {
            if (element == null)
            {
                return "";
            }
            string strValue = "";
            try
            {
                strValue = element.getAttribute(AttributeName, 0) as string ?? "";
            }
            catch (Exception)
            {
            }
            return strValue;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWINFO
        {
            public uint cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        public static Int64 GetWindowStyle(IntPtr hwnd)
        {
            var info = new WINDOWINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);
            GetWindowInfo(hwnd, ref info);
            return Convert.ToInt64(info.dwStyle);
        }

        #region CompileScript

        public string CompileScript(Template templateObj, bool runScript)
        {
            return CompileScript(ActiveList.GetCode(templateObj.CodeFormatter, BrowserTypeTarget), templateObj, runScript);
        }

        private static bool AssemblyAlreadyInList(string filename, StringCollection assemblyList)
        {
            for (int i = 0; i < assemblyList.Count; i++)
            {
                string nameonly = Path.GetFileName(assemblyList[i]).ToLower();
                if (Path.GetFileName(filename).ToLower()==nameonly)
                {
                    return true;
                }
            }
            return false;
        }

        private  string CompileScript(string scriptCode, Template templateObj, bool runScript)
        {
            if (ActionListSet.Count==0)
            {
                return Properties.Resources.NoTestsToCompile;
            }

            if (!templateObj.CanCompile)
            {
                	return Properties.Resources.TargetTemplateIsSetNotToCompile;
            }

            var functionAssemblies = new StringCollection();
            if (!Template.AllFilesExistInList(templateObj.ReferencedAssemblies) || !Template.AllFilesExistInList(templateObj.IncludedFiles))
            {
                var frm = new frmLocateResource();
                frm.ShowResourceList(templateObj, functionAssemblies, true);

                // make sure all items can be found
                if (!Template.AllFilesExistInList(templateObj.ReferencedAssemblies) || !Template.AllFilesExistInList(templateObj.IncludedFiles))
                {
                    return Properties.Resources.NecessaryCodeFilesCouldNotBeFound;
                }
            }

            if (!Settings.CompilePath.EndsWith(@"\"))
            {
                Settings.CompilePath = Path.GetDirectoryName(Settings.CompilePath)+"\\";
            }
                        
            if (!Directory.Exists(Settings.CompilePath))
            {
                try
                {
                    Directory.CreateDirectory(Settings.CompilePath);
                }
                catch (Exception)
                {
                    return string.Format(Properties.Resources.CompilePathCouldNotBeCreated,Settings.CompilePath);
                }                
            }

            var sbErrors = new StringBuilder();

            var cps = new CompilerParameters
                          {
                              OutputAssembly = Path.Combine(Settings.CompilePath, "WatiNOutput.exe"),
                              GenerateExecutable = true,
                              IncludeDebugInformation = true
                          };

            if (!templateObj.CanRun)
            {
                cps.OutputAssembly = Path.ChangeExtension(cps.OutputAssembly, ".dll");
                cps.GenerateExecutable = false;
            }

            StringCollection scAssemblies = templateObj.GetAssemblyList();
            for (int i = 0; i < scAssemblies.Count; i++)
            {
                cps.ReferencedAssemblies.Add(scAssemblies[i]);
            }

            // add assemblies from function explorer
            for (int i = 0; i < functionAssemblies.Count; i++)
            {
                if (!AssemblyAlreadyInList(functionAssemblies[i], cps.ReferencedAssemblies))
                {
                    cps.ReferencedAssemblies.Add(functionAssemblies[i]);
                }
            }

            var sourcefiles = new string[templateObj.IncludedFiles.Count + 1];
            for (int i = 0; i < templateObj.IncludedFiles.Count; i++)
            {
                if (templateObj.IncludedFiles[i].Trim()=="")
                {
                    continue;
                }
                sourcefiles[i] = File.ReadAllText(templateObj.IncludedFiles[i]);
            }

            var nvTest = new NameValueCollection {{"CurrentTest", scriptCode}};
            sourcefiles[sourcefiles.Length - 1] = templateObj.PrepareScript(nvTest);            
            
            // Compile the source code
            CompilerResults cr = null;
            try
            {
                if (templateObj.CodeLanguage==AppSettings.CodeLanguages.CSharp)
                {
                    var codeprovider = new Microsoft.CSharp.CSharpCodeProvider();
                    cr = codeprovider.CompileAssemblyFromSource(cps, sourcefiles);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Properties.Resources.CompilerError,ex.Message));
            }
            
            // Check for errors
            if (cr != null)
                if (cr.Errors.Count > 0)
                {
                    // Has errors so display them
                    foreach (CompilerError ce in cr.Errors)
                    {
                        sbErrors.AppendFormat(Properties.Resources.ErrorLineListing+Environment.NewLine, ce.ErrorNumber, ce.Line, ce.Column, ce.ErrorText, (ce.IsWarning) ? "Warning" : "Error");
                    }
                    Debug.WriteLine(sbErrors.ToString());
                    return sbErrors.ToString();
                }

            // copy imported assemblies (not in .NET main)
            string netPath = RuntimeEnvironment.GetRuntimeDirectory();
            for (int i = 0; i < scAssemblies.Count; i++)
            {
                if (scAssemblies[i] == null || scAssemblies[i].Trim() == "")
                {
                    continue;
                }
                if (!File.Exists(scAssemblies[i]))
                {
                    return string.Format(Properties.Resources.CompileSuccessfulButCantFind,scAssemblies[i]);
                }

                if (Path.GetDirectoryName(netPath) != Path.GetDirectoryName(scAssemblies[i]))
                {
                    File.Copy(scAssemblies[i], Path.Combine(Settings.CompilePath, Path.GetFileName(scAssemblies[i])), true);
                }
                
            }

            if (runScript && templateObj.CanRun)
            {
                string scriptrun = RunScriptOutput(templateObj.StartupApplication, cps.OutputAssembly);
                if (scriptrun != "")
                {
                    return scriptrun;
                }
            }

            return "";
        }
        /// <summary>
        /// 输出脚本
        /// </summary>
        /// <param name="startupApplication"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string RunScriptOutput(string startupApplication, string filename)
        {
            try
            {
                ProcessStartInfo info = startupApplication.Trim()=="" ? new ProcessStartInfo(filename) : new ProcessStartInfo(startupApplication) {Arguments = filename};
                if (Settings.HideDOSWindow)
                {
                    info.WindowStyle = ProcessWindowStyle.Hidden;
                }
                Process.Start(info);
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return "";
        }
        #endregion
        
    }
}
