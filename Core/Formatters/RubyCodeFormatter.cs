using System;
using System.Data;
using System.Text;
using TestRecorder.Core.Actions;

namespace TestRecorder.Core
{
    public class RubyCodeFormatter : ICodeFormatter
    {
        #region ICodeFormatter Members

        public bool FileDestination { get; set; }

        public string LineEnding
        {
            get
            {
                return "";
            }
        }

        public string StartupApplication { get; set; }


        public string ScriptExtension
        {
            get
            {
                return ".rb";
            }
        }

        public string CommentMarker
        {
            get
            {
                return "#";
            }
        }

        public string VariableDeclarator
        {
            get
            {
                return "";
            }
        }

        public string NewDeclaration
        {
            get
            {
                return "";
            }
        }

        public string MethodSeparator
        {
            get
            {
                return ".";
            }
        }

        public string ClassNameFormat(Type classType, string classVariable)
        {
            return classVariable + " = " + classType + "()";
        }

        public string ElementVariable(ElementTypes elementType, string elementVariable, string elementValue)
        {
            return elementVariable + " = " + elementValue;
        }

        public string InitialBrowser(string browserName, BrowserTypes browserType)
        {
            var builder = new StringBuilder();
            builder.AppendLine("require 'win32ole'");
            builder.AppendLine("Interface = WIN32OLE.new('WatiN.COMInterface')");

            switch (browserType)
            {
                default: builder.AppendLine(browserName + " = Interface.CreateIE()"); break;
                case BrowserTypes.FireFox: builder.AppendLine(browserName + " = Interface.CreateFireFox()"); break;
                case BrowserTypes.Chrome: builder.AppendLine(browserName + " = Interface.CreateChrome()"); break;
            }

            return builder.ToString();
        }

        public string DisposeBrowser(string browserName)
        {
            return "";
        }

        public string GetProperty(ActionElementBase element, DataRow row, string propertyName)
        {
            return "";
        }

        public bool DeclaredLogonHandler { get; set; }
        public bool DeclaredAlertHandler { get; set; }
        public bool DeclaredConfirmHandler { get; set; }

        #endregion
    }
}
