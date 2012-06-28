using System;
using System.Reflection;
using System.Web;
using System.Windows.Forms;
using IfacesEnumsStructsClasses;
using TestRecorder.Core;
using System.IO;
using TestRecorder.Core.Actions;
using System.Drawing;
using TestRecorder.Tools;

namespace TestRecorder
{
    public partial class frmMain : Form
    {
        private void tsHelpMnuContents_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, helpProvider1.HelpNamespace, HelpNavigator.TableOfContents);
        }

        private void tsHelpMnuIndex_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, helpProvider1.HelpNamespace, HelpNavigator.Index);
        }

        private void tsHelpMnuSearch_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, helpProvider1.HelpNamespace, HelpNavigator.Find, "");
        }

        private void menuCheckForUpdates_Click(object sender, EventArgs e)
        {
            NavToUrl("http://watintestrecord.sourceforge.net/version.php?version=" + Assembly.GetExecutingAssembly().GetName().Version);
        }

        private void menuSpecificEvent_Click(object sender, EventArgs e)
        {
            var frm = new frmFireSpecificEvent();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                wsManager.AddEvent(watinIE, textActiveElement, frm.txtEventName.Text);
            }
        }

        private void menuWaitTime_Click(object sender, EventArgs e)
        {
            var frm = new frmWaitTime();
            frm.WaitTime = wsManager.Settings.GlobalWaitTime;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                wsManager.AddSleep(Convert.ToInt32(frm.WaitTime));
            }
        }

        private void menuWaitValue_Click(object sender, EventArgs e)
        {
            var frm = new frmAttributeEqualsValue();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                ActionWait wait = wsManager.AddWait(watinIE, textActiveElement, ActionWait.WaitTypes.AttributeValue);
                wait.AttributeName = frm.txtAttribute.Text;
                wait.AttributeValue = frm.txtValue.Text;
                wait.AttributeRegex = frm.chkRegex.Checked;
            }
        }

        private void menuBrowserSelect_Click(object sender, EventArgs e)
        {
            // uncheck the others
            int checkedItem = Convert.ToInt32(((ToolStripMenuItem)sender).Tag);
            wsManager.BrowserTypeTarget = (BrowserTypes)checkedItem;
            for (int i = 0; i < tsbBrowserSelect.DropDownItems.Count; i++)
            {
                if (checkedItem == i) continue;
                ((ToolStripMenuItem)tsbBrowserSelect.DropDownItems[i]).Checked = false;
            }
        }
        private void feedbackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFeedback();
        }

        private static void ShowFeedback()
        {
            var frm = new frmFeedback();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                EmailHelper.SendMail("", "Comments", frm.txtComments.Text, false, false);
            }
        }

        private void timerFeedback_Tick(object sender, EventArgs e)
        {
            timerFeedback.Enabled = false;
            if (wsManager.Settings.RunCount > 0 && wsManager.Settings.RunCount % 5 == 0)
            {
                ShowFeedback();
            }
        }
        private void comboSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;

                if (comboSearch.Text.Length == 0)
                    return;

                string str = comboSearch.Text.Replace(" ", "+");
                str = string.Format(Properties.Resources.SearchURL, str);
                NavToUrl(str);
            }
        }

        private void GoSearchToolStripButtonClickHandler(object sender, EventArgs e)
        {
            try
            {
                if (sender == tsSplitBtnSearch)
                {
                    if (comboSearch.Text.Length == 0)
                        return;

                    string str = comboSearch.Text.Replace(" ", "+");
                    str = string.Format(Properties.Resources.SearchURL, str);
                    NavToUrl(str);
                }
                else if (sender == tsBtnGo)
                {
                    if (tsChkBtnGo.Checked) //Open in a new background browser
                    {
                        AddNewBrowser(Blank, AboutBlank, comboURL.Text, false);
                    }
                    else
                        NavToUrl(comboURL.Text);
                }
                else if (sender == tsBtnBack)
                {
                    if (_CurWB.CanGoBack)
                    {
                        wsManager.AddBack(watinIE);
                        _CurWB.GoBack();
                    }

                }
                else if (sender == tsBtnForward)
                {
                    if (_CurWB.CanGoForward)
                    {
                        wsManager.AddForward(watinIE);
                        _CurWB.GoForward();
                    }
                }
                else if (sender == tsBtnRefresh)
                {
                    wsManager.AddRefresh(watinIE);
                    _CurWB.Refresh();
                }

                else if (sender == tsBtnStop)
                    _CurWB.Stop();
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("GoSearchToolStripButtonClickHandler\r\n" + ee);
            }
        }

        /// <summary>
        /// URL combo click handler,复制、粘贴，回车
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboURL_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Return)
                {
                    e.Handled = true;
                    NavToUrl(comboURL.Text);
                }
                else if (e.Control && e.KeyCode == Keys.V)
                {
                    e.Handled = true;
                    comboURL.Text = Clipboard.GetText();
                    NavToUrl(Clipboard.GetText());
                    cEXWB1.Focus();
                }
                else if (e.Control && e.KeyCode == Keys.C)
                {
                    Clipboard.SetText(comboURL.Text);
                }
            }
            catch (Exception eex)
            {
                MessageBox.Show(eex.Message, "comboUrl_KeyUp");
            }
        }

        //Use a FileSystemWatcher to determine whether to reload favorites upon
        //dropping down or not.
        //To be more effeicent, I would have, in case of
        //Create, insert a new menu item in the appropriate index
        //delete, remove the menu item
        //renamed, modify the text
        //changed, modify text and/or url
        private bool FavNeedReload;

        private void fswFavorites_Created(object sender, FileSystemEventArgs e)
        {
            FavNeedReload = true;
            //e.ChangeType;
            //e.FullPath;
            //e.Name;
        }

        private void fswFavorites_Deleted(object sender, FileSystemEventArgs e)
        {
            FavNeedReload = true;
            //try
            //{
            //    //If a link then we remove it
            //    ToolStripItem itema = null;
            //    foreach (ToolStripItem item in tsLinks.Items)
            //    {
            //        if (item.Name == e.Name)
            //        {
            //            itema = item;
            //            break;
            //        }
            //    }
            //    if (itema != null)
            //        tsLinks.Items.Remove(itema);
            //}
            //catch (Exception ee)
            //{
            //    AllForms.FrmLog.AppendToLog("fswFavorites_Deleted\r\n" + ee);
            //}
        }

        private void fswFavorites_Renamed(object sender, RenamedEventArgs e)
        {
            FavNeedReload = true;
        }

        private void fswFavorites_Changed(object sender, FileSystemEventArgs e)
        {
            FavNeedReload = true;
        }

        private void LoadFavoriteMenuItems()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                var objDir = new DirectoryInfo(fswFavorites.Path);
                //Recurse, starting from main dir
                LoadFavoriteMenuItems(objDir, tsFavoritesMnu);
                Cursor = Cursors.Default;
            }
            catch (Exception ee)
            {
                Cursor = Cursors.Default;
                FormHelper.FrmLog.AppendToLog("LoadFavoriteMenuItems\r\n" + ee);
            }
        }

        /// <summary>
        /// Recursive function
        /// </summary>
        /// <param name="objDir"></param>
        /// <param name="menuitem"></param>
        private void LoadFavoriteMenuItems(DirectoryInfo objDir, ToolStripDropDownItem menuitem)
        {
            try
            {
                string strName;
                string strUrl;

                DirectoryInfo[] dirs = objDir.GetDirectories();
                foreach (DirectoryInfo dir in dirs)
                {
                    var diritem = new ToolStripMenuItem(dir.Name, tsFileMnuOpen.Image);
                    menuitem.DropDownItems.Add(diritem);
                    LoadFavoriteMenuItems(dir, diritem);
                }

                bool addlinks = (objDir.Name.Equals("links",
                    StringComparison.CurrentCultureIgnoreCase)) ? true : false;
                FileInfo[] urls = objDir.GetFiles("*.url");
                foreach (FileInfo url in urls)
                {
                    strName = Path.GetFileNameWithoutExtension(url.Name);
                    strUrl = _CurWB.ResolveInternetShortCut(url.FullName);
                    //load up quick links
                    if (addlinks)
                    {
                        var btn = new ToolStripButton(strName, ImgUnLock) { Tag = strUrl };
                        btn.Click += ToolStripLinksButtonClickHandler;
                        tsLinks.Items.Add(btn);
                    }
                    var item = new ToolStripMenuItem(strName, ImgUnLock) { Tag = strUrl };
                    item.Click += ToolStripFavoritesMenuClickHandler;
                    menuitem.DropDownItems.Add(item);
                }
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("LoadFavoriteMenuItems\r\n" + ee);
            }
        }
        private void tsFavoritesMnu_DropDownOpening(object sender, EventArgs e)
        {
            if (!FavNeedReload)
                return;
            FavNeedReload = false;
            try
            {
                //Reload favorites
                if (tsFavoritesMnu.DropDownItems.Count > 3)
                {
                    //Remove from back to front except the original items
                    for (int i = tsFavoritesMnu.DropDownItems.Count - 1; i > 2; i--)
                    {
                        if ((tsFavoritesMnu.DropDownItems[i] != tsFavoritesMnuAddToFavorites) &&
                            (tsFavoritesMnu.DropDownItems[i] != tsFavoritesMnuOrganizeFavorites) &&
                            (tsFavoritesMnu.DropDownItems[i] != tsFavoritesMnuSeparator))
                        {
                            tsFavoritesMnu.DropDownItems.Remove(tsFavoritesMnu.DropDownItems[i]);
                        }
                    }
                    for (int i = tsLinks.Items.Count - 1; i > 0; i--)
                    {
                        tsLinks.Items.Remove(tsLinks.Items[i]);
                    }
                }
                //Load favorites
                LoadFavoriteMenuItems();
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("tsFavoritesMnu_DropDownOpening\r\n" + ee);
            }
        }

        private void ToolStripFavoritesMenuClickHandler(object sender, EventArgs e)
        {
            try
            {
                if (sender == tsFavoritesMnuAddToFavorites)
                {
                    _CurWB.AddToFavorites();
                }
                else if (sender == tsFavoritesMnuOrganizeFavorites)
                {
                    _CurWB.OrganizeFavorites();
                }
                var item = (ToolStripItem)sender;
                if (item.Tag != null)
                {
                    NavToUrl(item.Tag.ToString());
                    comboURL.Text = item.Tag.ToString();
                }

            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("ToolStripFavoritesMenuClickHandler\r\n" + ee);
            }
        }
        private void ToolStripViewMenuClickHandler(object sender, EventArgs e)
        {
            try
            {
                if ((sender == tsViewMnuLogs) && (!FormHelper.FrmLog.Visible))
                    FormHelper.FrmLog.Show(this);
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("ToolStripViewMenuClickHandler\r\n" + ee);
            }

        }
        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if ((sender == statisticsToolStripMenuItem) && (!FormHelper.frmStatistic.Visible))
                {
                    FormHelper.frmStatistic.BindData(lPages);
                    FormHelper.frmStatistic.Show(this);
                }
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("statisticsToolStripMenuItemClickHandler\r\n" + ee);
            }

        }
        private void TsWBTabsToolStripButtonCtxMenuHandler(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    //Do not show for the first btn,
                    var ptC = new Point(Cursor.Position.X, Cursor.Position.Y);
                    Point pt = tsWBTabs.PointToClient(ptC);
                    m_tsBtnctxMnu = (ToolStripButton)tsWBTabs.GetItemAt(pt);
                    if ((m_tsBtnctxMnu != null) && (m_tsBtnctxMnu.Name != m_tsBtnFirstTab.Name))
                    {
                        tsMnuCloasAllWBs.Enabled = (IdxCountWB > 1) ? true : false;
                        ctxMnuCloseWB.Show(tsWBTabs.PointToScreen(pt));
                    }
                }
                catch (Exception ee)
                {
                    FormHelper.FrmLog.AppendToLog("TabContextMenuHandler\r\n" + ee);
                }
            }
        }


        private void ToolStripHelpMenuClickHandler(object sender, EventArgs e)
        {
            try
            {
                if (sender == tsHelpMnuHelpAbout)
                {
                    var about = new frmAbout();
                    about.ShowDialog(this);
                    about.Dispose();
                }
                else if (sender == tsHelpMnuHelpContents)
                {
                    string directory = Path.GetDirectoryName(Application.ExecutablePath);
                    NavToUrl(directory+ @"\Resources\Help.html");
                }
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("ToolStripHelpMenuClickHandler\r\n" + ee);
            }
        }

        private void ToolStripToolsMenuClickHandler(object sender, EventArgs e)
        {
            if (sender == tsToolsMnuDocumentInfo)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    m_frmDocInfo.LoadDocumentInfo(_CurWB);
                    if (!m_frmDocInfo.Visible)
                        m_frmDocInfo.Show(this);
                    else
                        m_frmDocInfo.BringToFront();
                    Cursor = Cursors.Default;
                }
                catch (Exception eeee)
                {
                    Cursor = Cursors.Default;
                    FormHelper.FrmLog.AppendToLog("tsToolsMnuDocumentInfo\r\n" + eeee);
                }
                return;
            }

            #region Cache Cookie History

            bool bshowform = true;
            int iCount = 0; //Number of cookies or cache entries deleted
            try
            {
                if (sender == tsToolsMnuClearHistory)
                {
                    if (!FormHelper.AskForConfirmation(Properties.Resources.RemoveAllHistory, this))
                        return;
                    _CurWB.ClearHistory();
                }
                else if (sender == tsToolsMnuCookieViewAll)
                {
                    Cursor = Cursors.WaitCursor;
                    iCount = m_frmCacheCookie.LoadListViewItems(FormHelper.Cookie);
                    Cursor = Cursors.Default;
                }
                else if (sender == tsToolsMnuCookieViewCurrentSite)
                {
                    string url = _CurWB.LocationUrl;
                    if (url.Length > 0)
                    {
                        Cursor = Cursors.WaitCursor;
                        iCount = m_frmCacheCookie.LoadListViewItems(
                            FormHelper.SetupCookieCachePattern(_CurWB.LocationUrl, FormHelper.Cookie));
                        Cursor = Cursors.Default;
                    }
                }
                else if (sender == tsToolsMnuCacheViewAll)
                {
                    Cursor = Cursors.WaitCursor;
                    iCount = m_frmCacheCookie.LoadListViewItems(FormHelper.Visited);
                    Cursor = Cursors.Default;
                }
                else if (sender == tsToolsMnuCacheViewCurrentSite)
                {
                    string url = _CurWB.LocationUrl;
                    if (url.Length > 0)
                    {
                        //Visited:.*\.example\.com
                        Cursor = Cursors.WaitCursor;
                        iCount = m_frmCacheCookie.LoadListViewItems(
                            FormHelper.SetupCookieCachePattern(_CurWB.LocationUrl, FormHelper.Visited));
                        Cursor = Cursors.Default;
                    }
                }
                else if (sender == tsToolsMnuCookieEmptyAll)
                {
                    if (!FormHelper.AskForConfirmation(Properties.Resources.RemoveAllCookies, this))
                        return;
                    Cursor = Cursors.WaitCursor;
                    iCount = m_frmCacheCookie.ClearAllCookies(string.Empty);
                    bshowform = false;
                    Cursor = Cursors.Default;
                    MessageBox.Show(this, string.Format(Properties.Resources.DeletedCookies, iCount),
                        Properties.Resources.Information, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (sender == tsToolsMnuCookieEmptyCurrentSite)
                {
                    if (!FormHelper.AskForConfirmation(string.Format(Properties.Resources.RemoveCookiesFromPath, _CurWB.LocationUrl), this))
                        return;
                    Cursor = Cursors.WaitCursor;
                    iCount = m_frmCacheCookie.ClearAllCookies(_CurWB.LocationUrl);
                    bshowform = false;
                    Cursor = Cursors.Default;
                    MessageBox.Show(this, string.Format(Properties.Resources.DeletedCookiesFromPath, iCount, _CurWB.LocationUrl),
                        Properties.Resources.Information, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (sender == tsToolsMnuCacheEmptyAll)
                {
                    if (!FormHelper.AskForConfirmation(Properties.Resources.RemoveAllCacheEntries, this))
                        return;
                    Cursor = Cursors.WaitCursor;
                    iCount = m_frmCacheCookie.ClearAllCache(string.Empty);
                    bshowform = false;
                    Cursor = Cursors.Default;
                    MessageBox.Show(this, string.Format(Properties.Resources.DeletedCacheEntries, iCount),
                        Properties.Resources.Information, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (sender == tsToolsMnuCacheEmptyCurrentSite)
                {
                    if (!FormHelper.AskForConfirmation(string.Format(Properties.Resources.RemoveCacheEntriesFromPath, _CurWB.LocationUrl), this))
                        return;
                    Cursor = Cursors.WaitCursor;
                    iCount = m_frmCacheCookie.ClearAllCache(_CurWB.LocationUrl);
                    bshowform = false;
                    Cursor = Cursors.Default;
                    MessageBox.Show(this, string.Format(Properties.Resources.DeletedCacheEntriesFromPath, iCount, _CurWB.LocationUrl),
                        Properties.Resources.Information, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (bshowform)
                {
                    if (iCount > 0)
                    {
                        if (!m_frmCacheCookie.Visible)
                            m_frmCacheCookie.Show(this);
                    }
                    else
                        MessageBox.Show(this, Properties.Resources.NoItemsFound, Properties.Resources.Information, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ee)
            {
                Cursor = Cursors.Default;
                FormHelper.FrmLog.AppendToLog("ToolStripToolsMenuClickHandler\r\n" + ee);
            }

            #endregion

        }

        /// <summary>
        /// File menu items click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripFileMenuClickHandler(object sender, EventArgs e)
        {
            try
            {
                if (sender == tsFileMnuBackgroundBlankPage)
                {
                    AddNewBrowser(Blank, AboutBlank, string.Empty, false);
                }
                else if (sender == tsFileMnuBackgroundFromAddress)
                {
                    AddNewBrowser(Blank, AboutBlank, comboURL.Text, false);
                }
                else if (sender == tsFileMnuForegroundBlankPage)
                {
                    AddNewBrowser(Blank, AboutBlank, string.Empty, true);
                }
                else if (sender == tsFileMnuForegroundFromAddress)
                {
                    AddNewBrowser(Blank, AboutBlank, comboURL.Text, true);
                }
                else if (sender == tsFileMnuPrint)
                {
                    _CurWB.Print();
                }
                else if (sender == tsFileMnuPrintPreview)
                {
                    _CurWB.PrintPreview();
                }
                else if (sender == tsFileMnuSaveDocument)
                {
                    _CurWB.SaveAs();
                }
                else if (sender == tsFileMnuSaveDocumentImage)
                {
                    ////gif format produces some of the smallest sizes
                    if (FormHelper.ShowStaticSaveDialogForImage(this) == DialogResult.OK)
                    {
                        System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Bmp;
                        string ext = ".bmp";
                        switch (FormHelper.DlgSave.FilterIndex)
                        {
                            case 1:
                                break;
                            case 2:
                                format = System.Drawing.Imaging.ImageFormat.Gif;
                                ext = ".gif";
                                break;
                            case 3:
                                format = System.Drawing.Imaging.ImageFormat.Jpeg;
                                ext = ".jpeg";
                                break;
                            case 4:
                                format = System.Drawing.Imaging.ImageFormat.Png;
                                ext = ".png";
                                break;
                            case 5:
                                format = System.Drawing.Imaging.ImageFormat.Wmf;
                                ext = ".wmf";
                                break;
                            case 6:
                                format = System.Drawing.Imaging.ImageFormat.Tiff;
                                ext = ".tiff";
                                break;
                            case 7:
                                format = System.Drawing.Imaging.ImageFormat.Emf;
                                ext = ".emf";
                                break;
                            default:
                                break;
                        }
                        if (string.IsNullOrEmpty(Path.GetExtension(FormHelper.DlgSave.FileName)))
                            FormHelper.DlgSave.FileName += ext;

                        _CurWB.SaveBrowserImage(FormHelper.DlgSave.FileName,System.Drawing.Imaging.PixelFormat.Format24bppRgb, format);
                    }
                }
                else if (sender == tsFileMnuOpen)
                {
                    if (FormHelper.ShowStaticOpenDialog(this, FormHelper.DlgHtmlsFilter,
                        string.Empty, "C:", true) == DialogResult.OK)
                        _CurWB.Navigate(FormHelper.DlgOpen.FileName);
                }
                else if (sender == tsFileMnuExit)
                {
                    Application.Exit();
                }
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("File Menu\r\n" + ee);
            }
        }

        /// <summary>
        /// Edit menu items click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripEditMenuClickHandler(object sender, EventArgs e)
        {
            try
            {
                if (sender == tsEditMnuSelectAll)
                    _CurWB.SelectAll();
                else if (sender == tsEditMnuCopy)
                    _CurWB.Copy();
                else if (sender == tsEditMnuCut)
                    _CurWB.Cut();
                else if (sender == tsEditMnuPaste)
                    _CurWB.Paste();
                else if (sender == tsEditMnuFindInPage)
                {
                    m_frmFind.Show(this);
                }
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("ToolStripEditMenuClickHandler\r\n" + ee);
            }
        }
        void ToolStripLinksButtonClickHandler(object sender, EventArgs e)
        {
            try
            {
                var item = (ToolStripItem)sender;
                if (item.Tag != null)
                {
                    NavToUrl(item.Tag.ToString());
                }

            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("ToolStripLinksButtonClickHandler\r\n" + ee);
            }
        }

    }
}
