using System;
using System.Text;
using System.Xml;
using TestRecorder.Core.Formatters;
using WatiN.Core;

namespace TestRecorder.Core.Actions
{
    public class ActionDoubleClick : ActionElementBase
    {
        //public ActionDoubleClick() {}

        public ActionDoubleClick(ActionContext context) : base(context) { }

        public override string Name { get { return "DoubleClick"; } }

        public override System.Drawing.Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("doubleclick.bmp");
        }

        public override string Description
        {
            get
            {
                return "DoubleClick";
            }
        }

        public override bool Perform()
        {
            bool result;
            try
            {
                Element element = GetTheElement();
                if (element != null) element.DoubleClick();
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
                Element element = GetTheElement();
                result = element.Exists;
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
            var builder = new StringBuilder();
            //line.Frames = Context.FrameList;
            line.ModelPath = GetDocumentPath(Formatter);
            builder.Append(line.ModelPath);
            builder.Append(Formatter.MethodSeparator);
            builder.Append(ElementType);
            builder.Append("(" + Context.FindMechanism.ToString() + ")");
            line.ModelLocalProperty = builder.ToString();
            builder.Append(Formatter.MethodSeparator);
            line.ModelFunction = "DoubleClick()" + Formatter.LineEnding;
            builder.Append(line.ModelFunction);
            line.FullLine = builder.ToString();
            return line;
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "BrowserDoubleClick");
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            SaveSettings(writer);
            writer.WriteEndElement();
        }
    }
}
