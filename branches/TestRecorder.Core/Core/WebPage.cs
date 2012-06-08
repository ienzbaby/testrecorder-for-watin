using System;
using System.Xml;

namespace TestRecorder.Core
{
    /// <summary>
    /// Class for information about each web page
    /// </summary>
    class WebPage
    {
        /// <summary>
        /// User-readable name of the web page
        /// </summary>
        internal string FriendlyName = "page";

        /// <summary>
        /// parent for frames and IFrames
        /// </summary>
        internal WebPage ParentPage;

        /// <summary>
        /// hash code for the parent page, used when loading
        /// </summary>
        internal int ParentPageHash = 0;

        /// <summary>
        /// Url of the page
        /// </summary>
        internal string URL = "";

        /// <summary>
        /// Browser containing this page
        /// </summary>
        internal BrowserWindow Browser;

        /// <summary>
        /// Full text of the web page
        /// </summary>
        internal string Content = "";

        /// <summary>
        /// hash code of the page content
        /// </summary>
        internal int HashCode
        {
            get
            {
                if (_hashcode == -1)
                {
                    try
                    {
                        _hashcode = Content.GetHashCode();
                    }
                    catch (Exception)
                    {
                        _hashcode = -1;
                    }

                }
                return _hashcode;
            }
        }
        private int _hashcode = -1;

        /// <summary>
        /// date this page was loaded (last)
        /// </summary>
        public DateTime LoadDate = DateTime.MinValue;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="browser"></param>
        public WebPage(BrowserWindow browser)
        {
            Browser = browser;
            if (browser != null) Content = browser.Html;
            LoadDate = DateTime.Now;
            if (browser != null) URL = browser.Url;
            FriendlyName = AutoPageName();
        }
        private string AutoPageName()
        {
            if (string.IsNullOrEmpty(URL)) URL = "unknownUrl";
            var uriPage = new Uri("http://www.unk.com");
            try
            {
                uriPage = new Uri(URL);
            }
            catch (Exception)
            {
                //
            }
            string strPath = uriPage.LocalPath;
            strPath = System.Text.RegularExpressions.Regex.Replace(strPath, @"/|\.|-", "");
            if (System.Text.RegularExpressions.Regex.IsMatch(strPath, @"\A\d+"))
            {
                strPath = "N" + strPath;
            }

            // if local path is a bust, try using the hostname
            if (strPath == "")
            {
                strPath = uriPage.Host;
                strPath = System.Text.RegularExpressions.Regex.Replace(strPath, @"/|\.|-", "");
                if (System.Text.RegularExpressions.Regex.IsMatch(strPath, @"\A\d+"))
                {
                    strPath = "N" + strPath;
                }
            }
            return strPath;
        }
        /// <summary>
        /// 保存到Xml文件
        /// </summary>
        /// <param name="writer"></param>
        internal void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("WebPage");
            writer.WriteElementString("FriendlyName", FriendlyName);
            writer.WriteElementString("URL", URL);
            writer.WriteElementString("HashCode", HashCode.ToString());
            writer.WriteElementString("LoadDate", LoadDate.ToFileTime().ToString());
            writer.WriteElementString("ParentPageHash", ParentPageHash.ToString());
            writer.WriteEndElement();
        }
        /// <summary>
        /// 从Xml文件中加载
        /// </summary>
        /// <param name="node"></param>
        internal void LoadFromXML(XmlNode node)
        {
            FriendlyName = node.SelectSingleNode("FriendlyName").InnerText;
            URL = node.SelectSingleNode("URL").InnerText;
            _hashcode = Convert.ToInt32(node.SelectSingleNode("HashCode").InnerText);
            LoadDate = DateTime.FromFileTime(Convert.ToInt64(node.SelectSingleNode("LoadDate").InnerText));
            ParentPageHash = Convert.ToInt32(node.SelectSingleNode("ParentPageHash").InnerText);
        }
    }
}
