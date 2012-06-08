using System;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using TestRecorder.Core.Formatters;
using TestRecorder.UserControls;
using WatiN.Core;
using System.Collections.Generic;
using System.Threading;

namespace TestRecorder.Core.Actions
{
    public class ActionTypeText : ActionElementBase
    {
        public override string Name { get { return "Type Text"; } }
        public bool ValueOnly;
        public bool Overwrite;
        public string TextToType { get; set; }

        public ActionTypeText(ActionContext context):base(context) {}

        //public ActionTypeText(ScriptManager caller) : base(caller) {}

        public override IUcBaseEditor GetEditor()
        {
            return new ucTypeText();
        }

        public override System.Drawing.Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("TypeText.bmp");
        }

        public override string Description
        {
            get
            {
                if (Overwrite) 
                    return "Type Text \"" + TextToType + "\" into " + this.GetElemDesc();
                else 
                    return "Append Text \"" + TextToType + "\" into " + this.GetElemDesc();
            }
        }

        public override bool Perform()
        {
            bool result;
            try
            {
                var constraint = GetAllConstraint();
                TextField field = Context.ActivePage.Browser.TextField(constraint);
                if (Overwrite)
                {
                    field.TypeText(TextToType);
                }
                else if (ValueOnly)
                {
                    field.Value = TextToType;
                }
                else
                {
                    field.AppendText(TextToType);
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

        public override bool Validate()
        {
            bool result;
            try
            {
                string text = ((TextField) GetTheElement()).Value;
                result = text == TextToType;
                if (!result) ErrorMessage = "Text does not match";
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
            if (ValueOnly) line.ModelFunction = "Value = \"" + TextToType + "\"" + Formatter.LineEnding;
            if (Overwrite) line.ModelFunction = "TypeText(\"" +  TextToType + "\")" + Formatter.LineEnding;
            else line.ModelFunction = "AppendText(\"" + TextToType + "\")" + Formatter.LineEnding;
            builder.Append(line.ModelFunction);
            line.FullLine = builder.ToString();
            return line;
        }

        public override void LoadFromXml( XmlNode node)
        {
            base.LoadFromXml( node);
            TextToType = node.Attributes["TextToType"].Value;
            if (node.Attributes["ValueOnly"]!=null) ValueOnly = node.Attributes["ValueOnly"].Value=="1";
            if (node.Attributes["Overwrite"] != null) Overwrite = node.Attributes["Overwrite"].Value == "1";
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "TypeText");
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            writer.WriteAttributeString("TextToType", TextToType);
            writer.WriteAttributeString("ValueOnly", ValueOnly?"1":"0");
            writer.WriteAttributeString("Overwrite", Overwrite ? "1" : "0");
            SaveSettings(writer);
            writer.WriteEndElement();
        }
    }
}
