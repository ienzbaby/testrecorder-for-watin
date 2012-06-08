using System;
using System.Data;
using System.Text;
using TestRecorder.Core.Actions;

namespace TestRecorder.Core
{
    public class PerlCodeFormatter : ICodeFormatter
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
                return ".pl";
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
                return "$";
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
                return "->";
            }
        }

        public string ClassNameFormat(Type classType, string classVariable)
        {
            return VariableDeclarator + classVariable + " = new " + classType + "();";
        }

        public string ElementVariable(ElementTypes elementType, string elementVariable, string elementValue)
        {
            return VariableDeclarator + elementVariable + " = " + elementValue + LineEnding;
        }

        public string InitialBrowser(string browserName, BrowserTypes browserType)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Win32::OLE->Initialize(Win32::OLE::COINIT_APARTMENTTHREADED);");
            builder.AppendLine("$Interface = Win32::OLE->new('WatiN.COMInterface') or die 'Cannot start WatiN COM interface';");

            switch (browserType)
            {
                default: builder.AppendLine(VariableDeclarator + browserName + " = $Interface->CreateIE();"); break;
                case BrowserTypes.FireFox: builder.AppendLine(VariableDeclarator + browserName + " = $Interface->CreateFireFox();"); break;
                case BrowserTypes.Chrome: builder.AppendLine(VariableDeclarator + browserName + " = $Interface->CreateChrome();"); break;
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
