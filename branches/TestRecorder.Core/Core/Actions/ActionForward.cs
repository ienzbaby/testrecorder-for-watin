using System.Xml;
using TestRecorder.Core.Formatters;
using System.Drawing;

namespace TestRecorder.Core.Actions
{
    public class ActionForward : ActionBase
    {
        public override string Name
        {
            get { return "Forward"; }
        }

        public override Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("forward.bmp");
        }

        public override string Description
        {
            get
            {
                return "Navigate Forward";
            }
        }
        public ActionForward(ActionContext context):base(context)
        {
        }
        public override bool Perform()
        {
            Context.ActivePage.Browser.Forward();
            return true;
        }

        public override CodeLine ToCode(ICodeFormatter Formatter)
        {
            var line = new CodeLine() {NoModel = true};
            line.FullLine = Formatter.VariableDeclarator + Context.ActivePage.FriendlyName + Formatter.MethodSeparator + "Forward()" + Formatter.LineEnding;
            return line;
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "WindowForward");
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            writer.WriteEndElement();
        }
    }
}
