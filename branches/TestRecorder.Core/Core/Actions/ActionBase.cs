using System;
using System.Data;
using System.Drawing;
using System.Xml;
using TestRecorder.Core.Formatters;
using TestRecorder.UserControls;
using System.Collections.Generic;

namespace TestRecorder.Core.Actions
{
   
    /// <summary>
    /// 抽象的动作类
    /// </summary>
    public abstract class ActionBase
    {
        public BreakpointIndicators Breakpoint = BreakpointIndicators.NoBreakpoint;
        /// <summary>
        /// error message (if any) for the action
        /// </summary>
        public string ErrorMessage = "";
        //public DataRow ReplacementRow = null;
        /// <summary>
        /// status of this action
        /// </summary>
        public StatusIndicators Status { get; set; }
        //public ActionList ParentTest { get; set; }

        /// <summary>
        /// virtual method to retrieve the icon for the action
        /// </summary>
        /// <returns>icon as a bitmap</returns>
        public abstract Bitmap GetIcon();

        public ActionContext Context;
        /// <summary>
        /// 设置上下文
        /// </summary>
        /// <param name="context"></param>
        public void SetContext(ActionContext context)
        {
            this.Context = context;
        }
        public ActionBase(ActionContext context)
        {
            this.Context = context;
        }
        /// <summary>
        /// executes this action
        /// </summary>
        /// <returns>true on success</returns>
        public abstract bool Perform();
        /// <summary>
        /// retrieves the description text about this action
        /// </summary>
        /// <returns>description of the action</returns>
        public abstract string Name { get; }
        public abstract string Description { get; }
        /// <summary>
        /// returns object as code, formatted for a specific language
        /// </summary>
        /// <param name="Formatter">language-specific formatting</param>
        /// <returns>code from the object</returns>
        public abstract CodeLine ToCode(ICodeFormatter Formatter);
        public abstract void SaveToXml(XmlWriter writer);

        /// <summary>
        /// validates the action
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool Validate()
        {
            return true;
        }
        /// <summary>
        /// 从Xml中加载
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="node"></param>
        public virtual void LoadFromXml(XmlNode node)
        {
            int pagehash = Convert.ToInt32(node.Attributes.GetNamedItem("PageHash").Value);
            if (pagehash != 0 && Context.PageContext.FindByHash(pagehash) != null)
            {
                Context.ActivePage = Context.PageContext.FindByHash(pagehash);
            }
        }
        /// <summary>
        /// 取得编辑器
        /// </summary>
        /// <param name="MainForm"></param>
        /// <returns></returns>
        public virtual IUcBaseEditor GetEditor()
        {
            return null;
        }
        
    }
}
