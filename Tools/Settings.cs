using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace TestRecorder.Tools
{
    /// <summary>
    /// ≈‰÷√¿‡
    /// </summary>
    public class Settings
    {
        private readonly XmlDocument xmlDocument = new XmlDocument();
        public string documentPath = Application.StartupPath + "\\settings.xml";
        
        public Settings()
        {
            try
            {
                xmlDocument.Load(documentPath);
            }
            catch
            {
                xmlDocument.LoadXml("<settings></settings>");
            }
        }

        public Settings(string path)
        {
            try
            {
                documentPath = path;
                xmlDocument.Load(documentPath);
            }
            catch
            {
                xmlDocument.LoadXml("<settings></settings>");
            }
        }

        public int GetSetting(string xPath, int defaultValue)
        { return Convert.ToInt16(GetSetting(xPath, Convert.ToString(defaultValue))); }

        public string GetSetting(string xPath, string defaultValue)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
            if (xmlNode != null) { return xmlNode.InnerText; }
            return defaultValue;
        }

        public void PutSetting(string xPath, int value)
        { PutSetting(xPath, Convert.ToString(value)); }

        public void PutSetting(string xPath, string value)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath) ??
                              createMissingNode("settings/" + xPath);
            xmlNode.InnerText = value;
            xmlDocument.Save(documentPath);
        }

        private XmlNode createMissingNode(string xPath)
        {
            string[] xPathSections = xPath.Split('/');
            string currentXPath = "";
            XmlNode currentNode = xmlDocument.SelectSingleNode("settings");
            foreach (string xPathSection in xPathSections)
            {
                currentXPath += xPathSection;
                XmlNode testNode = xmlDocument.SelectSingleNode(currentXPath);
                if (testNode == null)
                {
                    currentNode.InnerXml += "<" +
                                xPathSection + "></" +
                                xPathSection + ">";
                }
                currentNode = xmlDocument.SelectSingleNode(currentXPath);
                currentXPath += "/";
            }
            return currentNode;
        }


        public static string AppDirectory
        {
            get
            {
                string directory = Path.GetDirectoryName(Application.ExecutablePath);
                if (directory.Contains("TestDriven"))
                {
                    directory = @"C:\Development\TestRecorder\bin\Debug\";
                }
                return directory;
            }
        }

        public static string IconDirectory
        {
            get
            {
                string directory = Path.GetDirectoryName(Application.ExecutablePath);
                directory = Path.Combine(directory, "Icons");
                if (directory.Contains("TestDriven"))
                {
                    directory = @"C:\Development\TestRecorder\bin\Debug\Icons\";
                }
                return directory;
            }
        }
    }
}
