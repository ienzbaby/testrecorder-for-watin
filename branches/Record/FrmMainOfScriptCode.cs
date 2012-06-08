using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TestRecorder.Core;
using System.Drawing;
using System.Data;
using TestRecorder.Tools;
using TestRecorder.Core.Actions;
using TestRecorder.UserControls;
using System.IO;
using System.Text;
using WatiN.Core.Exceptions;

namespace TestRecorder
{
    public partial class frmMain : Form
    {
        public RunnerManager Runner = new RunnerManager();

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (wsManager.Recording) return;

            // Check we have a selected action or are at the top
            if (currentIndex < 1) return;
            // Check we have more than 1 action
            if (wsManager.ActiveList.Count < 2) return;
            // Create temp action
            var action = wsManager.ActiveList[currentIndex];
            // Remove the action from the list
            wsManager.ActiveList.RemoveAt(currentIndex);
            // Insert ther action at the appropriate place
            wsManager.ActiveList.Insert(currentIndex - 1, action);
            // Update the grid
            ClearGrid();
            foreach (ActionBase localAction in wsManager.ActiveList)
            {
                try
                {
                    AddGridRowItem(localAction);
                }
                catch (Exception)
                {
                    // swallow the exception
                }
            }
            gridSource.Selection.FocusRow(currentIndex);
            currentIndex -= 1;
            gridSource.Refresh();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (wsManager.Recording) return;

            // Check we have a selected action
            if (currentIndex < 0) return;
            // Check we have more than 1 action
            if (wsManager.ActiveList.Count < 2) return;
            // Check if we are at the bottom
            if (currentIndex == wsManager.ActiveList.Count - 1) return;
            // Create temp action
            var action = wsManager.ActiveList[currentIndex];
            // Remove the action from the list
            wsManager.ActiveList.RemoveAt(currentIndex);
            // Insert ther action at the appropriate place
            wsManager.ActiveList.Insert(currentIndex + 1, action);
            // Update the grid
            ClearGrid();
            foreach (ActionBase localAction in wsManager.ActiveList)
            {
                try
                {
                    AddGridRowItem(localAction);
                }
                catch (Exception)
                {
                    // swallow the exception
                }
            }
            currentIndex += 1;
            gridSource.Selection.FocusRow(currentIndex + 1);
            gridSource.Refresh();
        }
        private void LoadWatiNScript()
        {
            if (watinIE.Html.IndexOf("WatiNScript") == -1)
            {
                string path = Path.GetDirectoryName(Application.ExecutablePath) + @"\Resources\WatiNScript.js";
                using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
                {
                    string strWatiN = sr.ReadToEnd();
                    watinIE.RunScript(strWatiN);
                }
            }
        }
        private void btnScriptRun_Click(object sender, EventArgs e)
        {
            try
            {
                this.LoadWatiNScript();
                if (txtScript.SelectionLength > 0)
                {
                    watinIE.RunScript(txtScript.SelectedText);
                }
                else
                {
                    watinIE.RunScript(txtScript.Text);
                }
            }
            catch (RunScriptException re)
            {
                MessageBox.Show(re.Message);
            }
        }

