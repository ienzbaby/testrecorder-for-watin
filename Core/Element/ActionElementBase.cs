using System;
using System.Text;
using System.Xml;
using mshtml;
using TestRecorder.UserControls;
using WatiN.Core;
using AttributeConstraint=WatiN.Core.Constraints.AttributeConstraint;
using IHTMLDocument2=IfacesEnumsStructsClasses.IHTMLDocument2;
using IHTMLElement=IfacesEnumsStructsClasses.IHTMLElement;
using System.Collections.Generic;
using WatiN.Core.Constraints;
using System.Collections;

namespace TestRecorder.Core.Actions
{
    /// <summary>
    /// 抽象基类
    /// </summary>
    public abstract class ActionElementBase : ActionBase
    {
        public ElementTypes ElementType { get; set; }
        protected ActionElementBase(ActionContext context):base(context){
        }
       
        /// <summary>
        /// 对当前情况的描述
        /// </summary>
        /// <returns></returns>
        protected string GetElemDesc()
        {
            var builder = new StringBuilder();
            builder.Append(ElementType + " ");
            bool first = true;
            foreach (var attr in Context.FindMechanism)
            {
                if (attr.FindValue != null && attr.FindValue.Length > 0)
                {
                    if (first == true)
                    {
                        builder.Append(attr.FindMethod + " = " + attr.FindValue);
                        first = false;
                    }
                    else
                    {
                        builder.Append(" and " + attr.FindMethod + " = " + attr.FindValue);
                    }

                }
            }
            if (Context.GetFrame() != null)
            {
                builder.Append(" and Frame=" + Context.GetFrame().Url + "");
            }
            return builder.ToString();
        }

        protected string GetDocumentPath(ICodeFormatter Formatter)
        {
            var builder = new StringBuilder();
            builder.Append(Context.ActivePage.Browser.FriendlyName);
            //if (Context.FrameList.Count > 0)
            //{
            //    builder.Append(Formatter.MethodSeparator);
            //    builder.Append("Frame(" + Context.FrameList + ")");
            //}
            return builder.ToString();
        }

        public override IUcBaseEditor GetEditor()
        {
            return null;
        }

        public override void LoadFromXml( XmlNode node)
        {
            base.LoadFromXml(node);

            string elementTypeString = node.SelectSingleNode("ElementType").InnerText;
            ElementType = (ElementTypes) Enum.Parse(typeof (ElementTypes), elementTypeString);

            XmlNode findernode = node.SelectSingleNode("FinderSet");
            if (findernode == null) return;

            XmlNodeList finderList = findernode.SelectNodes("Finder");
            if (finderList == null) return;
            
            foreach (XmlNode child in finderList)
            {
                var attribute = new FindAttribute();
                attribute.FromXml(child);
                Context.FindMechanism.Add(attribute);
            }

            XmlNode framenode = node.SelectSingleNode("Frame");
            if (framenode != null)
            {
                Context.FrameUrl = framenode.Attributes["URL"].Value;
            }
        }

