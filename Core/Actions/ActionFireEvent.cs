using System;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using TestRecorder.Core.Formatters;
using TestRecorder.UserControls;
using WatiN.Core;
using System.Collections.Generic;

namespace TestRecorder.Core.Actions
{
    public class ActionFireEvent : ActionElementBase
    {
        public override string Name
        {
            get { return "Fire Event"; }
        }
        public string EventName { get; set; }
        public bool NoWait { get; set; }

        public NameValueCollection EventParameters = new NameValueCollection();

        public ActionFireEvent(ActionContext context):base(context){}

        public override IUcBaseEditor GetEditor()
        {
            return new ucFireEvent();
        }

        public override System.Drawing.Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("FireEvent.bmp");
        }

        public override string Description
        {
            get
            {
                return "Fire Event " + EventName;
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
                    NameValueCollection parameters = null;
                    if(EventParameters.Count >0) parameters= EventParameters;

                    if (NoWait)
                    {
                        element.FireEventNoWait(EventName,parameters);
                    }
                    else
                    {
                        element.FireEvent(EventName, parameters);
                    }
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
            line.NoModel = true;
            var builder = new StringBuilder();

            if (EventParameters != null)
            {
                builder.AppendLine("using (var nvc = new NameValueCollection())");
                builder.AppendLine("{");
                foreach (var key in EventParameters.AllKeys)
                {
                    builder.AppendLine("nvc" + Formatter.MethodSeparator + "Add(\"" + key + "\",\"" + EventParameters[key] + "\")" + Formatter.LineEnding);
                }
            }
            //line.Frames = Context.FrameList;
            line.ModelPath = GetDocumentPath(Formatter);
            builder.Append(line.ModelPath);
            builder.Append(Formatter.MethodSeparator);
            builder.Append(ElementType);
            builder.Append("(" + Context.FindMechanism.ToString() + ")");
            builder.Append(Formatter.MethodSeparator);

            string localevent = EventName;

            if (EventParameters != null)
            {
                if (NoWait) builder.Append("FireEventNoWait(\"" + localevent + "\", nvc)");
                else builder.Append("FireEvent(\"" + localevent + "\", nvc)");
            }
            else
            {
                if (NoWait) builder.Append("FireEventNoWait(\"" + localevent + "\")");
                else builder.Append("FireEvent(\"" + localevent + "\")");
            }

            builder.Append(Formatter.LineEnding);
            builder.Append(Environment.NewLine);

            if (EventParameters != null)
            {
                builder.AppendLine("}");
            }
            line.FullLine = builder.ToString();
            return line;
        }

        public override void LoadFromXml( XmlNode node)
        {
            base.LoadFromXml( node);
            NoWait = node.Attributes.GetNamedItem("NoWait").ToString() == "1";
            EventName = node.Attributes.GetNamedItem("EventName").ToString();
            XmlNodeList list = node.SelectNodes("EventParameter");
            if (list != null)
            {
                foreach (XmlNode child in list)
                {
                    EventParameters.Add(child.Attributes["Key"].Value, child.Attributes["Value"].Value);
                }
            }
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "FireEvent");
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            writer.WriteAttributeString("EventName", EventName);
            writer.WriteAttributeString("NoWait", NoWait ? "1" : "0");

            for (int i = 0; i < EventParameters.Count; i++)
            {
                string key = EventParameters.AllKeys[i];
                writer.WriteStartElement("EventParameter");
                writer.WriteAttributeString("Key",key);
                writer.WriteAttributeString("Value", EventParameters[key]);
                writer.WriteEndElement();
            }

            SaveSettings(writer);
            writer.WriteEndElement();
        }
    }
}
