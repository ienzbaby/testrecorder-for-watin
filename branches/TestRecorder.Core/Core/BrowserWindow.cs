using System;
using System.Collections.Generic;
using System.Text;
using SHDocVw;
using WatiN.Core;
using WatiN.Core.DialogHandlers;

namespace TestRecorder.Core
{
    /// <summary>
    /// 浏览器窗口
    /// </summary>
    public class BrowserWindow : WatiN.Core.IE
    {
        public string FriendlyName = "window";

        private static BrowserWindow _instance = null;
        public static BrowserWindow Instance(csExWB.cEXWB ieContainer)
        {
            if (_instance == null)
            {
                _instance = new BrowserWindow(ieContainer);
            }
            return _instance;
        }
        private BrowserWindow(csExWB.cEXWB ieContainer)
            : base(ieContainer.WebbrowserObject)
        {
            WatiN.Core.Settings.MakeNewIeInstanceVisible = false;
            WatiN.Core.Settings.AutoStartDialogWatcher = false;
        }
    }
}
