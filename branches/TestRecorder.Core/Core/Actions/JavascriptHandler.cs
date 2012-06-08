using System;
using System.Collections.Generic;
using System.Text;
using TestRecorder.Core.Formatters;
using System.Xml;
using WatiN.Core.DialogHandlers;
using TestRecorder.UserControls;

namespace TestRecorder.Core.Actions
{
    public class JavascriptHandler : ActionBase
    {
        public override string Name { get { return "Javascript Handler"; } }
        private string _script = "";
        /// <summary>
        /// 待执行的脚步
        /// </summary>
        public string Script
        {
            get
            {
                return _script;
            }
            set
            {
                _script=value;
            }
        }
        public JavascriptHandler(ActionContext context):base(context)
        {
            //this.Context.ActivePage = browser.DeterminePage();
        }

      
        public override System.Drawing.Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("JavascriptHanler.bmp");
        }

        public override IUcBaseEditor GetEditor()
        {

            return new ucJavascript();
        }
        public override string Description
        {
            get
            {
                return "Javascript["+this.Script+"]";
            }
        }

        public override bool Perform()
        {
            Context.ActivePage.Browser.RunScript(this.Script);
            return true;
        }

        public override CodeLine ToCode(ICodeFormatter Formatter)
        {
            var builder = new StringBuilder();
            if (Formatter.DeclaredAlertHandler)
            {
                builder.AppendLine("adhdl = " + Formatter.NewDeclaration + " JavascriptHandler()" + Formatter.LineEnding);
            }
            else
            {
                builder.AppendLine(Formatter.ClassNameFormat(typeof(AlertDialogHandler), "adhdl") + "  = " + Formatter.NewDeclaration + " AlertDialogHandler()" + Formatter.LineEnding);
            }

            if (Context.ActivePage != null)
                builder.AppendLine(Context.ActivePage.FriendlyName + ".AddDialogHandler(adhdl)" + Formatter.LineEnding);
            else builder.AppendLine("window.AddDialogHandler(adhdl)" + Formatter.LineEnding);
            builder.AppendLine("adhdl.OKButton.Click()" + Formatter.LineEnding);
            var line = new CodeLine { NoModel = true, FullLine = builder.ToString() };
            return line;
        }
        public override void LoadFromXml( XmlNode node)
        {
            base.LoadFromXml( node);
            this.Script = node.InnerText;
        }
        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "JavascriptHandler");
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            writer.WriteCData(this.Script);
            writer.WriteEndElement();
        }
    }
}
