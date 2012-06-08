using System;
using System.Text;
using System.Xml;
using TestRecorder.Core.Formatters;
using TestRecorder.UserControls;
using WatiN.Core;
using System.Collections.Generic;

namespace TestRecorder.Core.Actions
{
    public class ActionKey : ActionElementBase
    {
        public override string Name { get { return "Key"; } }

        public enum KeyFunctions
        {
            Up, Down, Press
        }

        public KeyFunctions KeyFunction { get; set; }
        public char KeyToPress { get; set; }

        public ActionKey(ActionContext context) : base(context) { }

        public override IUcBaseEditor GetEditor()
        {
            return new ucKey();
        }

        public override System.Drawing.Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("KeyPress.bmp");
        }

        public override string Description
        {
            get
            {
                return "Key " + KeyFunction;
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
                    if (KeyFunction == KeyFunctions.Up) element.KeyUp(KeyToPress);
                    else if (KeyFunction == KeyFunctions.Down) element.KeyDown(KeyToPress);
                    else element.KeyPress(KeyToPress);
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

            if (KeyFunction == KeyFunctions.Up) line.ModelFunction = "KeyUp(Convert.ToChar(\""+KeyToPress+"\"))";
            else if (KeyFunction == KeyFunctions.Down) line.ModelFunction = "KeyDown(Convert.ToChar(\"" + KeyToPress + "\"))";
            else line.ModelFunction = "KeyPress(Convert.ToChar(\"" + KeyToPress + "\"))";
            line.ModelFunction += Formatter.LineEnding;

            builder.Append(line.ModelFunction);
            line.FullLine = builder.ToString();
            return line;
        }

        public override void LoadFromXml( XmlNode node)
        {
            base.LoadFromXml( node);
            KeyFunction = (KeyFunctions) Enum.Parse(typeof(KeyFunctions), node.Attributes["KeyFunction"].Value);
            KeyToPress = Convert.ToChar(node.Attributes["KeyToPress"].Value);
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "Key");
            writer.WriteAttributeString("KeyFunction", KeyFunction.ToString());
            writer.WriteAttributeString("KeyToPress", KeyToPress.ToString());
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            SaveSettings(writer);
            writer.WriteEndElement();
        }
    }
}
