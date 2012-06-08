using System.Xml;
using TestRecorder.Core.Formatters;
using System.Drawing;

namespace TestRecorder.Core.Actions
{
    public class ActionBack : ActionBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        public override string Name
        {
            get
            {
                return "Back";
            }
        }

        public override Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("back.bmp");
        }

        public override string Description
        {
            get
            {
                return "Navigate Backward";
            }
        }
        public ActionBack(ActionContext context):base(context)
        {
        }
        public override bool Perform()
        {
            Context.ActivePage.Browser.Back();
            return true;
        }

        public override CodeLine ToCode(ICodeFormatter Formatter)
        {
            var line = new CodeLine() { NoModel = true };
            string pageFriendly = "";
            if (Context.ActivePage != null) pageFriendly = Context.ActivePage.FriendlyName;
            line.FullLine = Formatter.VariableDeclarator + pageFriendly + Formatter.MethodSeparator + "Back()" + Formatter.LineEnding;
            return line;
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "WindowBack");
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            writer.WriteEndElement();
        }
    }
}
