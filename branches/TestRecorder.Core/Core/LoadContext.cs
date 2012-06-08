using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TestRecorder.Core.Actions;
using IfacesEnumsStructsClasses;
using System.Globalization;

namespace TestRecorder.Core
{
    /// <summary>
    /// 加载上下文
    /// </summary>
    public class LoadContext
    {
        internal List<WebPage> PageList = new List<WebPage>();
        private static LoadContext context = new LoadContext();

        private LoadContext()
        {
        }
        public static LoadContext Instance()
        {
            return context;
        }
        /// <summary>
        /// 保存到xml文件
        /// </summary>
        /// <param name="writer"></param>
        public void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("PageList");
            foreach (WebPage page in context.PageList)
            {
                page.SaveToXml(writer);
            }
            writer.WriteEndElement();
        }
       
      
        /// <summary>
        /// locates a web page by the hash number
        /// </summary>
        /// <param name="Hash">hash to find</param>
        /// <returns>web page object</returns>
        internal WebPage FindByHash(int Hash)
        {
            foreach (WebPage page in context.PageList)
            {
                if (page.HashCode == Hash)
                {
                    return page;
                }
            }
            return null;
        }
        /// <summary>
        /// 加载页面
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="Browser"></param>
        public LoadContext LoadFromXML(XmlNode ParentNode, BrowserWindow Browser)
        {
            XmlNodeList pageNodes = ParentNode.SelectNodes("WebPage");
            foreach (XmlNode node in pageNodes)
            {
                var page = new WebPage(Browser);
                page.LoadFromXML(node);
                context.PageList.Add(page);
            }
            //给父页面赋值
            foreach (WebPage page in this.PageList)
            {
                if (page.ParentPageHash != 0)
                {
                    page.ParentPage =this.FindByHash(page.HashCode);
                }
            }
            return context;
        }

        internal WebPage FindByURL(string URL)
        {
            foreach (WebPage page in this.PageList)
            {
                if (page.URL == URL)
                {
                    return page;
                }
            }
            return null;
        }
    }
}
