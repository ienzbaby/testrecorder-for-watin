using System;
using TestRecorder.Core.Actions;
using System.Windows.Forms;
namespace TestRecorder.Core
{
    /// <summary>
    /// 用户控件接口
    /// </summary>
    public interface IUcBaseEditor
    {
        ActionBase Action { get; set; }
        //ActionList ActionList { get; set; }
    }
}
