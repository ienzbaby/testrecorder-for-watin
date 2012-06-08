using System.Data;
using System.Xml;
using TestRecorder.Core.Formatters;
using System.Threading;

namespace TestRecorder.Core.Actions
{
    public class ActionOpenPopup : ActionOpenWindow
    {
        public override string Name
        {
            get { return "Open Popup"; }
        }

        //public frmPopup PopupForm { get; set; }

        public ActionOpenPopup(ActionContext context):base(context) {}

        public override bool Perform()
        {
            return true;
        }

        public override CodeLine ToCode(ICodeFormatter Formatter)
        {
            var line = new CodeLine() {NoModel = true};
            //if (PopupForm!=null && PopupForm.Watinie!=null)
            //{
            //    line.FullLine = Formatter.ClassNameFormat(typeof(WatiN.Core.IE), PopupForm.Watinie.FriendlyName);
            //}
            //else
            //{
            //    line.FullLine = Formatter.ClassNameFormat(typeof (WatiN.Core.IE), "popup_nameerr");
            //}
            return line;
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "WindowOpenPopup");
            writer.WriteAttributeString("PageHash", Context.ActivePage!=null?Context.ActivePage.HashCode.ToString():"-1");
            writer.WriteEndElement();
        }
    }
}
