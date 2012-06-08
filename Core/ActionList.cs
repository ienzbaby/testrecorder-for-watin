using System.Collections.Generic;
using System.Data;
using System.Xml;
using TestRecorder.Core.Actions;

namespace TestRecorder.Core
{
    /// <summary>
    /// 动作列表
    /// </summary>
    public class ActionList : List<ActionBase>
    {
        public string TestName = "";
        //public DataTable ReplacementTable = new DataTable();

        public void LoadDataFromXml(XmlNode parentNode)
        {
            XmlNodeList list = parentNode.SelectNodes("Row");
            if (list == null) return;
            foreach (XmlNode node in list)
            {
                var sarrData = new string[node.ChildNodes.Count];
                for (int i = 0; i < sarrData.Length; i++)
                {
                    sarrData[i] = node.ChildNodes[i].InnerText;
                }
            }
        }

        private void SaveDataToXml(XmlWriter writer)
        {
            writer.WriteStartElement("ReplacementData");
            writer.WriteEndElement();
        }
        public string GetCode(ICodeFormatter formatter, BrowserTypes browserType)
        {
            var generator = new CodeGenerator
            {
                Formatter = formatter,
                Browser = browserType,
                ActionList = this
            };
            return generator.GetCode();
        }

        public void SaveXml(XmlTextWriter writer)
        {
            writer.WriteStartElement("Test");
            writer.WriteAttributeString("Name","test");
            foreach (ActionBase action in this)
            {
                action.SaveToXml(writer);
            }
            writer.WriteEndElement();
        }
      
    }
}
