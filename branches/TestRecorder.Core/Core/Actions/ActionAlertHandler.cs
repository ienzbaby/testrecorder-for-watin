using System.Text;
using System.Xml;
using TestRecorder.Core.Formatters;
using WatiN.Core.DialogHandlers;

namespace TestRecorder.Core.Actions
{
    public class ActionAlertHandler : ActionBase
    {
        public override string Name { get { return "Alert Handler"; } }
        public ActionAlertHandler(ActionContext context):base(context)
        {
        }
        public override System.Drawing.Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("AlertHandler.bmp");
        }

        public override string Description
        {
            get
            {
                return "Handle Alert Dialog";
            }
        }
        public override bool Perform()
        {
            return true;
        }

        public override CodeLine ToCode(ICodeFormatter Formatter)
        {
            var builder = new StringBuilder();
            if (Formatter.DeclaredAlertHandler)
            {
                builder.AppendLine("adhdl = " + Formatter.NewDeclaration + " AlertDialogHandler()" + Formatter.LineEnding);
            }
            else
            {
                builder.AppendLine(Formatter.ClassNameFormat(typeof(AlertDialogHandler), "adhdl") + "  = " + Formatter.NewDeclaration + " AlertDialogHandler()" + Formatter.LineEnding);
            }

            if (Context.ActivePage != null)
                builder.AppendLine(Context.ActivePage.FriendlyName + ".AddDialogHandler(adhdl)" + Formatter.LineEnding);
            else builder.AppendLine("window.AddDialogHandler(adhdl)" + Formatter.LineEnding);
            builder.AppendLine("adhdl.OKButton.Click()" + Formatter.LineEnding);
            var line = new CodeLine {NoModel = true, FullLine = builder.ToString()};
            return line;
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "AlertHandler");
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            writer.WriteEndElement();
        }
    }
}
