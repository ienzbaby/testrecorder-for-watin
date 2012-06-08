using System;
using System.Text;
using System.Xml;
using TestRecorder.Core.Formatters;
using TestRecorder.UserControls;
using WatiN.Core;
using System.Collections.Generic;

namespace TestRecorder.Core.Actions
{
    public class ActionMouse : ActionElementBase
    {
        public override string Name { get { return "Mouse"; } }

        public enum MouseFunctions
        {
            Up, Down, Enter
        }

        public MouseFunctions MouseFunction { get; set; }
        public ActionMouse(ActionContext context) : base(context) { }

        public override IUcBaseEditor GetEditor()
        {
            return new ucMouse();
        }

        public override System.Drawing.Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("mouse.bmp");
        }

        public override string Description
        {
            get
            {
                return "Mouse " + MouseFunction;
            }
        }

        public override bool Perform()
        {
            bool result;
            try
            {
                Element element = GetTheElement();

                if (element != null)
                {
                    if (MouseFunction == MouseFunctions.Up) element.MouseUp();
                    else if (MouseFunction == MouseFunctions.Down) element.MouseDown();
                    else element.MouseEnter();
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

            if (MouseFunction == MouseFunctions.Up) line.ModelFunction = "MouseUp()";
            else if (MouseFunction == MouseFunctions.Down) line.ModelFunction = "MouseDown()";
            else line.ModelFunction = "MouseEnter()";
            line.ModelFunction += Formatter.LineEnding;
            builder.Append(line.ModelFunction);
            line.FullLine = builder.ToString();
            return line;
        }

        public override void LoadFromXml( XmlNode node)
        {
            base.LoadFromXml( node);
            MouseFunction = (MouseFunctions)Enum.Parse(typeof(MouseFunctions), node.Attributes["MouseFunction"].Value);
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "Mouse");
            writer.WriteAttributeString("MouseFunction", MouseFunction.ToString());
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            SaveSettings(writer);
            writer.WriteEndElement();
        }
    }
}
