using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using TestRecorder.Core.Actions;

namespace TestRecorder.Core
{
    public class CSharpCodeFormatter : ICodeFormatter
    {
        #region ICodeFormatter Members

        public bool FileDestination { get; set; }

        public string LineEnding
        {
            get
            {
                return ";";
            }
        }

        public string StartupApplication { get; set; }


        public string ScriptExtension
        {
            get
            {
                return ".cs";
            }
        }

        public string CommentMarker
        {
            get
            {
                return "//";
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
                return "new";
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
            return classType+" " + classVariable + " = new " + classType+"();";
        }

        public string ElementVariable(ElementTypes elementType, string elementVariable, string elementValue)
        {
            return elementType + " " + elementVariable + " = " + elementValue + LineEnding;
        }

        public string InitialBrowser(string browserName, BrowserTypes browserType)
        {
            switch (browserType)
            {
                case BrowserTypes.FireFox: return ClassNameFormat(typeof(WatiN.Core.FireFox), browserName);
                //case BrowserTypes.Chrome: return ClassNameFormat(typeof(WatiN.Core.Chrome), browserName);
                default: return ClassNameFormat(typeof(WatiN.Core.IE), browserName);
            }
        }

        public string DisposeBrowser(string browserName)
        {
            return browserName + ".Dispose();";
        }

        public string GetProperty(ActionElementBase element, DataRow row, string propertyName)
        {
            var builder = new StringBuilder();
            //[FindBy(Id = "userName")]
            builder.AppendLine("["+element.Context.FindMechanism.ToAttribute(row)+"]");
            builder.AppendLine("public abstract " + element.ElementType + " " + propertyName + " { get; }");
            return builder.ToString();
        }

        public bool DeclaredLogonHandler { get; set; }
        public bool DeclaredAlertHandler { get; set; }
        public bool DeclaredConfirmHandler { get; set; }

        #endregion
    }
}
