using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using TestRecorder.Core.Formatters;
using TestRecorder.UserControls;
using WatiN.Core;
using Timer=System.Windows.Forms.Timer;
using System.Collections.Generic;

namespace TestRecorder.Core.Actions
{
    public class ActionFileDialog : ActionElementBase
    {
        public override string Name { get { return "Radio Button"; } }
        public string Filename { get; set; }

        public ActionFileDialog(ActionContext context):base(context) 
        {
            ElementType = ElementTypes.FileUpload;
        }

        public override IUcBaseEditor GetEditor()
        {
            return new ucFileUpload();
        }

        public override Bitmap GetIcon()
        {
            return Helper.GetIconFromFile("Upload.bmp");
        }

        public class ClickDialogThread
        {
            public Element element;
            public void Run()
            {
                element.ClickNoWait();                
            }
        }

        private Timer EnterFilenameTimer;
        /// <summary>
        /// 取得上传文件
        /// </summary>
        /// <returns></returns>
        protected FileUpload GetFileElement()
        {
            var constraint = GetAllConstraint();
            return Context.ActivePage.Browser.FileUpload(constraint);
        }
        /// <summary>
        /// 执行动作
        /// </summary>
        /// <returns></returns>
        public override bool Perform()
        {
            bool result;
            try
            {
                var element = GetFileElement();
                if (element != null)
                {
                    var clickDialog = new ClickDialogThread {element = element};

                    var thread = new Thread(clickDialog.Run) {Priority = ThreadPriority.Highest};
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();

                    EnterFilenameTimer = new Timer {Interval = 2000, Enabled = true};
                    EnterFilenameTimer.Tick += EnterFilenameTimer_Tick;
                    
                    while (EnterFilenameTimer.Enabled) Application.DoEvents();
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

        void EnterFilenameTimer_Tick(object sender, EventArgs e)
        {
            EnterFilenameTimer.Enabled = false;
            SendKeys.SendWait(Filename + "{ENTER}");
        }


        public override string Description
        {
            get
            {
                return this.GetElemDesc() + ", File: " + Filename;
            }
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
            line.ModelFunction = "Set(\"" + Filename + "\")" + Formatter.LineEnding;
            line.FullLine = builder.ToString();
            return line;
        }

        public override void LoadFromXml( XmlNode node)
        {
            base.LoadFromXml(node);
            Filename = node.Attributes["Filename"].Value;
        }

        public override void SaveToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Action");
            writer.WriteAttributeString("ActionType", "FileUpload");
            writer.WriteAttributeString("Filename", Filename);
            writer.WriteAttributeString("PageHash", Context.ActivePage != null ? Context.ActivePage.HashCode.ToString() : "-1");
            SaveSettings(writer);
            writer.WriteEndElement();
        }
    }
}
