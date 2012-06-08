using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NUnit.Framework;
using XD.Client.Tests;
using System.Threading;
using XD.Tools.Tasks;
using System.Text;
using System.Data;
using XD.Tools.DBUtility;

namespace XD.QQ.Tests
{
    [TestFixture]
    class Test_QQ:Test_Abstract
    {
        private ActorManager manager = ActorManager.Instance();
        [Test(Description = "从NodeJs下载的文件中导入数据")]
        public void ExcuteTaskOnce()
        {
            string path = @"E:\nodejs\Data";
            ImportTask task = new ImportTask();
            task.SearchPath = path;
            task.Execute(null);
            Console.WriteLine("完成目录[{0}]的数据导入！", path);
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
        public void GetPartionFunction()
        {
            DataTable dt = DataFactory.ExecuteSql("select * from QQ_Actor limit 1,100").Tables[0];
            Console.WriteLine(dt.Rows.Count);
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
            IList<long> ids = new List<long>();
            ids.Add(-9223119721086940864);
            ids.Add(-9223149);

            ids = manager.Exists(ids);
            Console.WriteLine(ids.Count);
        }
    }
}