        protected void SaveSettings(XmlWriter writer)
        {
            writer.WriteElementString("ElementType", ElementType.ToString());
            Context.FindMechanism.ToXML("FinderSet", writer);
            if (Context.FrameUrl.Length>0)
            {
                writer.WriteStartElement("Frame");
                writer.WriteAttributeString("URL", Context.FrameUrl);
                writer.WriteEndElement();
            }
        }
        /// <summary>
        /// 取得匹配元素
        /// </summary>
        /// <returns></returns>
        protected Element GetTheElement()
        {
            Element result = null;
            var constraint = GetAllConstraint();

            Document doc = Context.GetDocument();
            switch (ElementType)
            {
                case ElementTypes.Area: result = doc.Area(constraint); break;
                case ElementTypes.Button: result = doc.Button(constraint); break;
                case ElementTypes.CheckBox: result = doc.CheckBox(constraint); break;
                case ElementTypes.Div: result = doc.Div(constraint); break;
                case ElementTypes.Element: result = doc.Element(constraint); break;
                case ElementTypes.Image: result = doc.Image(constraint); break;
                case ElementTypes.Label: result = doc.Label(constraint); break;
                case ElementTypes.Link: result = doc.Link(constraint); break;
                case ElementTypes.RadioButton: result = doc.RadioButton(constraint); break;
                case ElementTypes.SelectList: result = doc.SelectList(constraint); break;
                case ElementTypes.Span: result = doc.Span(constraint); break;
                case ElementTypes.Table: result = doc.Table(constraint); break;
                case ElementTypes.TableRow: result = doc.TableRow(constraint); break;
                case ElementTypes.TableCell: result = doc.TableCell(constraint); break;
                case ElementTypes.TextField: result = doc.TextField(constraint); break;
            }
            return result;
        }
        /// <summary>
        /// 取得相关限制
        /// </summary>
        /// <returns></returns>
        protected Constraint GetAllConstraint()
        {
            Constraint constraint = null; //=====计算第一个属性对=======

            if (Context.FindMechanism.Count > 0) constraint = Helper.GetConstraintByAttr(Context.FindMechanism[0]);
            if (Context.FindMechanism.Count > 1)
            {
                for (int i = 1; i < Context.FindMechanism.Count; i++)
                {
                    var finder = Context.FindMechanism[i];
                    if (finder != null && finder.FindValue != null)
                    {
                        Constraint current = Helper.GetConstraintByAttr(finder);
                        if (current != null) constraint = constraint.And(current);
                    }
                }
            }
            return constraint;
        }
        /// <summary>
        /// Determines the find method using the Settings' find pattern
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="element">element to search for</param>
        public void SetFindMethod(BrowserWindow browser, IHTMLElement element)
        {
            //========确定元素类型=======
            ElementType = Helper.GetElementType(element);

            // find the appropriate find method
            FindAttribute Finder = GetOneFinder(element);

            // find the page index,no exist correct attribute
            if (Finder.FindValue != null)
            {
                Context.FindMechanism.Add(Finder);

                int filterCount = 0;
                List<string> lFilters = new List<string>();
                lFilters.Add(Finder.FindMethod.ToString());

                // make sure the element is unique
                while (filterCount < Context.FindPattern.Count && this.GetFilterNum() > 1)
                {
                    var method = Context.FindPattern[filterCount];
                    string value = Helper.GetElementAttr(element, method.ToString());
                    if (IsMethodUsed(method) == false && value != "" && lFilters.Contains(method.ToString()) == false)
                    {
                        FindAttribute f = new FindAttribute();
                        f.FindMethod = method;
                        f.FindValue = value;

                        Context.FindMechanism.Add(f);
                        lFilters.Add(method.ToString());
                    }
                    filterCount++;
                }
                if (this.GetFilterNum() > 1)
                {
                    FindAttribute fIndex = new FindAttribute();
                    fIndex.FindMethod = FindMethods.Index;
                    fIndex.FindValue = Helper.GetIndexofElement(Context.GetDocument(), this.ElementType, this.GetAllConstraint(), element).ToString();
                    Context.FindMechanism.Add(fIndex);
                }
            }
            SetFrameList(element);
        }
        private void SetFrameList(IHTMLElement element)
        {
            IHTMLDocument2 doc=null;
            try
            {
                doc = element.document as IHTMLDocument2;
            }
            catch (Exception)
            {
                return;
            }

            var frameElement = SetFrame(doc);

            while (frameElement != null && frameElement.parentElement.GetType() == typeof(HTMLFrameSetSiteClass))
            {
                doc = frameElement.document as IHTMLDocument2;
                if (doc == null) return;
                frameElement = SetFrame(doc);
            }
            //Context.FrameList.Reverse();
        }
        /// <summary>
        /// 设置框架
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private HTMLFrameBase SetFrame(IHTMLDocument2 doc)
        {
            if (doc == null) return null;
            var frameCollection = doc.frames as HTMLWindow2;
            if (frameCollection == null) return null;
            frameCollection = frameCollection.frames as HTMLWindow2;
            if (frameCollection == null || frameCollection.frameElement==null) return null;

            // HTMLIFrameClass
            if (frameCollection.frameElement.GetType() == typeof(HTMLFrameElementClass))
            {
                var frameElement = frameCollection.frameElement as HTMLFrameElementClass;
                if (frameElement == null) return null;
                FindAttribute attribute = GetOneFinder((IHTMLElement) frameElement);
                //if (attribute != null) Context.FrameList.Add(attribute);
                return (HTMLFrameBase) frameElement;
            }
            if (frameCollection.frameElement.GetType() == typeof(HTMLIFrameClass))
            {
                var frameElement = frameCollection.frameElement as HTMLIFrameClass;
                if (frameElement == null) return null;
                FindAttribute attribute = GetOneFinder((IHTMLElement)frameElement);
                //if (attribute != null) Context.FrameList.Add(attribute);
                return (HTMLFrameBase)frameElement;
            }
            return null;
        }
        /// <summary>
        /// 取得符合条件的数字
        /// </summary>
        /// <returns></returns>
        public int GetFilterNum()
        {
            try
            {
                return Helper.GetInnerFilterNum(Context.GetDocument(),this.ElementType,GetAllConstraint());
            }
            catch (Exception)
            {
                return 0;
            }
        }
       
        /// <summary>
        /// 取得元素
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private FindAttribute GetOneFinder(IHTMLElement element)
        {
            var Finder = new FindAttribute();
            foreach (var method in Context.FindPattern)
            {
                string value = Helper.GetElementAttr(element, method.ToString());
                if (IsMethodUsed(method) == false && value != "")
                {
                    Finder.FindMethod = method;
                    Finder.FindValue = value;
                    break;
                }
            }
            return Finder;
        }
        private bool IsMethodUsed(FindMethods method)
        {
            foreach (FindAttribute attribute in Context.FindMechanism)
            {
                if (attribute.FindMethod == method) return true;
            }
            return false;
        }
    }
}
