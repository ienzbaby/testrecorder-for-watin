using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using XD.Tools.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Xml;

namespace XD.QQ.Tests
{
    [TestFixture]
    class Test_QQ:Test_Abstract
    {
        private ActorManager manager = ActorManager.Instance();
        [Test(Description = "从NodeJs下载的文件中导入数据")]
        public void ExcuteCardImportTask()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement elem=  doc.CreateElement("root");
            elem.SetAttribute("path",@"E:\nodejs\data");

            CardImportTask task = new CardImportTask();
            task.Execute(elem);

            Console.WriteLine("完成目录的数据导入！");
        }
        [Test(Description = "从NodeJs下载的文件中导入数据")]
        public void ExcuteUinImportTask()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement elem = doc.CreateElement("root");
            elem.SetAttribute("path", @"E:\nodejs\data");

            UinImportTask task = new UinImportTask();
            task.Execute(null);
            
            Console.WriteLine("完成目录的数据导入！");
        }
        [Test]
        public void ImportFromData()
        {
            string configFile = @"D:\程序备份\ZQ_Tools\XD.Tools.Tests\Tasks.config";
            TaskManager.Initialize(configFile).Start();
            while (true)
            {
                Thread.Sleep(1000);
                GC.Collect();
            }
        }
        [Test]
        public void GetUnUsed()
        {
            var list = manager.GetUnUsed(5);
            list = manager.GetUnUsed(5);
            Console.WriteLine(list.Count);
        }
        [Test]
        public void GetList()
        {
            JavaScriptObject root = new JavaScriptObject();
            root.Add("mod","uin");
            root.Add("act", "getlist");
            string request = JavaScriptConvert.SerializeObject(root);
            string ret = JsonServices.ProcessRequest(request);
            Console.WriteLine(ret);
        }
    }
}
