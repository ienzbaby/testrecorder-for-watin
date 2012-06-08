using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TestRecorder.Core.Actions;
using TestRecorder.Core.Formatters;

namespace TestRecorder.Core
{
    public class VBNetCodeFormatter : ICodeFormatter
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
                return ".vb";
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

        public string ClassNameFormat(Type ClassType, string ClassVariable)
        {
            return "Dim " + ClassVariable + " as " + ClassType + " = new " + ClassType+"()";
        }

        public string ElementVariable(ElementTypes elementType, string ElementVariable, string ElementValue)
        {
            return "Dim " + ElementVariable + " as " + elementType + " = " + ElementValue + LineEnding;
        }

        public string InitialBrowser(string BrowserName, BrowserTypes browserType)
        {
            switch (browserType)
            {
                default: return ClassNameFormat(typeof(WatiN.Core.IE), BrowserName);
                case BrowserTypes.FireFox: return ClassNameFormat(typeof(WatiN.Core.FireFox), BrowserName);
                //case BrowserTypes.Chrome: return ClassNameFormat(typeof(WatiN.Core.Chrome), BrowserName);
            }
        }

        public string DisposeBrowser(string BrowserName)
        {
            return BrowserName + ".Dispose()";
        }

        public string GetProperty(ActionElementBase element, DataRow row, string propertyName)
        {
            var builder = new StringBuilder();
            //[FindBy(Id = "userName")]
            builder.AppendLine("[" + element.Context.FindMechanism.ToAttribute(row) + "]");
            builder.AppendLine("public abstract " + element.ElementType + " " + propertyName + " { get; }");
            return builder.ToString();
        }

        public bool DeclaredLogonHandler { get; set; }
        public bool DeclaredAlertHandler { get; set; }
        public bool DeclaredConfirmHandler { get; set; }

        #endregion
    }
}
