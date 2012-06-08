using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using XD.Tools.Tasks;

namespace XD.QQ
{
    /// <summary>
    /// 通过字典缓存，提高执行效率
    /// </summary>
    public class ImportTask : ITask
    {
        private ActorManager manager = ActorManager.Instance();
        public int Total = 0;
        public string SearchPath = @"E:\nodejs\Data";

        private DataTable dtTemplate;
        private long CurrentNum = 0;
        private string ConnStr= @"max pool size=512; Server=192.168.1.104\SQL2005;initial catalog=SpaceBuilder;User Id=sa;Password=sa;";
        private string TableName = "QQ_Actor";
        private int PerBatchSize = 1000;

        private Stopwatch sw = new Stopwatch();

        private IEnumerable GetFiles()
        {
            FileDirectoryEnumerable fileSearcher = new FileDirectoryEnumerable();
            fileSearcher.SearchPath = SearchPath;
            fileSearcher.ReturnStringType = true;
            fileSearcher.SearchPattern = "*.log";
            //e.SearchDirectory = false;
            //e.SearchFile = true;
            return fileSearcher;
        }
        private void Init(IDictionary<long, Actor> dicMain)
        {
            //======初始化======
            CurrentNum = 0;
            dicMain.Clear();
            //DataFactory.ExecuteSql("truncate table QQ_Backup");
        }
        public void Execute(XmlElement xElement)
        {
            dtTemplate = manager.GetRecordByRowNumber(0, 1, "").Tables[0];
            dtTemplate.Columns.Remove("RowId");//去除序号列
            sw.Start();

            IDictionary<long, Actor> dicMain = new Dictionary<long, Actor>(10000);
            foreach (string name in GetFiles())
            {
               
                this.Init(dicMain);
                string path = SearchPath + @"\" + name;
                try
                {
                    this.ReadActorFromFile(dicMain, path);
                    File.Delete(path);
                }
                catch (Exception err)
                {
                    Console.WriteLine("File Read Error{0}:{1}", err.Message, err.StackTrace);
                }

                if (dicMain.Count > 0) this.SqlBulkImport(dicMain);
            }
        }
        private DataTable CreateDataTable(IDictionary<long, Actor> dicMain){
            
            DataTable dtCopy=dtTemplate.Clone();
            foreach (Actor actor in dicMain.Values)
            {
                DataRow dr = dtCopy.NewRow();
                dr["Id"] = actor.Id;
                dr["Uin"] = actor.Uin;
                dr["Name"] = actor.Name;
                dr["Sex"] = actor.Sex;
                dr["Age"] = actor.Age;
                dr["District"] = actor.District;
                dr["Title"] = actor.Title;
                dr["Photo"] = actor.Photo;
                dr["State"] = 0;
                dtCopy.Rows.Add(dr);
            }
            return dtCopy;
        }
        private void SqlBulkImport(IDictionary<long, Actor> dicMain)
        {
            DataTable dtReal = this.CreateDataTable(dicMain);
            this.SqlBulkFromDataTable(dtReal, TableName);
        }
        /// <summary>
        /// 批量导入数据
        /// </summary>
        /// <param name="dtImport"></param>
        private void SqlBulkFromDataTable(DataTable dtImport,string tableName)
        {
            CurrentNum = dtImport.Rows.Count;
            // Create the SqlBulkCopy object using a connection string. 
            // In the real world you would not use SqlBulkCopy to move
            // data from one table to the other in the same database.
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(ConnStr,SqlBulkCopyOptions.KeepIdentity|SqlBulkCopyOptions.TableLock))
            {
                bulkCopy.DestinationTableName = tableName;
                bulkCopy.BatchSize = PerBatchSize;
                bulkCopy.BulkCopyTimeout = 60000;
                bulkCopy.NotifyAfter = PerBatchSize;
                
                // Set up the event handler to notify after 50 rows.
                bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);

                try
                {
                    int count1 = manager.Count();
                    bulkCopy.WriteToServer(dtImport);
                    int count2 = manager.Count();

                    Total += count2 - count1;
                    Console.WriteLine("total add={0},current add={1}/{2},row count={3}",Total,count2-count1,dtImport.Rows.Count,count2);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("BulkCopy Error:{0}", ex.Message);

                    //DataView dv = dtImport.DefaultView;
                    //DataTable dtNew = dv.ToTable(true, new string[] { "Id" });
                    //Console.WriteLine("District:{0}->{1}", dtImport.Rows.Count, dtNew.Rows.Count);
                }
                finally
                {
                    // Close the SqlDataReader. The SqlBulkCopy
                    // object is automatically closed at the end of the using block.
                }
            }
            // Perform a final count on the destination 
            // table to see how many rows were added.
        }
        /// <summary>
        /// 订阅进度报告事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.WriteLine("[{2}]当前进度：{0}/{1}", e.RowsCopied,CurrentNum,sw.Elapsed);
        }
        /// <summary>
        /// 从好友列表导入数据
        /// </summary>
        /// <param name="path"></param>
        private void ReadActorFromFile(IDictionary<long, Actor> dicMain, string path)
        {
            string content = File.ReadAllText(path);
            content = Regex.Unescape(content);
            if (content.Length == 0) return;

            JavaScriptObject root = JavaScriptConvert.DeserializeObject(content) as JavaScriptObject;
            foreach (string key in root.Keys)
            {
                JavaScriptObject item = root[key] as JavaScriptObject;
                Actor actor = manager.NewActor(key);
                actor.Name = item["nickName"].ToString();
                actor.District = int.Parse(item["city"].ToString());
                actor.Sex = int.Parse(item["sex"].ToString());
                actor.Age = int.Parse(item["age"].ToString());
                actor.Photo = item["photo"].ToString();
                actor.Uin = key;
                actor.Title = 0;
                dicMain.Add(actor.Id, actor);
            }
        }
    }
}