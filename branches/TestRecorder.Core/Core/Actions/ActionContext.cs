using System.Collections.Generic;
using WatiN.Core;
using System;

namespace TestRecorder.Core.Actions
{
    public class ActionContext 
    {
        public List<FindMethods> FindPattern = new List<FindMethods>();
        public FindAttributeCollection FindMechanism = new FindAttributeCollection();
        public LoadContext PageContext =LoadContext.Instance();
        public string FrameUrl = "";

        internal WebPage ActivePage;
        /// <summary>
        /// 取得框架
        /// </summary>
        /// <returns></returns>
        public Document GetFrame()
        {
            Document objFrame = null;
            foreach (var frame in ActivePage.Browser.Frames)
            {
                if (frame.Url == FrameUrl)
                {
                    objFrame = frame;
                }
            }
            return objFrame;
        }
        /// <summary>
        /// 取得所在文档
        /// </summary>
        /// <returns></returns>
        public Document GetDocument()
        {
            return this.GetFrame() ?? this.ActivePage.Browser;
        }
        private ActionContext(BrowserWindow browser)
        {
            this.ActivePage = new WebPage(browser);
        }
        /// <summary>
        /// 构造函数2
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="findPattern"></param>
        public ActionContext(BrowserWindow browser, List<FindMethods> findPattern)
            : this(browser)
        {
            this.FindPattern=findPattern;
        }
        /// <summary>
        /// 转成对象
        /// </summary>
        /// <param name="ActionObjectString"></param>
        /// <returns></returns>
        public ActionBase StringToObject(string ActionObjectString)
        {
            return Helper.StringToObject(this, ActionObjectString);
        }
    }
}