using System.Xml;
using TestRecorder.Core.Formatters;
using System.Drawing;

namespace TestRecorder.Core.Actions
{
    public class ActionRefresh : ActionBase
    {
        public override string Name { get { return "Refresh"; } }

        public override Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("refresh.bmp");
        }

        public override string Description
        {
            get
            {
                return "Refresh Page";
            }
        }
        public ActionRefresh(ActionContext context):base(context)
        {
        }
        public override bool Perform()
        {
            Context.ActivePage.Browser.Refresh();
            return true;
        }

        public override CodeLine ToCode(ICodeFormatter Formatter)
        {
            var line = new CodeLine() {NoModel = true};
            line.FullLine = Formatter.VariableDeclarator + Context.ActivePage.FriendlyName + Formatter.MethodSeparator + "Refresh()" + Formatter.LineEnding;
            return line;
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "WindowRefresh");
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            writer.WriteEndElement();
        }

    }
}
