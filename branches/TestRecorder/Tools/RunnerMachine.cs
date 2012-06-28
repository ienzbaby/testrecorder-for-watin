using System.Data;
using System.Threading;
using System.Windows.Forms;
using TestRecorder.Core.Actions;
using TestRecorder.Core;

namespace TestRecorder.Tools
{
    /// <summary>
    /// 执行测试用例
    /// </summary>
    public sealed class  RunnerManager
    {
        //监听事件 
        public ActionCounterIncrementedEvent OnActionCounterIncremented;
        public RunStartEvent OnRunStarted;
        public RunCompleteEvent OnRunCompleted;
        public RunResultEvent OnRunResult;
       
        public RunJavascriptEvent OnRunJavascript;
        public BreakpointEvent OnBreakpoint;
        public ActionStatusEvent OnActionStatus;
        public UpdateCellBreakpoint OnSetCellBreakpoint;
        public UpdateResultEvent OnUpdateResult;
        public PopupResultEvent OnPopupResult;
        public AppSettings Settings;

        //public ActionList Actions;
        private bool BreakpointSleep = true;
        private bool SingleStep = false;
        private ActionBase CurrentAction;
        private ActionList currentList;
        private Thread Current;
        /// <summary>
        /// 是否活动状态
        /// </summary>
        /// <returns></returns>
        public bool IsAlive()
        {
            if (Current != null && Current.IsAlive)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 中断
        /// </summary>
        /// <param name="times"></param>
        public void AbortAndJoin(int times)
        {
            Current.Abort();
            Current.Join(times);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(ActionList actionlist)
        {
            currentList = actionlist;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="actionlist"></param>
        public RunnerManager()
        {
        }

        public void DeleteAllBreakpoints()
        {
            int counter = 1;
            foreach (ActionBase item in currentList)
            {
                if (item.Breakpoint != BreakpointIndicators.NoBreakpoint)
                {
                    item.Breakpoint = BreakpointIndicators.NoBreakpoint;
                    if(OnSetCellBreakpoint!=null ) this.OnSetCellBreakpoint(counter, BreakpointIndicators.NoBreakpoint);
                }
                counter++;
            }
        }
        public void DisableAllBreakpoints()
        {
            int counter = 0;
            foreach (ActionBase item in currentList)
            {
                if (item.Breakpoint != BreakpointIndicators.NoBreakpoint)
                {
                    item.Breakpoint = BreakpointIndicators.InactiveBreakpoint;
                    if (OnSetCellBreakpoint != null) this.OnSetCellBreakpoint(counter, BreakpointIndicators.NoBreakpoint);
                }
                counter++;
            }
        }

        public void ReEnableBreakpoints()
        {
            int counter = 0;
            foreach (ActionBase item in currentList)
            {
                if (item.Breakpoint != BreakpointIndicators.NoBreakpoint)
                {
                    item.Breakpoint = BreakpointIndicators.ActiveBreakpoint;
                    if (OnSetCellBreakpoint != null) this.OnSetCellBreakpoint(counter, BreakpointIndicators.NoBreakpoint);
                }
                counter++;
            }
        }
        public void Continue()
        {
            this.SingleStep = false;
            this.BreakpointSleep = false;
        }

        public void Stop()
        {
            Current.Abort();
            if (OnRunCompleted != null) OnRunCompleted(true);
        }

        public void RunStep(int index)
        {
            this.CurrentAction = currentList[index];
            this.SingleStep = true;
            this.BreakpointSleep = false;
            this.RunStep(this.CurrentAction);
        }
        /// <summary>
        /// 启动测试
        /// </summary>
        public void Run()
        {
            //开始运行
            this.Current = new Thread(this.RunInnerTest);
            Current.SetApartmentState(ApartmentState.STA);
            Current.Start();
        }
        /// <summary>
        /// 执行测试用例
        /// </summary>
        private void RunInnerTest()
        {
            RunTestInstance(null);
            //======脚本完成事件==========
            ReEnableBreakpoints();
            if (OnRunCompleted != null) OnRunCompleted(false);
        }
        //=========执行单个测试实例=============
        private void RunTestInstance(DataRow row)
        {
            int rowcounter = 0;
            foreach (ActionBase action in currentList)
            {
                if (OnActionCounterIncremented != null) OnActionCounterIncremented(++rowcounter);
                
                CurrentAction = action;
                RunStep(action);
            }
        }
        /// <summary>
        /// 真实的动作
        /// </summary>
        /// <param name="action"></param>
        /// <param name="row"></param>
        private void RunStep(ActionBase action)
        {
            if (SingleStep)
            {
                BreakpointSleep = true;
                while (BreakpointSleep)
                {
                    Thread.Sleep(500);
                    Application.DoEvents();
                }
            }
            else if (action.Breakpoint == BreakpointIndicators.ActiveBreakpoint)
            {
                if (OnBreakpoint != null) OnBreakpoint(action);

                BreakpointSleep = true;
                while (BreakpointSleep)
                {
                    Thread.Sleep(500);
                    Application.DoEvents();
                }
            }

            if (OnRunStarted != null) OnRunStarted(action);
            if (action is JavascriptHandler && OnRunJavascript != null) OnRunJavascript();

            bool result = action.Perform();
            Thread.Sleep(Settings.GlobalWaitTime);

            if (result == false && OnRunResult != null)
            {
                OnRunResult(action, result, action.ErrorMessage);
            }
            //监听特殊窗口事件
            if (action is ActionConfirmHandler && OnUpdateResult != null)
            {
                OnUpdateResult((action as ActionConfirmHandler).Result);
            }
            else if (action is ActionAlertHandler && OnUpdateResult != null)
            {
                OnUpdateResult(true);
            }
            else if (action is ActionOpenPopup && OnPopupResult != null)
            {
                ActionOpenPopup win = action as ActionOpenPopup;
                this.OnPopupResult(DialogResult.OK, win.BrowserURL, win.BrowserTitle);
            }
            else if (action is ActionCloseWindow && OnPopupResult != null)
            {
                ActionCloseWindow win = action as ActionCloseWindow;
                this.OnPopupResult(DialogResult.Cancel,"", "");
            }
            if (OnActionStatus != null) OnActionStatus(action);
        }
    }
}
