using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using IfacesEnumsStructsClasses;
using WatiN.Core;
using WatiN.Core.Constraints;

namespace TestRecorder.Core.Actions
{
    class Helper
    {
        public static int GetInnerFilterNum(Document doc, ElementTypes elementType, Constraint constraint)
        {
            int result = 0;
            switch (elementType)
            {
                case ElementTypes.Area: result = doc.Areas.Filter(constraint).Count; break;
                case ElementTypes.Button: result = doc.Buttons.Filter(constraint).Count; break;
                case ElementTypes.CheckBox: result = doc.CheckBoxes.Filter(constraint).Count; break;
                case ElementTypes.Div: result = doc.Divs.Filter(constraint).Count; break;
                case ElementTypes.Element: result = doc.Elements.Filter(constraint).Count; break;
                case ElementTypes.Image: result = doc.Images.Filter(constraint).Count; break;
                case ElementTypes.Label: result = doc.Labels.Filter(constraint).Count; break;
                case ElementTypes.Link: result = doc.Links.Filter(constraint).Count; break;
                case ElementTypes.RadioButton: result = doc.RadioButtons.Filter(constraint).Count; break;
                case ElementTypes.SelectList: result = doc.SelectLists.Filter(constraint).Count; break;
                case ElementTypes.Span: result = doc.Spans.Filter(constraint).Count; break;
                case ElementTypes.Table: result = doc.Tables.Filter(constraint).Count; break;
                case ElementTypes.TableRow: result = doc.TableRows.Filter(constraint).Count; break;
                case ElementTypes.TableCell: result = doc.TableCells.Filter(constraint).Count; break;
                case ElementTypes.TextField: result = doc.TextFields.Filter(constraint).Count; break;
            }
            return result;
        }
        /// <summary>
        /// 取得对象索引
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static int GetIndexofElement(Document doc, ElementTypes elementType, Constraint constraint, IHTMLElement element)
        {
            IEnumerable result = null;
            switch (elementType)
            {
                case ElementTypes.Area: result = doc.Areas.Filter(constraint); break;
                case ElementTypes.Button: result = doc.Buttons.Filter(constraint); break;
                case ElementTypes.CheckBox: result = doc.CheckBoxes.Filter(constraint); break;
                case ElementTypes.Div: result = doc.Divs.Filter(constraint); break;
                case ElementTypes.Element: result = doc.Elements.Filter(constraint); break;
                case ElementTypes.Image: result = doc.Images.Filter(constraint); break;
                case ElementTypes.Label: result = doc.Labels.Filter(constraint); break;
                case ElementTypes.Link: result = doc.Links.Filter(constraint); break;
                case ElementTypes.RadioButton: result = doc.RadioButtons.Filter(constraint); break;
                case ElementTypes.SelectList: result = doc.SelectLists.Filter(constraint); break;
                case ElementTypes.Span: result = doc.Spans.Filter(constraint); break;
                case ElementTypes.Table: result = doc.Tables.Filter(constraint); break;
                case ElementTypes.TableRow: result = doc.TableRows.Filter(constraint); break;
                case ElementTypes.TableCell: result = doc.TableCells.Filter(constraint); break;
                case ElementTypes.TextField: result = doc.TextFields.Filter(constraint); break;
            }

            if (result != null)//=========计算索引==========
            {
                int iIndex = 0;
                foreach (var a in result)
                {
                    if ((a as WatiN.Core.Element).OuterHtml.GetHashCode() == element.outerHTML.GetHashCode())
                    {
                        return iIndex;
                    }
                    else
                    {
                        iIndex++;
                    }
                }
            }
            return 0;
        }
        public static Constraint GetConstraintByAttr(FindAttribute attribute)
        {
            string findvalue = attribute.FindValue;
            Constraint constraint = null;
            switch (attribute.FindMethod)
            {
                case FindMethods.Alt:
                    constraint = Find.ByAlt(findvalue);
                    break;
                case FindMethods.Class:
                    constraint = Find.ByClass(findvalue);
                    break;
                case FindMethods.For:
                    constraint = Find.ByFor(findvalue);
                    break;
                case FindMethods.Id:
                    constraint = Find.ById(findvalue);
                    break;
                case FindMethods.Index:
                    constraint = Find.ByIndex(Convert.ToInt32(findvalue));
                    break;
                case FindMethods.Name:
                    constraint = Find.ByName(findvalue);
                    break;
                case FindMethods.Src:
                    constraint = Find.BySrc(findvalue);
                    break;
                case FindMethods.Style:
                    constraint = Find.ByStyle(attribute.FindName, findvalue);
                    break;
                case FindMethods.Text:
                    constraint = Find.ByText(findvalue);
                    break;
                case FindMethods.Title:
                    constraint = Find.ByTitle(findvalue);
                    break;
                case FindMethods.Url:
                    constraint = Find.ByUrl(findvalue);
                    break;
                case FindMethods.Value:
                    constraint = Find.ByValue(findvalue);
                    break;
                case FindMethods.Href:
                    constraint = Find.By("href", findvalue);
                    break;
                case FindMethods.Custom:
                    constraint = Find.By(attribute.FindName, findvalue);
                    break;
            }
            return constraint;
        }
        public static ElementTypes GetElementType(IHTMLElement element)
        {
            if (element == null) return ElementTypes.Element;

            ElementTypes tagtype;

            string tagName;
            try
            {
                tagName = element.tagName.ToLower(System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return ElementTypes.Element;
            }

            if (tagName == "input")
            {
                string tagstring = GetElementAttr(element, "type").ToLower(System.Globalization.CultureInfo.InvariantCulture);
                switch (tagstring)
                {
                    case "button":
                        tagtype = ElementTypes.Button;
                        break;
                    case "submit":
                        tagtype = ElementTypes.Button;
                        break;
                    case "reset":
                        tagtype = ElementTypes.Button;
                        break;
                    case "radio":
                        tagtype = ElementTypes.RadioButton;
                        break;
                    case "checkbox":
                        tagtype = ElementTypes.CheckBox;
                        break;
                    case "file":
                        tagtype = ElementTypes.FileUpload;
                        break;
                    case "image":
                        tagtype = ElementTypes.Image;
                        break;
                    default:
                        tagtype = ElementTypes.TextField;
                        break;
                }
            }
            else
            {
                switch (element.tagName.ToLower(CultureInfo.InvariantCulture))
                {
                    case "select":
                        tagtype = ElementTypes.SelectList;
                        break;
                    case "span":
                        tagtype = ElementTypes.Span;
                        break;
                    case "div":
                        tagtype = ElementTypes.Div;
                        break;
                    case "a":
                        tagtype = ElementTypes.Link;
                        break;
                    case "li":
                        tagtype = ElementTypes.Element;
                        break;
                    case "img":
                        tagtype = ElementTypes.Image;
                        break;
                    case "form":
                        tagtype = ElementTypes.Form;
                        break;
                    case "iframe":
                    case "frame":
                        tagtype = ElementTypes.Frame;
                        break;
                    case "p":
                        tagtype = ElementTypes.Para;
                        break;
                    case "table":
                        tagtype = ElementTypes.Table;
                        break;
                    case "tbody":
                        tagtype = ElementTypes.TableBody;
                        break;
                    case "tr":
                        tagtype = ElementTypes.TableRow;
                        break;
                    case "td":
                        tagtype = ElementTypes.TableCell;
                        break;
                    case "textarea":
                        tagtype = ElementTypes.TextField;
                        break;
                    case "body":
                        tagtype = ElementTypes.Body;
                        break;
                    default:
                        tagtype = ElementTypes.Element;
                        break;
                }
            }

            return tagtype;
        }
        /// <summary>
        /// 取得元素属性
        /// </summary>
        /// <param name="element"></param>
        /// <param name="AttributeName"></param>
        /// <returns></returns>
        public static string GetElementAttr(IHTMLElement element, string AttributeName)
        {
            string strValue = "";
            try
            {
                if (AttributeName.ToLower() == "class")
                {
                    strValue = element.className;
                }
                else
                {
                    strValue = element.getAttribute(AttributeName.ToLower(), 0) as string ?? "";
                }
            }
            catch (Exception)
            {
            }
            return strValue;
        }
        /// <summary>
        /// 执行替换
        /// </summary>
        /// <param name="ReplacementRow"></param>
        /// <param name="Incoming"></param>
        /// <returns></returns>
        //public static string PerformReplacement(DataRow ReplacementRow, string Incoming)
        //{
        //    if (ReplacementRow == null) return Incoming;
        //    string output = Incoming;
        //    foreach (DataColumn column in ReplacementRow.Table.Columns)
        //    {
        //        output = Regex.Replace(output,
        //            "\\[%" + column.ColumnName + "%\\]",
        //            ReplacementRow[column.ColumnName].ToString(),
        //            RegexOptions.IgnoreCase);
        //    }
        //    return output;
        //}
        /// <summary>
        /// 文本转动作
        /// </summary>
        /// <param name="ActionObjectString"></param>
        /// <returns></returns>
        public static ActionBase StringToObject(ActionContext context, string ActionObjectString)
        {
            ActionBase output = null;
            switch (ActionObjectString)
            {
                case "AlertHandler": output = new ActionAlertHandler(context); break;
                case "ConfirmHandler": output = new ActionConfirmHandler(context); break;
                case "JavascriptHandler": output = new JavascriptHandler(context); break;
                case "WindowBack": output = new ActionBack(context); break;
                case "BrowserClick": output = new ActionClick(context); break;
                case "CloseWindow": output = new ActionCloseWindow(context); break;
                case "BrowserDoubleClick": output = new ActionDoubleClick(context); break;
                case "FireEvent": output = new ActionFireEvent(context); break;
                case "WindowForward": output = new ActionForward(context); break;
                case "Key": output = new ActionKey(context); break;
                case "Mouse": output = new ActionMouse(context); break;
                case "Navigate": output = new ActionNavigate(context); break;
                case "Hightlight": output = new ActionHighlight(context); break;
                case "WindowOpenPopup": output = new ActionOpenPopup(context); break;
                case "WindowOpen": output = new ActionOpenWindow(context); break;
                case "WindowRefresh": output = new ActionRefresh(context); break;
                case "RadioButton": output = new ActionRadio(context); break;
                case "CheckBox": output = new ActionCheckbox(context); break;
                case "SelectList": output = new ActionSelectList(context); break;
                case "TypeText": output = new ActionTypeText(context); break;
                case "DirectionKey": output = new ActionDirectionKey(context,""); break;
                case "Sleep": output = new ActionSleep(context); break;
                case "Wait": output = new ActionWait(context); break;
                case "TestElement": output = new ActionTestElement(context); break;
            }
            return output;
        }
        /// <summary>
        /// 取得ICON图片
        /// </summary>
        /// <param name="Filename"></param>
        /// <returns></returns>
        public static Bitmap GetIconFromFile(string Filename)
        {
            Filename = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Icons\\" + Filename);
            if (!File.Exists(Filename)) Filename = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Icons\\Broken.bmp");
            if (!File.Exists(Filename)) throw new FileNotFoundException("File not found: " + Filename);
            var bmpBrowser = new Bitmap(Filename);
            bmpBrowser.MakeTransparent(Color.Fuchsia);
            return bmpBrowser;
        }
    }
}
