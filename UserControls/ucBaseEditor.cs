using System.Windows.Forms;
using TestRecorder.Core;
using TestRecorder.Core.Actions;

namespace TestRecorder.UserControls
{
    public partial class ucBaseEditor : UserControl, IUcBaseEditor
    {
        /// <summary>
        /// 关闭编辑器
        /// </summary>
        public EdtionEvent OnCloseEdtion;
        /// <summary>
        /// 动作列表
        /// </summary>
        public ActionList ActionList { get; set; }
        /// <summary>
        /// 当前动作
        /// </summary>
        public virtual ActionBase Action { get; set; }

        public ucBaseEditor()
        {
            InitializeComponent();
        }
      
    }
}
