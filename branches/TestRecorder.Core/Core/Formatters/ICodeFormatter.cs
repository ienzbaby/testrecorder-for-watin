using System;
using System.Data;
using TestRecorder.Core.Actions;

namespace TestRecorder.Core
{
    public interface ICodeFormatter
    {
        bool FileDestination { get; set; }
        string LineEnding { get; }
        string StartupApplication { get; set; }
        string ScriptExtension { get; }
        string CommentMarker { get; }
        string VariableDeclarator { get;  }
        string NewDeclaration { get; }
        string MethodSeparator { get; }
        string ClassNameFormat(Type ClassType, string ClassVariable);
        string ElementVariable(ElementTypes elementType, string ElementVariable, string ElementValue);
        string InitialBrowser(string BrowserName, BrowserTypes browserType);
        string DisposeBrowser(string BrowserName);
        string GetProperty(ActionElementBase element, DataRow row, string propertyName);
        bool DeclaredLogonHandler { get; set; }
        bool DeclaredAlertHandler { get; set; }
        bool DeclaredConfirmHandler { get; set; }
    }
}
