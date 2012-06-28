using System;
using System.Windows.Forms;
using IfacesEnumsStructsClasses;

namespace TestRecorder
{
    public partial class frmCacheCookie : Form
    {
        public frmCacheCookie()
        {
            InitializeComponent();
        }

        private string m_CurOp = string.Empty;
        private void AdjustThisText()
        {
            Text = string.Format(Properties.Resources.CacheCookieFound,lsvCacheCookie.Items.Count,m_CurOp);
        }

        public int ClearAllCookies(string FromSite)
        {
            int ideleted = 0;
            System.Collections.ArrayList results = WinApis.FindUrlCacheEntries(
                FormHelper.SetupCookieCachePattern(FromSite, FormHelper.Cookie));
            foreach (INTERNET_CACHE_ENTRY_INFO entry in results)
            {
                //Must have a SourceUrlName and LocalFileName
                if (
                    (!string.IsNullOrEmpty(entry.lpszSourceUrlName)) &&
                    (entry.lpszSourceUrlName.Trim().Length > 0) &&
                    (!string.IsNullOrEmpty(entry.lpszLocalFileName)) &&
                    (entry.lpszLocalFileName.Trim().Length > 0)
                    )
                {
                    WinApis.DeleteUrlCacheEntry(entry.lpszSourceUrlName);
                    ideleted++;
                }
            }
            return ideleted;
        }

        public int ClearAllCache(string FromSite)
        {
            int ideleted = 0;

            System.Collections.ArrayList results = WinApis.FindUrlCacheEntries(
                FormHelper.SetupCookieCachePattern(FromSite, FormHelper.Visited));
            foreach (INTERNET_CACHE_ENTRY_INFO entry in results)
            {
                //Avoid Null or empty entries
                if ((!string.IsNullOrEmpty(entry.lpszSourceUrlName)) &&
                    (entry.lpszSourceUrlName.Trim().Length > 0))
                {
                    WinApis.DeleteUrlCacheEntry(entry.lpszSourceUrlName);
                    ideleted++;
                }
            }

            return ideleted;
        }
        
        public int LoadListViewItems(string pattern)
        {
            m_CurOp = pattern.StartsWith(FormHelper.Cookie) ? " cookies" : " cache entries";
            //Reset
            lsvCacheCookie.Items.Clear();
            Int64 size = 0;
            int index = 0;
            System.Collections.ArrayList results = WinApis.FindUrlCacheEntries(pattern);
            foreach (INTERNET_CACHE_ENTRY_INFO entry in results)
            {
                if ( (!string.IsNullOrEmpty(entry.lpszSourceUrlName)) &&
                    (entry.lpszSourceUrlName.Trim().Length > 0) )
                {
                    var item = new ListViewItem();
                    lsvCacheCookie.Items.Add(item);

                    lsvCacheCookie.Items[index].SubItems[0].Text = entry.lpszSourceUrlName;
                    if( (!string.IsNullOrEmpty(entry.lpszLocalFileName)) &&
                        (entry.lpszLocalFileName.Trim().Length > 0) )
                        lsvCacheCookie.Items[index].SubItems.Add(entry.lpszLocalFileName);
                    else
                        lsvCacheCookie.Items[index].SubItems.Add(string.Empty);
                    lsvCacheCookie.Items[index].SubItems.Add(WinApis.ToStringFromFileTime(entry.LastModifiedTime));
                    lsvCacheCookie.Items[index].SubItems.Add(WinApis.ToStringFromFileTime(entry.LastAccessTime));
                    lsvCacheCookie.Items[index].SubItems.Add(WinApis.ToStringFromFileTime(entry.ExpireTime));
                    try
                    {
                        size = (((Int64)entry.dwSizeHigh) << 32) + entry.dwSizeLow;
                    }
                    catch (Exception)
                    {
                        size = 0;
                    }
                    finally
                    {
                        lsvCacheCookie.Items[index].SubItems.Add(size.ToString());
                    }
                }
                index++;
            }
            AdjustThisText();
            return index;
        }

        private void frmCacheCookie_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                lsvCacheCookie.Items.Clear();
            }
        }

        private void frmCacheCookie_Load(object sender, EventArgs e)
        {
            //Add some icons
            Icon = FormHelper.BitmapToIcon(5);
            toolStripButtonDelSelected.Image = FormHelper.ImgListMain.Images[29];
            toolStripButtonDelChecked.Image = FormHelper.ImgListMain.Images[39];
            toolStripButtonDelAll.Image = FormHelper.ImgListMain.Images[40];

            //To demonstrate of handling of a listviewitem dragdrop on a csEXWB control
            lsvCacheCookie.ItemDrag += lsvCacheCookie_ItemDrag;
        }

        void lsvCacheCookie_ItemDrag(object sender, ItemDragEventArgs e)
        {
            lsvCacheCookie.DoDragDrop(e.Item, DragDropEffects.All);
        }

        private void toolStripButtonDelSelected_Click(object sender, EventArgs e)
        {
            try
            {
                ListView.SelectedListViewItemCollection selitems = lsvCacheCookie.SelectedItems;
                if (selitems.Count == 0)
                    return;
                if (!FormHelper.AskForConfirmation(string.Format(Properties.Resources.DeleteSelectedItems,selitems.Count), this))
                    return;
                foreach (ListViewItem item in selitems)
                {
                    WinApis.DeleteUrlCacheEntry(item.SubItems[0].Text);
                    lsvCacheCookie.Items.Remove(item);
                }
                AdjustThisText();
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("toolStripButtonDelSelected_Click:\r\n" + ee);
            }
        }

        private void toolStripButtonDelChecked_Click(object sender, EventArgs e)
        {
            try
            {
                ListView.CheckedListViewItemCollection checkeditems = lsvCacheCookie.CheckedItems;
                if (checkeditems.Count == 0)
                    return;
                if (!FormHelper.AskForConfirmation(string.Format(Properties.Resources.DeleteCheckedItems,checkeditems.Count), this))
                    return;
                foreach (ListViewItem item in checkeditems)
                {
                    WinApis.DeleteUrlCacheEntry(item.SubItems[0].Text);
                    lsvCacheCookie.Items.Remove(item);
                }
                AdjustThisText();
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("toolStripButtonDelChecked_Click:\r\n" + ee);
            }
        }

        private void toolStripButtonDelAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (lsvCacheCookie.Items.Count == 0)
                    return;
                if (!FormHelper.AskForConfirmation(string.Format(Properties.Resources.DeleteAllItems,lsvCacheCookie.Items.Count), this))
                    return;
                foreach (ListViewItem item in lsvCacheCookie.Items)
                {
                    WinApis.DeleteUrlCacheEntry(item.SubItems[0].Text);
                }
                lsvCacheCookie.Items.Clear();
                AdjustThisText();
            }
            catch (Exception ee)
            {
                FormHelper.FrmLog.AppendToLog("toolStripButtonDelAll_Click:\r\n" + ee);
            }
        }
    }
}