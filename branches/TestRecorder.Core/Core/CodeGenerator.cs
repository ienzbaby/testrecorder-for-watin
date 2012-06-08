using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using TestRecorder;
using TestRecorder.Core.Actions;
using TestRecorder.Core.Formatters;

namespace TestRecorder.Core
{
    /// <summary>
    /// 代码生成
    /// </summary>
    class CodeGenerator
    {
        public ICodeFormatter Formatter;
        public BrowserTypes Browser;
        public ActionList ActionList;

        private readonly List<string> Code = new List<string>();
        private readonly List<string> Model = new List<string>();
        private readonly List<string> Windows = new List<string>();
        private readonly List<string> Frames = new List<string>();

        private readonly Dictionary<string, string> NameList = new Dictionary<string, string>();

        public string GetCode()
        {
            var builder = new StringBuilder();

            GetActionCode(null);

            builder.AppendLine(Environment.NewLine + Formatter.CommentMarker + " Windows");
            foreach (string item in Windows) builder.AppendLine(item);

            builder.AppendLine(Environment.NewLine + Formatter.CommentMarker + " Frames");
            foreach (string item in Frames) builder.AppendLine(item);

            builder.AppendLine(Environment.NewLine + Formatter.CommentMarker + " Model");
            foreach (string item in Model) builder.AppendLine(item);

            builder.AppendLine(Environment.NewLine + Formatter.CommentMarker + " Code");
            foreach (string item in Code) builder.AppendLine(item);

            return builder.ToString();
        }

        private void GetActionCode(DataRow row)
        {
            if (ActionList.Count == 0) return;

            // add the window creation to the action
            ActionBase firstAction = ActionList[0];
            string browserName = firstAction.Context.ActivePage.Browser.FriendlyName;
            BrowserWindow browserWindow = firstAction.Context.ActivePage.Browser;
            if (row != null)
            {
                int counter = row.Table.Rows.IndexOf(row);

                // this naming scheme is REALLY pathetic
                if (row.Table.Rows.Count > 1) firstAction.Context.ActivePage.Browser.FriendlyName += "_" + counter;
            }
            Windows.Add(Formatter.InitialBrowser(firstAction.Context.ActivePage.Browser.FriendlyName, Browser));


            // loop each action to create Code
            foreach (ActionBase action in ActionList)
            {
                //action.ReplacementRow = row;
                CodeLine line = action.ToCode(Formatter);

                // something to put into the Model (or not)
                if (!line.NoModel)
                {
                    var elementItem = (ActionElementBase) action;
                    string elementname = CreateName(elementItem.ElementType, elementItem.Context.FindMechanism, NameList, elementItem);
                    string modelProperty = line.ModelLocalProperty;

                    // add the frames, if available
                    if (!ItemExists((ActionElementBase)action, NameList))
                    {
                        Model.Add(Formatter.ElementVariable(elementItem.ElementType, elementname, modelProperty));
                        NameList.Add(elementItem.Context.FindMechanism.ToString(), elementname);
                    }

                    Code.Add(Formatter.VariableDeclarator + elementname + Formatter.MethodSeparator + line.ModelFunction);
                }
                else Code.Add(line.FullLine);
            }

            // put in the dispose
            ActionBase lastAction = ActionList[ActionList.Count - 1];
            if (lastAction.Context.ActivePage != null && lastAction.Context.ActivePage.Browser != null) 
                Code.Add(Formatter.DisposeBrowser(lastAction.Context.ActivePage.Browser.FriendlyName));

            // modify the friendly name, if we needed to
            if (browserWindow != null) browserWindow.FriendlyName = browserName;
        }

        private static string GetElementTypeString(ElementTypes elementType)
        {
            string elementName;

            switch (elementType)
            {
                case ElementTypes.Area:
                    elementName = "area";
                    break;
                case ElementTypes.Button:
                    elementName = "btn";
                    break;
                case ElementTypes.CheckBox:
                    elementName = "chk";
                    break;
                case ElementTypes.Div:
                    elementName = "div";
                    break;
                case ElementTypes.FileUpload:
                    elementName = "file";
                    break;
                case ElementTypes.Form:
                    elementName = "form";
                    break;
                case ElementTypes.Frame:
                    elementName = "frame";
                    break;
                case ElementTypes.Image:
                    elementName = "img";
                    break;
                case ElementTypes.Label:
                    elementName = "lbl";
                    break;
                case ElementTypes.Link:
                    elementName = "lnk";
                    break;
                case ElementTypes.Para:
                    elementName = "p";
                    break;
                case ElementTypes.RadioButton:
                    elementName = "rbn";
                    break;
                case ElementTypes.SelectList:
                    elementName = "sel";
                    break;
                case ElementTypes.Span:
                    elementName = "spn";
                    break;
                case ElementTypes.Table:
                    elementName = "tbl";
                    break;
                case ElementTypes.TableBody:
                    elementName = "tbody";
                    break;
                case ElementTypes.TableRow:
                    elementName = "tr";
                    break;
                case ElementTypes.TableCell:
                    elementName = "td";
                    break;
                case ElementTypes.TextField:
                    elementName = "txt";
                    break;
                default:
                    elementName = "_";
                    break;
            }

            return elementName;
        }

        private static string CreateName(ElementTypes elementType, IEnumerable<FindAttribute> collection, IDictionary<string, string> nameList, ActionElementBase action)
        {
            string elementName = GetElementTypeString(elementType);

            // loop the find mechanism
            foreach (FindAttribute attribute in collection)
            {
                string value =  attribute.FindValue;
                elementName += "_" + value;
            }
            elementName = Regex.Replace(elementName, @"http://", "", RegexOptions.IgnoreCase);
            elementName = Regex.Replace(elementName, @"[^a-z0-9_]+", "", RegexOptions.IgnoreCase);
            if (elementName.Length > 256) elementName = elementName.Substring(0, 256);

            if (ItemExists(action,nameList) || ItemExists(collection, nameList))
            {
                while (nameList.ContainsKey(elementName))
                {
                    elementName += "2";
                }                
            }
            
            return elementName;
        }

        private static bool ItemExists(ActionElementBase action, IDictionary<string, string> nameList)
        {
            return action != null && nameList.ContainsKey(action.Context.FindMechanism.ToString());
        }

        private static bool ItemExists(IEnumerable<FindAttribute> collection, IDictionary<string, string> nameList)
        {
            return nameList.ContainsKey(collection.ToString());
        }
    }
}
