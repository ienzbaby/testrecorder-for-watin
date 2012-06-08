using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using mshtml;
using TestRecorder.Core.Formatters;
using TestRecorder.UserControls;
using WatiN.Core;
using WatiN.Core.Native.InternetExplorer;
using System.Collections.Generic;
using System.Drawing;

namespace TestRecorder.Core.Actions
{
    public class ActionSelectList : ActionElementBase
    {
        private IfacesEnumsStructsClasses.IHTMLElement ActiveElement { get; set; }
        private string _selectedValue;

        public override string Name
        {
            get { return "Select List"; }
        }
        public bool Regex { get; set; }
        public bool ByValue { get; set; }
        public string SelectedText { get; set; }
        public string SelectedValue
        {
            get
            {
                if (string.IsNullOrEmpty(_selectedValue))
                {
                    if (ByValue) Helper.GetElementAttr(ActiveElement, "value");
                    else
                    {
                        var sel = ActiveElement as IHTMLSelectElement;
                        if (sel == null) return null;
                        for (int i = 0; i < sel.length; i++)
                        {
                            var op = sel.item(i, i) as IHTMLOptionElement;
                            if (op != null && op.selected)
                            {
                                _selectedValue = op.value;
                                SelectedText = op.text;
                                break;
                            }
                        }
                    }
                }
                return _selectedValue;
            }
            set { _selectedValue = value; }
        }


        public ActionSelectList(ActionContext context) : base(context) { }

        public override IUcBaseEditor GetEditor()
        {
            return new ucSelectList();
        }

        public override Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("SelectList.bmp");
        }

        public override string Description
        {
            get
            {
                return this.GetElemDesc() + ", Select " + SelectedText + " (" + SelectedValue + ")";
            }
        }

        public override bool Perform()
        {
            bool result;
            try
            {
                var element = (SelectList) GetTheElement();
                if (element != null)
                {
                    ActiveElement = (IfacesEnumsStructsClasses.IHTMLElement)((IEElement)element.NativeElement).AsHtmlElement;
                    if (ByValue)
                    {
                        if (Regex) element.SelectByValue(new Regex(SelectedValue));
                        else element.SelectByValue(SelectedValue);
                    }
                    else
                    {
                        if (Regex) element.Select(new Regex(SelectedValue));
                        else element.Select(SelectedValue);
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                result = false;
            }

            return result;
        }

        public override bool Validate()
        {
            bool result;

            try
            {
                var element = (SelectList) GetTheElement();
                string itemdata = ByValue ? element.SelectedOption.Value : element.SelectedOption.Text;

                ActiveElement = (IfacesEnumsStructsClasses.IHTMLElement)((IEElement)element.NativeElement).AsHtmlElement;
                if (Regex) result = System.Text.RegularExpressions.Regex.IsMatch(itemdata, SelectedValue);
                else result = itemdata == SelectedValue;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                result = false;
            }

            return result;
        }

        public override CodeLine ToCode(ICodeFormatter Formatter)
        {
            var line = new CodeLine();
            line.Attributes = Context.FindMechanism;
            var builder = new StringBuilder();
            //line.Frames = Context.FrameList;
            line.ModelPath = GetDocumentPath(Formatter);
            builder.Append(line.ModelPath);
            builder.Append(Formatter.MethodSeparator);
            builder.Append(ElementType);
            builder.Append("(" + Context.FindMechanism.ToString() + ")");
            line.ModelLocalProperty = builder.ToString();
            builder.Append(Formatter.MethodSeparator);

            string localvalue = SelectedValue;
            if (Regex)
            {
                if (ByValue) line.ModelFunction = "SelectByValue(new Regex(\"" + localvalue + "\"))";
                else line.ModelFunction = "Select(new Regex(\"" + localvalue + "\"))";
            }
            else
            {
                if (ByValue) line.ModelFunction = "SelectByValue(\"" + localvalue + "\")";
                else line.ModelFunction = "Select(\"" + localvalue + "\")";
            }
            line.ModelFunction += Formatter.LineEnding;
            builder.Append(line.ModelFunction);
            line.FullLine = builder.ToString();
            return line;
        }

        public override void LoadFromXml( XmlNode node)
        {
            base.LoadFromXml( node);
            ByValue = node.Attributes["ByValue"].Value == "1";
            Regex = node.Attributes["Regex"].Value == "1";
            SelectedValue = node.Attributes["SelectedValue"].Value;
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "SelectList");
            writer.WriteAttributeString("ByValue", ByValue?"1":"0");
            writer.WriteAttributeString("SelectedValue", SelectedValue);
            writer.WriteAttributeString("Regex", Regex?"1":"0");
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            SaveSettings(writer);
            writer.WriteEndElement();
        }
    }
}
