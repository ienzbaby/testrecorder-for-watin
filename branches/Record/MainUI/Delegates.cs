using System;
using System.Collections.Generic;
using System.Text;
using TestRecorder.Core.Actions;
using System.Windows.Forms;

namespace TestRecorder
{
        public delegate void ActionCounterIncrementedEvent(int index);
        public delegate void RunStartEvent(ActionBase action);
        public delegate void RunResultEvent(ActionBase action,bool result,string message);
        public delegate void RunCompleteEvent(bool isAbort);
        public delegate void RunJavascriptEvent();

        public delegate void BreakpointEvent(ActionBase action);
        
        public delegate void ActionStatusEvent(ActionBase action);
        public delegate void UpdateCellBreakpoint(int rowIndex, BreakpointIndicators breakpointType);
        public delegate void UpdateResultEvent(bool result);
        public delegate void GridInsertEvent(int rowIndex, ActionBase actionObject);
        public delegate void SetCellBreakpointEvent(int rowIndex, BreakpointIndicators breakpointType);
        public delegate void GridAddEvent(ActionBase actionObject);
        public delegate void PopupResultEvent(DialogResult result,string url,string title);
}