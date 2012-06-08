using System;
using System.Drawing;
using System.Text;
using System.Xml;
using TestRecorder.Core.Formatters;
using TestRecorder.UserControls;
using WatiN.Core;
using WatiN.Core.Native.InternetExplorer;
using System.Collections.Generic;
using IfacesEnumsStructsClasses;

namespace TestRecorder.Core.Actions
{
    public class ActionCheckbox : ActionElementBase
    {
        public override string Name { get { return "Checkbox"; } }
        public bool Checked { get; set; }
        
        public ActionCheckbox(ActionContext context) : base(context) 
        {
            ElementType = ElementTypes.CheckBox;
        }
        public override IUcBaseEditor GetEditor()
        {
            return null;
        }

        public override Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("Checkbox.bmp");
        }

        public override bool Perform()
        {
            bool result;
            try
            {
                var element = (CheckBox)GetTheElement();
                if (element != null)
                {
                    element.Checked = Checked;
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
                var element = (CheckBox)GetTheElement();
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

        public override void LoadFromXml( XmlNode node)
        {
            base.LoadFromXml( node);
            Checked = node.Attributes["Checked"].Value == "1";
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "CheckBox");
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            writer.WriteAttributeString("Checked", Checked ? "1" : "0");
            SaveSettings(writer);
            writer.WriteEndElement();
        }
    }
}
