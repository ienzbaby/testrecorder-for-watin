using System;
using System.Collections.Generic;
using System.Text;

namespace TestRecorder.Core.Actions
{
    /// <summary>
    /// 断点指示器
    /// </summary>
    public enum BreakpointIndicators
    {
        NoBreakpoint = 0,
        ActiveBreakpoint = 1,
        InactiveBreakpoint = 2
    }

    /// <summary>
    /// status indicators for an action
    /// 状态指示器
    /// </summary>
    public enum StatusIndicators
    {
        /// <summary>
        /// status is unknown
        /// </summary>
        Unknown,
        /// <summary>
        /// action target has been validated or action was successful
        /// </summary>
        Validated,
        /// <summary>
        /// action target is not valid or action was unsuccessful
        /// </summary>
        Faulted
    }
}
