using System;
using System.Drawing;
using System.Text;
using System.Xml;
using TestRecorder.Core.Formatters;
using TestRecorder.UserControls;
using WatiN.Core;
using System.Collections.Generic;
using WatiN.Core.DialogHandlers;
using System.Threading;

namespace TestRecorder.Core.Actions
{
    public class ActionClick : ActionElementBase
    {
        public override string Name { get { return "Click"; } }
        public bool NoWait { get; set; }
        public ActionClick(ActionContext context) : base(context) { }

        public override IUcBaseEditor GetEditor()
        {
            return new ucClick();
        }

        public override Bitmap GetIcon()
        {
            // change image based on target
            return Helper.GetIconFromFile("Click.bmp");
        }

        public override string Description
        {
            get
            {
                return "Click " + this.GetElemDesc();
            }
        }
        public override bool Perform()
        {
            bool result = true;
            try
            {
                Element element = GetTheElement();
                if (element.Exists)
                {
                    element.ClickNoWait();
                    Context.ActivePage.Browser.WaitForComplete(3000);
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
        public override bool Validate()
        {
            bool result;
            try
            {
                Element element = GetTheElement();
                result = element.Exists;
                Status = StatusIndicators.Validated;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                result = false;
                Status = StatusIndicators.Faulted;
            }

            return result;
        }

        public override CodeLine ToCode(ICodeFormatter Formatter)
        {
            var line = new CodeLine();
            line.Attributes = Context.FindMechanism;
            var builder = new StringBuilder();
            //line.Frames = Context.FrameList;
            line.ModelPath = GetDocumentPath(Formatter);
            builder.Append(line.ModelPath);
            builder.Append(Formatter.MethodSeparator);
            builder.Append(ElementType);
            builder.Append("(" + Context.FindMechanism.ToString() + ")");
            line.ModelLocalProperty = builder.ToString();
            builder.Append(Formatter.MethodSeparator);

            line.ModelFunction = NoWait ? "ClickNoWait()" : "Click()";
            line.ModelFunction += Formatter.LineEnding;
            builder.Append(line.ModelFunction);
            
            line.FullLine = builder.ToString();
            return line;
        }

        public override void LoadFromXml( XmlNode node)
        {
            base.LoadFromXml( node);
            if (node.Attributes.GetNamedItem("NoWait") != null)
            {
                NoWait = node.Attributes.GetNamedItem("NoWait").ToString() == "1";
            }
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "BrowserClick");
            writer.WriteAttributeString("NoWait", NoWait?"1":"0");
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            SaveSettings(writer);
            writer.WriteEndElement();
        }
    }
}
