using System;
using System.Drawing;
using System.Text;
using System.Xml;
using TestRecorder.Core.Formatters;
using TestRecorder.UserControls;
using WatiN.Core;
using WatiN.Core.Native.InternetExplorer;
using System.Collections.Generic;

namespace TestRecorder.Core.Actions
{
    public class ActionRadio : ActionElementBase
    {
        //private IfacesEnumsStructsClasses.IHTMLElement ActiveElement { get; set; }
        public override string Name { get { return "Radio Button"; } }
        public bool Checked { get; set; }

        public ActionRadio(ActionContext context):base(context)
        {
            ElementType = ElementTypes.RadioButton;
        }

        public override IUcBaseEditor GetEditor()
        {
            return null;
        }
        public override Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("RadioButton.bmp");
        }

        public override bool Perform()
        {
            bool result;
            try
            {
                var element = (RadioButton)GetTheElement();
                if (element != null)
                {
                    //IfacesEnumsStructsClasses.IHTMLElement activeElement = (IfacesEnumsStructsClasses.IHTMLElement)((IEElement)element.NativeElement).AsHtmlElement;
                    element.Checked = Checked;
                    element.ClickNoWait();
                }
                else
                {
                    throw new SystemException("Not Find Element!");
                }
                result = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                result = false;
            }

            return result;
        }

        public override string Description
        {
            get
            {
                return (Checked ? "Check " : "Uncheck ") + this.GetElemDesc();
            }
        }
        public override bool Validate()
        {
            bool result;

            try
            {
                var element = (RadioButton)GetTheElement();
                result = element.Checked == Checked;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                result = false;
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
            line.ModelFunction = "Checked = " + Checked.ToString().ToLower() + Formatter.LineEnding;
            builder.Append(line.ModelFunction);
            line.FullLine = builder.ToString();
            return line;
        }

        public override void LoadFromXml(XmlNode node)
        {
            base.LoadFromXml(node);
            Checked = node.Attributes["Checked"].Value == "1";
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "RadioButton");
            writer.WriteAttributeString("Checked", Checked ? "1" : "0");
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            SaveSettings(writer);
            writer.WriteEndElement();
        }
    }
}
