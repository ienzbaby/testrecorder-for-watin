using System.Text;
using System.Xml;
using TestRecorder.Core.Formatters;
using WatiN.Core.DialogHandlers;
using System.Windows.Forms;
using System.Collections.Generic;

namespace TestRecorder.Core.Actions
{
    public class ActionConfirmHandler : ActionBase
    {
        public override string Name { get { return "Alert Handler"; } }
        public bool Result = false;

        public ActionConfirmHandler(ActionContext context)
            : base(context)
        {
        }
        public override System.Drawing.Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("ConfirmHandler.bmp");
        }

        public override string Description
        {
            get
            {
                return "Handle Confirm Dialog, Result="+Result;
            }
        }

        public override bool Perform()
        {
            return true;
        }
        public override CodeLine ToCode(ICodeFormatter Formatter)
        {
            var builder = new StringBuilder();
            if (Formatter.DeclaredConfirmHandler)
            {
                builder.AppendLine("adhdl = " + Formatter.NewDeclaration + " ConfirmDialogHandler()" + Formatter.LineEnding);
            }
            else
            {
                builder.AppendLine(Formatter.ClassNameFormat(typeof(ConfirmDialogHandler), "adhdl") + "  = " + Formatter.NewDeclaration + " ConfirmDialogHandler()" + Formatter.LineEnding);
            }

            if (Context.ActivePage != null)
                builder.AppendLine(Context.ActivePage.FriendlyName + ".AddDialogHandler(adhdl)" + Formatter.LineEnding);
            else builder.AppendLine("window.AddDialogHandler(adhdl)" + Formatter.LineEnding);
            builder.AppendLine("adhdl.OKButton.Click()" + Formatter.LineEnding);
            var line = new CodeLine { NoModel = true, FullLine = builder.ToString() };
            return line;
        }
        public override void LoadFromXml(XmlNode node)
        {
            base.LoadFromXml(node);
            this.Result = (node.Attributes["Result"].Value=="true");
        }
        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "ConfirmHandler");
            writer.WriteAttributeString("Result", this.Result.ToString().ToLower());
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            writer.WriteEndElement();
        }
    }
}
