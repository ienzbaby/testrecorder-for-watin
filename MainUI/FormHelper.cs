using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace TestRecorder
{
    /// <summary>
    /// Simple class to encapsulate global forms, controls, consts, and methods
    /// </summary>
    public sealed class FormHelper
    {
        public const string Cookie = "Cookie:";
        public const string Visited = "Visited:";
        public const string DlgHtmlsFilter = "Html files (*.html)|*.html|Htm files (*.htm)|*.htm|Text files (*.txt)|*.txt|All files (*.*)|*.*";
        public const string DlgXmlsFilter = "XML files (*.xml)|*.xml";
        public const string DlgTextfilesFilter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
        public const string DlgImagesFilter = "Bmp files (*.bmp)|*.bmp" +
                        "|Gif files (*.gif)|*.gif" +
                        "|Jpeg files (*.Jpeg)|*.Jpeg" +
                        "|Png files (*.png)|*.png" +
                        "|Wmf files (*.wmf)|*.wmf" +
                        "|Tiff files (*.tiff)|*.tiff" +
                        "|Emf files (*.emf)|*.emf";

        public static frmLog FrmLog = new frmLog();
        public static frmDynamicConfirm FrmDynamicConfirm = new frmDynamicConfirm();
        public static ImageList ImgListMain = new ImageList();
        public static SaveFileDialog DlgSave = new SaveFileDialog();
        public static OpenFileDialog DlgOpen = new OpenFileDialog();
        public static frmStatistics frmStatistic = new frmStatistics();

        public static Icon BitmapToIcon(Image orgImage)
        {
            Icon icoRet = null;
            if (orgImage == null)
                return icoRet;
            var bmp = new Bitmap(orgImage);
            icoRet = Icon.FromHandle(bmp.GetHicon());
            bmp.Dispose();
            return icoRet;
        }

        //Using ImgListMain static member
        public static Icon BitmapToIcon(int orgImage)
        {
            Icon icoRet = null;
            if (ImgListMain.Images.Count > 0)
            {
                var bmp = new Bitmap(ImgListMain.Images[orgImage]);
                icoRet = Icon.FromHandle(bmp.GetHicon());
                bmp.Dispose();
            }
            return icoRet;
        }

        //replace = visited: or cookie:
        public static string SetupCookieCachePattern(string pattern, string replace)
        {
            const string cookiecachepattern = ".*";
            const string dot = ".";
            const string backslashdot = "\\.";

            string url = pattern;
            if (url.Length > 0)
            {
                var curUri = new Uri(url);
                url = curUri.Host;
                //Replace "." with "\\."
                url = url.Replace(dot, backslashdot);
                url = replace + cookiecachepattern + url;

                //www.google.com
                //visited:.*www\\.google\\.com

                //login.live.com
                //cookie:.*login\\.live\\.com
            }
            return url;
        }

        /// <summary>
        /// A little shortcut when asking for yes or no type of confirmation
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="win"></param>
        /// <returns></returns>
        public static bool AskForConfirmation(string msg, IWin32Window win)
        {
            DialogResult result = MessageBox.Show(win, msg, Properties.Resources.ConfirmationRequested, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return (result == DialogResult.Yes) ? true : false;
        }

        /// <summary>
        /// To display a save dialog from any form within this project
        /// </summary>
        /// <param name="win"></param>
        /// <param name="defaultext">If empty, "txt" is used.</param>
        /// <param name="filename">Name or Name.ext</param>
        /// <param name="filter">If empty, Textual filter is used.</param>
        /// <param name="title">if empty, "Save File" is used.</param>
        /// <param name="initialdir">If empty, current directory is used.</param>
        /// <returns></returns>
        public static DialogResult ShowStaticSaveDialog(IWin32Window win,
            string defaultext, string filename,
            string filter, string title, string initialdir)
        {
            DlgSave.DefaultExt = string.IsNullOrEmpty(defaultext) ? "txt" : defaultext;
            DlgSave.FileName = string.IsNullOrEmpty(filename) ? "FileName" : filename;
            DlgSave.Filter = string.IsNullOrEmpty(filter) ? DlgTextfilesFilter : filter;
            DlgSave.Title = string.IsNullOrEmpty(title) ? "Save File" : title;
            DlgSave.InitialDirectory = string.IsNullOrEmpty(initialdir) ? Environment.CurrentDirectory : initialdir;
            return DlgSave.ShowDialog(win);
        }

        public static DialogResult ShowStaticSaveDialogForText(IWin32Window win)
        {
            return ShowStaticSaveDialog(win, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public static DialogResult ShowStaticSaveDialogForImage(IWin32Window win)
        {
            return ShowStaticSaveDialog(win, "bmp", "ImageFileName", DlgImagesFilter, "Save Image", string.Empty);
        }

        public static DialogResult ShowStaticSaveDialogForXML(IWin32Window win)
        {
            return ShowStaticSaveDialog(win, "xml", "XMLFileName", DlgXmlsFilter, "Save XML", string.Empty);
        }

        public static DialogResult ShowStaticOpenDialog(IWin32Window win,
            string filter, string title, string initialdir, bool showreadonly)
        {
            DlgOpen.Filter = filter;
            DlgOpen.Title = string.IsNullOrEmpty(title) ? "Open File" : title;
            DlgOpen.InitialDirectory = string.IsNullOrEmpty(initialdir) ? Environment.CurrentDirectory : initialdir;
            DlgOpen.ShowReadOnly = showreadonly;
            return DlgOpen.ShowDialog(win);
        }

        public static void LoadWebColors(ComboBox combo)
        {
            Array knownColors = Enum.GetValues(typeof(KnownColor));
            //First add an empty color
            combo.Items.Add(Color.Empty);
            //Then the rest
            foreach (KnownColor k in knownColors)
            {
                Color c = Color.FromKnownColor(k);

                if (!c.IsSystemColor && (c.A > 0))
                {
                    combo.Items.Add(c);
                }
            }
            //Select default
            combo.SelectedIndex = 0;
        }

    }

}