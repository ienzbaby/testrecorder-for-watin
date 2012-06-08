using System;
using System.Xml;
using TestRecorder.Core.Formatters;
using System.Drawing;
using System.Threading;

namespace TestRecorder.Core.Actions
{
    public class ActionCloseWindow : ActionBase
    {
        //public string BrowserURL = "";
        //public string BrowserTitle = "";

        public override string Name
        {
            get { return "Close Window"; }
        }

        public ActionCloseWindow(ActionContext context):base(context)
        {
        }
        public override Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("CloseWindow.bmp");
        }

        public override bool Perform()
        {
            Thread.Sleep(1000);
            return true;
        }
        /// <summary>
        /// 描述
        /// </summary>
        public override string Description
        {
            get
            {
                string description = "Close Window ";
                //if (BrowserURL.Length > 0) description += " Url=" + BrowserURL;
                return description;
            }
        }

        public override CodeLine ToCode(ICodeFormatter Formatter)
        {
            var line = new CodeLine() {NoModel = true};
            line.FullLine = Context.ActivePage.FriendlyName+Formatter.MethodSeparator+"Close()"+Formatter.LineEnding;
            return line;
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "CloseWindow");
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            writer.WriteEndElement();
        }
    }
}
