using System;
using System.Diagnostics;
using System.IO;
using log4net;
using NUnit.Framework;
using XD.Tools;

namespace XD.Client.Tests
{
    /// <summary>
    /// 抽象测试类
    /// </summary>
    class Test_Abstract
    {
        protected ILog log = null;
        protected Stopwatch sw = new Stopwatch();
        [SetUp]
        public void SetWorkFlow()
        {
            MockHttpContext mock = new MockHttpContext(false);
            System.Web.HttpContext.Current = mock.Context;

            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("XD.Tools.Tests.dll.config"));
            log = LogManager.GetLogger(this.GetType());

            sw.Start();
            this.InitData();
        }
        
        [TearDown]
        public void ClearWorkFlow()
        {
            Console.Out.Flush();
            sw.Stop();
        }

        protected virtual void InitData() {
            //XD.Appeal.Model.Admin mAdmin = bAdmin.GetLoginModel("admin", "");
            //bAdmin.SetLoginState(mAdmin);
        }
    }
}