        private void btnScriptStop_Click(object sender, EventArgs e)
        {
            try
            {
                watinIE.RunScript(";");
            }
            catch (RunScriptException re)
            {
                MessageBox.Show(re.Message);
            }
        }
        private void txtScript_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    e.Handled = true;
                    string strInsertText = Clipboard.GetText();
                    int start = this.txtScript.SelectionStart;
                    this.txtScript.Text = this.txtScript.Text.Insert(start, strInsertText);
                    this.txtScript.Focus();
                    this.txtScript.SelectionStart = start;
                    //this.txtScript.SelectionLength = strInsertText.Length;
                }
                else if (e.Control && e.KeyCode == Keys.C)
                {
                    if (txtScript.SelectedText.Length > 0)
                    {
                        Clipboard.SetText(txtScript.SelectedText);
                    }
                }
            }
            catch (Exception eex)
            {
                MessageBox.Show(eex.Message, "txtScript_KeyDown");
            }
        }


        #region ScriptCodeInterface
        private void tsbClickElement_CheckedChanged(object sender, EventArgs e)
        {
            if (tsbClickElement.Checked == true && wsManager.Recording == false)
            {
                this.tsbRecord_Click(null, null);
            }

            if (tsbClickElement.Checked == true && wsManager.Recording == false)
            {
                MessageBox.Show("Element actions can be saved only when recording!");
                tsbClickElement.Checked = false;
                return;
            }
            else
            {
                highlightingElements = tsbClickElement.Checked;
                HighlightElement(null);
            }
        }
        /// <summary>
        /// 打开用户控件编辑器 
        /// </summary>
        /// <param name="action"></param>
        private void ShowEditAction(ActionBase action)
        {
            actionEditing = action;
            ucEditor = action.GetEditor() as ucBaseEditor;

            if (ucEditor == null) return;

            ucEditor.ActionList = wsManager.ActiveList;
            ucEditor.Action = action;
            ucEditor.Dock = DockStyle.Bottom;
            ucEditor.BringToFront();
            ucEditor.Height = tabControl.Height;
            Controls.Add(ucEditor);

            //添加监听事件
            ucEditor.OnCloseEdtion += this.CloseEditAction;
            tabControl.Visible = false;
            ucEditor.Show();

            statusStripMain.SendToBack();
        }

        internal void CloseEditAction(DialogResult result)
        {
            if (ucEditor == null) return;
            if (result == DialogResult.OK)
            {
                actionEditing = ucEditor.Action;
                this.RefreshGrid();
            }
            ucEditor.Dispose();
            tabControl.Visible = true;
        }

        private void tsbEditAction_Click(object sender, EventArgs e)
        {
            if (gridSource.Selection.ActivePosition.Row == -1) return;
            var action = wsManager.ActiveList[gridSource.Selection.ActivePosition.Row - 1];
            ShowEditAction(action);
        }

        private void btnDisableAllBreakpoints_Click(object sender, EventArgs e)
        {
            Runner.DisableAllBreakpoints();
        }

        private void btnDeleteAllBreakpoints_Click(object sender, EventArgs e)
        {
            Runner.DeleteAllBreakpoints();
        }

        private void tsbAddAction_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            AddActionByName(e.ClickedItem.Text);
        }

        private void AddActionSelection(object sender, EventArgs e)
        {
            AddActionByName(((ToolStripItem)sender).Text);
        }

        private void AddActionByName(string actionName)
        {
            switch (actionName)
            {
                case "Alert Handler": wsManager.AddAlertHandler(watinIE); break;
                case "Javascript": wsManager.AddJavascriptHandler(watinIE); break;
                case "Back": wsManager.AddBack(watinIE); break;
                case "Click": wsManager.AddClick(watinIE, textActiveElement,false); break;
                case "Close Window": wsManager.AddClosePopup(watinIE); break;
                case "Double Click": wsManager.AddEvent(watinIE, textActiveElement, "ondblClick"); break;
                case "Fire Event": wsManager.AddEvent(watinIE, textActiveElement, ""); break;
                case "Forward": wsManager.AddForward(watinIE); break;
                case "Key Press": wsManager.AddEvent(watinIE, textActiveElement, "OnKeyPress"); break;
                case "Key Up": wsManager.AddEvent(watinIE, textActiveElement, "OnKeyUp"); break;
                case "Key Down": wsManager.AddEvent(watinIE, textActiveElement, "OnKeyDown"); break;
                case "Mouse Down": wsManager.AddEvent(watinIE, textActiveElement, "OnMouseDown"); break;
                case "Mouse Up": wsManager.AddEvent(watinIE, textActiveElement, "OnMouseUp"); break;
                case "Navigate": wsManager.AddNavigation(watinIE,watinIE.Url, "", ""); break;
                case "Highlight": wsManager.AddHighlight(watinIE, textActiveElement); break;
                case "Open Popup": wsManager.AddPopup( watinIE,"Popup Window",watinIE.Url); break;
                case "Refresh": wsManager.AddRefresh(watinIE); break;
                case "Select List": wsManager.AddSelectListItem(watinIE, textActiveElement, false); break;
                case "Test Element (Add Assert)": wsManager.AddTestElement(watinIE, textActiveElement); break;
                case "Type Text": wsManager.AddTyping(watinIE, textActiveElement); break;
                case "Until Exists": wsManager.AddWait(watinIE, textActiveElement, ActionWait.WaitTypes.Exists); break;
                case "Until Removed": wsManager.AddWait(watinIE, textActiveElement, ActionWait.WaitTypes.Removed); break;
                default: MessageBox.Show("sorry，interface of '" + actionName + "' is not be implemented now!"); break;
            }
        }

        private void BreakpointReached(ActionBase action)
        {
            if (InvokeRequired)
            {
                Invoke(new BreakpointEvent(BreakpointReached), action);
                return;
            }

            btnStep.Enabled = true;
            btnContinue.Enabled = true;
        }
        private void tsbDeleteAction_Click(object sender, EventArgs e)
        {
            int iIndex = gridSource.Selection.ActivePosition.Row;
            if (iIndex == -1) return;

            wsManager.ActiveList.RemoveAt(iIndex - 1);
            this.DeleteGridRow(iIndex - 1);
        }
        private void tsbClearTest_Click(object sender, EventArgs e)
        {
            wsManager.ActiveList.Clear();
            this.ClearGrid();
        }
        private void tsbStopRunning_Click(object sender, EventArgs e)
        {
            Runner.Stop();
        }
        /// <summary>
        /// 停止事件
        /// </summary>
        /// <param name="isAbort"></param>
        private void RunFinished(bool isAbort)
        {
            if (--Count >= 0) this.txtTestNum.Text = Count.ToString();

            if (Count > 0 && isAbort == false)
            {
                Runner.Init(wsManager.ActiveList);
                Runner.Run();//======执行脚本=======
                return;
            }

            if (InvokeRequired)
            {
                Invoke(new RunCompleteEvent(RunFinished), new object[] { isAbort });
                return;
            }

            tsScript.Text = "";
            scriptTimer.Stop();
            tsbRunCurrent.Visible = true;
            pnlRun.Visible = false;
        }
        /// <summary>
        /// 运行全部用例
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbRun_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lbTests.Items.Count; i++)
            {
                StartInitTest();
                lbTests.SelectedIndex = i;
                Runner.Init(wsManager.ActionListSet[i]);
                Runner.Run();
            }
        }

        private void StartInitTest()
        {
            int.TryParse(this.txtTestNum.Text, out Count);
            if (Count < 1) Count = 1;
            if (Count > 500)
            {
                MessageBox.Show("单次循环不能超过500次！");
                return;
            }

            txtTestNum.Text = Count.ToString();
            tsScript.Text = "Script Is Running";
            scriptTimer.Interval = 1000;
            scriptTimer.Tick += delegate(object senderTimer, EventArgs eTimer)
            {
                tsScript.ForeColor = DateTime.Now.Second % 2 == 0 ? Color.Blue : Color.Red;
                tsScript.Invalidate();//重绘
            };

            scriptTimer.Start();

            tsbRunCurrent.Visible = false;
            pnlRun.Visible = true;
        }
        private void tsbRunCurrent_Click(object sender, EventArgs e)
        {
            StartInitTest();
            Runner.Init(wsManager.ActiveList);
            Runner.Run();//======执行脚本=======
        }
       
        private void tsbRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDOM(_CurWB.WebbrowserObject.Document);
                LoadWatinTree(watinIE);
            }
            catch (Exception ex)
            {
                MessageBox.Show("DOM refresh failed:" + Environment.NewLine + ex.Message, "DOM Refresh Failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            btnStep.Enabled = false;
            btnContinue.Enabled = false;
            Runner.Continue();
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            Runner.RunStep(currentIndex - 1);
        }

        private void menuSettings_Click(object sender, EventArgs e)
        {
            var frm = new frmSettings();
            AppSettings.CodeLanguages language = wsManager.Settings.CodeLanguage;
            wsManager.Settings.documentPath = Application.StartupPath + "\\settings.xml";
            wsManager.Settings.LoadSettings();
            frm.ObjectToGUI(wsManager.Settings);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                frm.GUIToObject(wsManager.Settings);
                // Save setting
                wsManager.Settings.SaveSettings();
                // language changed?
                if (language != wsManager.Settings.CodeLanguage)
                {
                    if (MessageBox.Show(Properties.Resources.LanguageChange, Properties.Resources.ChangeLanguage, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        Application.Restart();
                    }
                }
            }
            frm.Dispose();
        }
        /// <summary>
        /// 加载脚本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbLoad_Click(object sender, EventArgs e)
        {
            if (openCodeDialog.ShowDialog() == DialogResult.Cancel) return;

            if (wsManager.Recording) tsbStop_Click(null, e); //停止录制脚本

            this.ClearGrid();
            wsManager.LoadFromXML(this.watinIE, openCodeDialog.FileName);
            this.Refresh();

            lbTests.Items.Clear();
            for (int i = 0; i < wsManager.ActionListSet.Count; i++)
            {
                lbTests.Items.Add(wsManager.ActionListSet[i].TestName);

                if (i == 0)
                {
                    lbTests.SelectedIndex = 0;
                    txtTestName.Text = wsManager.ActionListSet[0].TestName;
                }
            }
        }
        private void tsbSave_Click(object sender, EventArgs e)
        {
            if (wsManager.Recording)
            {
                tsbStop_Click(null, e);
            }
            
            saveCodeDialog.Filter = "Native XML Format (*.xml)|*.xml|" + templatesAvailable.GetSaveFilter();
            if (saveCodeDialog.ShowDialog() != DialogResult.OK
                || wsManager.ActionListSet.Count == 0) return;

            // get the selected template
            if (saveCodeDialog.FilterIndex == 1) wsManager.SaveXML(saveCodeDialog.FileName);
            else
            {
                Template selectedTemplate = templatesAvailable.GetTemplate(saveCodeDialog.FilterIndex - 2);
                wsManager.SaveCode(saveCodeDialog.FileName, selectedTemplate);
            }
        }

        private BrowserTypes GetBrowserTypeSelected()
        {
            for (int i = 0; i < tsbBrowserSelect.DropDownItems.Count; i++)
            {
                if (tsbBrowserSelect != null)
                    if (((ToolStripMenuItem)tsbBrowserSelect.DropDownItems[i]).Checked)
                        return (BrowserTypes)i;
            }
            return BrowserTypes.IE;
        }

        private void cToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string code = wsManager.ActiveList.GetCode(new CSharpCodeFormatter { FileDestination = false }, GetBrowserTypeSelected());
            Clipboard.SetText(code);
        }

        private void vBNETToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string code = wsManager.ActiveList.GetCode(new VBNetCodeFormatter { FileDestination = false }, GetBrowserTypeSelected());
            Clipboard.SetText(code);
        }

        private void tsbRecord_Click(object sender, EventArgs e)
        {
            if (txtTestName.Text.Trim() == "")
            {
                MessageBox.Show(Properties.Resources.TestsMustHaveAName,
                    Properties.Resources.TestName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
               );
                return;
            }
            if (!Regex.IsMatch(txtTestName.Text, @"\A[a-zA-Z0-9_]+\z"))
            {
                MessageBox.Show(Properties.Resources.TestNameCanContainOnlyAZAnd09,
                    Properties.Resources.TestName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            tsbRecord.Enabled = false;
            tsbStop.Enabled = true;
            txtTestName.Enabled = false;

            wsManager.CurrentName = txtTestName.Text;
            wsManager.Recording = true;
            //wsManager.ClearTimer();

            // inserting items into the current position
            if (wsManager.ActiveList.Count > 0 && gridSource.Selection.ActivePosition.Row + 1 < gridSource.RowsCount)
            {
                wsManager.InsertPosition = gridSource.Selection.ActivePosition.Row;
                if (wsManager.InsertPosition == -1)
                {
                    wsManager.InsertPosition = 0;
                }
            }
            else
            {
                wsManager.InsertPosition = -1;
            }
        }

        private void tsbStop_Click(object sender, EventArgs e)
        {
            tsbRecord.Enabled = true;
            tsbStop.Enabled = false;
            txtTestName.Enabled = true;
            wsManager.Recording = false;
        }

        private void tsbDeleteTest_Click(object sender, EventArgs e)
        {
            if (lbTests.SelectedIndex == -1) return;

            int indexTest = wsManager.GetTestIndex(lbTests.SelectedItem.ToString());
            wsManager.ActionListSet.RemoveAt(indexTest);
            lbTests.Items.RemoveAt(indexTest);

            this.tsbClearTest_Click(sender, e);
        }

        private void lbTests_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if the test name is different, stop recording, save the current (if not blank) and load the new
            if (lbTests.SelectedIndex == -1) return;

            if (lbTests.SelectedItem.ToString() != wsManager.CurrentName && wsManager.Recording)
            {
                tsbStop_Click(null, e);
            }

            //txtCode.Text = wscript.RecordedTests[lbTests.SelectedItem];

            txtTestName.Text = lbTests.SelectedItem.ToString();
            wsManager.ActiveList = wsManager.FindTestByName(txtTestName.Text);
        }

        private void txtTestName_TextChanged(object sender, EventArgs e)
        {
            txtTestName.Text = Regex.Replace(txtTestName.Text, @"[^a-zA-Z0-9_]+", "");
            txtTestName.SelectionStart = txtTestName.Text.Length;
        }

        private void watiNHomePageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NavToUrl("http://watin.sourceforge.net");
        }
        #endregion

      
    }
}
