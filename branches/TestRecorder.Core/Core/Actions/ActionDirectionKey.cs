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
    public class ActionDirectionKey : ActionBase
    {
        public string DirectionKey { get; set;}
        public override string Name
        {
            get { return "Direction Key "; }
        }

        public ActionDirectionKey(ActionContext context, string direction):base(context)
        {
            DirectionKey = direction;
        }

        public override IUcBaseEditor GetEditor()
        {
            return new ucBaseElement();
        }

        public override Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("TypeText.bmp");
        }

        public override bool Perform()
        {
            System.Windows.Forms.SendKeys.SendWait(DirectionKey);
            return true;
        }

        public override string Description
        {
            get
            {
                return "Press " + DirectionKey;
            }
        }

        public override bool Validate()
        {
            return true;
        }

        public override CodeLine ToCode(ICodeFormatter Formatter)
        {
            var line = new CodeLine();
            line.NoModel = true;
            line.FullLine = "System.Windows.Forms.SendKeys.SendWait(\""+DirectionKey+"\")"+Formatter.LineEnding;
            return line;
        }

        public override void LoadFromXml( XmlNode node)
        {
            base.LoadFromXml( node);
            DirectionKey = node.Attributes["DirectionKey"].Value;
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "DirectionKey");
            writer.WriteAttributeString("DirectionKey", DirectionKey);
            if (Context.ActivePage == null) writer.WriteAttributeString("PageHash", "0");
            else writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            writer.WriteEndElement();
        }
    }
}
