using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using TestRecorder.Core.Formatters;
using TestRecorder.UserControls;
using WatiN.Core;
using System.Collections.Generic;

namespace TestRecorder.Core.Actions
{
    public class ActionWait : ActionElementBase
    {
        public override string Name { get { return "Wait"; } }

        public enum WaitTypes
        {
            Exists=0, Removed=1, AttributeValue=2
        }
        public WaitTypes WaitType { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public bool AttributeRegex { get; set; }
        public int WaitTimeout { get; set; }

        public ActionWait(ActionContext context):base(context) { }

        //public ActionWait(ScriptManager caller) : base(caller) { }

        public override IUcBaseEditor GetEditor()
        {
            return new ucWait();
        }

        public override Bitmap GetIcon()
        {
            // change image based on target
            return Helper.GetIconFromFile("Wait.bmp");
        }

        public override string Description
        {
            get
            {
                // Exists, Removed, or Until Attribute = Value
                return "Wait " + this.GetElemDesc();
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
                    if (WaitType == WaitTypes.Exists) element.WaitUntilExists(WaitTimeout);
                    else if (WaitType == WaitTypes.Removed) element.WaitUntilRemoved(WaitTimeout);
                    else
                    {
                        if (AttributeRegex) element.WaitUntil(AttributeName, new Regex(AttributeValue), WaitTimeout);
                        else element.WaitUntil(AttributeName, AttributeValue, WaitTimeout);
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                Status = StatusIndicators.Faulted;
                ErrorMessage = ex.Message;
                result = false;
            }

            return result;
        }

        public override bool Validate()
        {
            return true;
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

            if (WaitType == WaitTypes.Exists) line.ModelFunction = "WaitUntilExists("+WaitTimeout+")";
            else if (WaitType == WaitTypes.Removed) line.ModelFunction = "WaitUntilRemoved(" + WaitTimeout + ")";
            else
            {
                if (AttributeRegex) line.ModelFunction = "WaitUntil("+AttributeName+", new Regex("+AttributeValue+"), "+WaitTimeout+")";
                else line.ModelFunction = "WaitUntil(" + AttributeName + ", " + AttributeValue + ", " + WaitTimeout + ")";
            }
            line.ModelFunction += Formatter.LineEnding;
            builder.Append(line.ModelFunction);

            line.FullLine = builder.ToString();
            return line;
        }

        public override void LoadFromXml( XmlNode node)
        {
            base.LoadFromXml( node);
            AttributeName = node.Attributes.GetNamedItem("AttributeName").ToString();
            AttributeValue = node.Attributes.GetNamedItem("AttributeValue").ToString();
            AttributeRegex = node.Attributes.GetNamedItem("AttributeRegex").ToString() == "1";
            WaitTimeout = Convert.ToInt32(node.Attributes.GetNamedItem("WaitTimeout"));
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "Wait");
            writer.WriteAttributeString("AttributeName", AttributeName);
            writer.WriteAttributeString("AttributeValue", AttributeValue);
            writer.WriteAttributeString("AttributeRegex", AttributeRegex?"1":"0");
            writer.WriteAttributeString("WaitTimeout", WaitTimeout.ToString());
            SaveSettings(writer);
            writer.WriteEndElement();
        }
    }
}
