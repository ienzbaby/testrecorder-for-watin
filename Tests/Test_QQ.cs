using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using XD.Tools.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace XD.QQ.Tests
{
    [TestFixture]
    class Test_QQ:Test_Abstract
    {
        private ActorManager manager = ActorManager.Instance();
        [Test(Description = "从NodeJs下载的文件中导入数据")]
        public void ExcuteTaskOnce()
        {
            //string path = @"E:\nodejs\Data";

            ImportUinTask task = new ImportUinTask();
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
        public void Exist()
        {
            Console.WriteLine(int.MaxValue);
            Console.WriteLine(int.MaxValue / 100);

            Console.WriteLine("----------------------------");

            Console.WriteLine(long.MaxValue);
            Console.WriteLine(long.MaxValue / 100);
        }
    }
}
