using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TestRecorder.Core.Formatters;
using System.Drawing;
using WatiN.Core;

namespace TestRecorder.Core.Actions
{
    public class ActionHighlight:ActionElementBase
    {
         /// <summary>
        /// 名称
        /// </summary>
        public override string Name
        {
            get
            {
                return "Hightlight";
            }
        }

        public override Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("Highlight.bmp");
        }

        public override string Description
        {
            get
            {
                return "Element Hightlight " + this.GetElemDesc(); ;
            }
        }
        public ActionHighlight(ActionContext context):base(context)
        {
        }
        public override bool Perform()
        {

            bool result = true;
            try
            {
                Element element = GetTheElement();
                if (element.Exists)
                {
                    element.Highlight(true);
                    Context.ActivePage.Browser.WaitForComplete(1000);
                    element.Highlight(false);
                }
                else
                {
                    throw new SystemException("Not Find Element!");
                }
            }
            catch (Exception ex)
            {
                Status = StatusIndicators.Faulted;
                ErrorMessage = "[" + this.GetElemDesc() + "]," + ex.Message + ":\n" + ex.StackTrace;
                result = false;
            }
            return result;
        }

        public override CodeLine ToCode(ICodeFormatter Formatter)
        {
            var line = new CodeLine() { NoModel = true };
            string pageFriendly = "";
            if (Context.ActivePage != null) pageFriendly = Context.ActivePage.FriendlyName;
            line.FullLine = Formatter.VariableDeclarator + pageFriendly + Formatter.MethodSeparator + "Hightlight()" + Formatter.LineEnding;
            return line;
        }
        public override void LoadFromXml( XmlNode node)
        {
            base.LoadFromXml( node);
        }
        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "Hightlight");
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            SaveSettings(writer);
            writer.WriteEndElement();
        }
    }
}
