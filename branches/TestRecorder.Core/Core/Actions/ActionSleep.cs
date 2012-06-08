using System;
using System.Xml;
using TestRecorder.Core.Formatters;
using TestRecorder.UserControls;
using System.Collections.Generic;
using System.Threading;

namespace TestRecorder.Core.Actions
{
    public class ActionSleep : ActionBase
    {
        public override string Name { get { return "Sleep"; } }

        /// <summary>
        /// 暂停时间
        /// </summary>
        public int SleepTime { get; set; }

        public override System.Drawing.Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("sleep.bmp");
        }
        /// <summary>
        /// 取得编辑器
        /// </summary>
        /// <returns></returns>
        public override IUcBaseEditor GetEditor()
        {
            return new ucSleep();
        }
        /// <summary>
        /// 描述
        /// </summary>
        public override string Description
        {
            get
            {
                return "Sleep for " + SleepTime + " milliseconds.";
            }
        }
        public ActionSleep(ActionContext context):base(context)
        {
        }
        /// <summary>
        /// 运行改动作，暂停一定时间
        /// </summary>
        /// <returns></returns>
        public override bool Perform()
        {
            Thread.Sleep(SleepTime);
            return true;
        }

        public override CodeLine ToCode(ICodeFormatter Formatter)
        {
            var line = new CodeLine { NoModel = true };

            // not sure how to sleep in other languages
            if (Formatter.GetType() == typeof(CSharpCodeFormatter) || Formatter.GetType() == typeof(VBNetCodeFormatter))
            {
                line.FullLine = "System.Threading.Thread.Sleep(" + SleepTime.ToString() + ")" + Formatter.LineEnding;
            }
            else
            {
                line.FullLine = Formatter.CommentMarker + " Unknown sleep command";
            }
            return line;
        }
        /// <summary>
        /// 保存到Xml文件
        /// </summary>
        /// <param name="writer"></param>
        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "Sleep");
            writer.WriteAttributeString("SleepTime", SleepTime.ToString());
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            writer.WriteEndElement();
        }
        /// <summary>
        /// 从Xml文件加载
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="node"></param>
        public override void LoadFromXml( XmlNode node)
        {
            base.LoadFromXml(node);
            SleepTime = Convert.ToInt32(node.Attributes["SleepTime"].Value);
        }
    }
}
