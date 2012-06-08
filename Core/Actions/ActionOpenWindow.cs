using System;
using System.Xml;
using TestRecorder.Core.Formatters;
using System.Drawing;
using System.Threading;

namespace TestRecorder.Core.Actions
{
    public class ActionOpenWindow : ActionBase
    {
        public override string Name
        {
            get { return "Open Window"; }
        }
        public string BrowserURL = "";
        public string BrowserTitle = "";

        public ActionOpenWindow(ActionContext context):base(context){}
        /// <summary>
        /// retrieve the icon for the action
        /// </summary>
        /// <returns>icon as a bitmap</returns>
        public override Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("window.bmp");
        }

        public override bool Perform()
        {
            return true;
        }

        public override string Description
        {
            get
            {
                string description = "Open Window " + BrowserURL;
                //if (BrowserURL.Length>0) description += " Url=" + BrowserURL;
                return description;
            }
        }

        public override CodeLine ToCode(ICodeFormatter Formatter)
        {
            var line = new CodeLine() {NoModel = true};
            line.FullLine = Formatter.ClassNameFormat(typeof(WatiN.Core.IE), Context.ActivePage.Browser.FriendlyName);
            return line;
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "WindowOpen");
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            writer.WriteEndElement();
        }
        
    }
}
