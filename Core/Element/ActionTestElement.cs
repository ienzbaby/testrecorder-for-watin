using System;
using System.Drawing;
using System.Text;
using System.Xml;
using TestRecorder.Core.Formatters;
using TestRecorder.UserControls;
using System.Collections.Generic;

namespace TestRecorder.Core.Actions
{
    public class ActionTestElement : ActionElementBase
    {
        public override string Name { get { return "Test Element"; } }

        public enum AvailableTests { AreEqual, AreNotEqual, Greater, Less, GreaterOrEqual, LessOrEqual, IsTrue, IsFalse }
        public AvailableTests TestToPerform;
        public string TestingProperty;
        public string TestingValue;
        public string ExceptionMessage;
        public ActionTestElement(ActionContext context) : base(context) { }
        public override Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("TestElement.bmp");
        }

        public override IUcBaseEditor GetEditor()
        {
            return new ucTestElement();
        }

        public override bool Perform()
        {
            bool result = false;
            var element = GetTheElement();
            string propertyvalue = element.GetAttributeValue(TestingProperty);
            switch (TestToPerform)
            {
                case AvailableTests.AreEqual:
                    result = propertyvalue == TestingValue;
                    break;
                case AvailableTests.AreNotEqual:
                    result = propertyvalue != TestingValue;
                    break;
                case AvailableTests.Greater:
                    result = float.Parse(TestingValue) < float.Parse(propertyvalue);
                    break;
                case AvailableTests.GreaterOrEqual:
                    result = float.Parse(TestingValue) <= float.Parse(propertyvalue);
                    break;
                case AvailableTests.Less:
                    result = float.Parse(TestingValue) > float.Parse(propertyvalue);
                    break;
                case AvailableTests.LessOrEqual:
                    result = float.Parse(TestingValue) >= float.Parse(propertyvalue);
                    break;
                case AvailableTests.IsTrue:
                    result = bool.Parse(propertyvalue);
                    break;
                case AvailableTests.IsFalse:
                    result = !bool.Parse(propertyvalue);
                    break;
            }
            if (result == false) this.ErrorMessage = this.ExceptionMessage;
            return result;
        }

        public override string Description
        {
            get
            {
                return "Perform Test";
            }
        }

        public override CodeLine ToCode(ICodeFormatter Formatter)
        {
            var line = new CodeLine();
            line.NoModel = true;
            var builder = new StringBuilder();
            //line.Frames = Context.FrameList;
            builder.Append("Assert.That(");
            line.ModelPath = GetDocumentPath(Formatter);
            builder.Append(line.ModelPath);
            builder.Append(Formatter.MethodSeparator);
            builder.Append(ElementType);
            builder.Append("(" + Context.FindMechanism.ToString() + ")");
            builder.Append(Formatter.MethodSeparator);
            builder.Append("GetAttributeValue(\"" + TestingProperty + "\"),");

            string testvalue =  TestingValue;

            switch (TestToPerform)
            {
                case AvailableTests.AreEqual: builder.Append("Is" + Formatter.MethodSeparator + "Equal(\"" + testvalue + "\")"); break;
                case AvailableTests.AreNotEqual: builder.Append("Is" + Formatter.MethodSeparator + "Not" + Formatter.MethodSeparator + "Equal(\"" + testvalue + "\")"); break;
                case AvailableTests.Greater: builder.Append("Is" + Formatter.MethodSeparator + "GreaterThan(" + testvalue + ")"); break;
                case AvailableTests.GreaterOrEqual: builder.Append("Is" + Formatter.MethodSeparator + "GreaterThanOrEqualTo(" + testvalue + ")"); break;
                case AvailableTests.Less: builder.Append("Is" + Formatter.MethodSeparator + "LessThan(" + testvalue + ")"); break;
                case AvailableTests.LessOrEqual: builder.Append("Is" + Formatter.MethodSeparator + "LessThanOrEqualTo(" + testvalue + ")"); break;
                case AvailableTests.IsTrue: builder.Append("Is" + Formatter.MethodSeparator + "True"); break;
                case AvailableTests.IsFalse: builder.Append("Is" + Formatter.MethodSeparator + "False"); break;
            }

            builder.Append(")");
            builder.Append(Formatter.LineEnding);
            line.FullLine = builder.ToString();
            return line;
        }

        public override void LoadFromXml(XmlNode node)
        {
            base.LoadFromXml(node);
            TestToPerform = (AvailableTests) Enum.Parse(typeof (AvailableTests), node.Attributes["TestToPerform"].Value);
            TestingProperty = node.Attributes["TestingProperty"].Value;
            TestingValue = node.Attributes["TestingValue"].Value;
            ExceptionMessage = node.Attributes["ExceptionMessage"].Value;
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "TestElement");
            writer.WriteAttributeString("TestToPerform", TestToPerform.ToString());
            writer.WriteAttributeString("TestingProperty", TestingProperty);
            writer.WriteAttributeString("TestingValue", TestingValue);
            writer.WriteAttributeString("ExceptionMessage", ExceptionMessage);
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            SaveSettings(writer);
            writer.WriteEndElement();
        }
    }
}
