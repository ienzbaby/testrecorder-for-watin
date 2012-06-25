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
using System.Configuration;
using log4net;

namespace XD.QQ
{
    /// <summary>
    /// 通过字典缓存，提高执行效率
    /// </summary>
    public class CardImportTask : ITask
    {
        private int Total = 0; //数量
        private UinManager manager = UinManager.Instance();
        private string SearchPath = @"E:\nodejs\Data";
        private DataTable dtTemplate;
        private long CurrentNum = 0;
        private string ConnStr = ConfigurationManager.AppSettings["ConnectionString"];
        private int PerBatchSize = 1000;
        private int MaxBatchSize = 10000;
        private Stopwatch sw = new Stopwatch();
        private ILog log = LogManager.GetLogger(typeof(CardImportTask));
        
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
        private void Init()
        {
            //======初始化======
            CurrentNum = 0;

            if (dtTemplate == null)
            {
                dtTemplate = new DataTable();
                dtTemplate.Columns.Add("Id", typeof(long));
                dtTemplate.Columns.Add("Name", typeof(string));
                dtTemplate.Columns.Add("District", typeof(int));
                dtTemplate.Columns.Add("Sex", typeof(int));
                dtTemplate.Columns.Add("Age", typeof(int));
                dtTemplate.Columns.Add("Qzone", typeof(int));
                dtTemplate.Columns.Add("Viplevel", typeof(int));
                dtTemplate.Columns.Add("Score", typeof(int));
                dtTemplate.Columns.Add("Title", typeof(string)); 
                dtTemplate.Columns.Add("State", typeof(int));
            }
            dtTemplate.Clear();
            sw.Start();

        }
        public void Execute(XmlElement xElement)
        {
            if (xElement != null && xElement.Attributes["path"] != null)//=====读取路径===
                this.SearchPath = xElement.Attributes["path"].Value;
            this.Init();

            foreach (string name in GetFiles())
            {
                string path = SearchPath + @"\" + name;
                string content=File.ReadAllText(path);
                try
                {
                    this.ReadActorFromFile(content);
                    File.Delete(path);
                }
                catch (Exception err)
                {
                    File.WriteAllText(path.Replace("data","error"),content);
                    log.Error("File [" + path + "] Read Error", err);
                }

                //数据超过一定的阀值，则导入数据
                if (dtTemplate.Rows.Count > MaxBatchSize)
                    this.SqlBulkFromDataTable("QQ_Uin");
            }
            this.SqlBulkFromDataTable("QQ_Uin");
        }
        /// <summary>
        /// 批量导入数据
        /// </summary>
        /// <param name="dtImport"></param>
        private void SqlBulkFromDataTable(string tableName)
        {
            CurrentNum = dtTemplate.Rows.Count;
            // Create the SqlBulkCopy object using a connection string. 
            // In the real world you would not use SqlBulkCopy to move
            // data from one table to the other in the same database.
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(ConnStr, SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.TableLock))
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
                    bulkCopy.WriteToServer(dtTemplate);
                    int count2 = manager.Count();

                    Total += count2 - count1;
                    log.WarnFormat("total add={0},current add={1}/{2},row count={3}", Total, count2 - count1,CurrentNum, count2);
                }
                catch (Exception ex)
                {
                    log.Error("BulkCopy Error", ex);
                }
                finally
                {
                    // Close the SqlDataReader. The SqlBulkCopy
                    // object is automatically closed at the end of the using block.
                }
            }
            dtTemplate.Clear();
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
            log.WarnFormat("[{2}]当前进度：{0}/{1}", e.RowsCopied,CurrentNum,sw.Elapsed);
        }
        /// <summary>
        /// 从好友列表导入数据
        /// </summary>
        /// <param name="path"></param>
        private void ReadActorFromFile(string content)
        {
            //content = Regex.Unescape(content);
            if (content.Length == 0) return;

            JavaScriptObject root = JavaScriptConvert.DeserializeObject(content) as JavaScriptObject;
            foreach (string key in root.Keys)
            {
                JavaScriptObject item = root[key] as JavaScriptObject;
                DataRow dr = dtTemplate.NewRow();
                dr["Id"] = item["uin"];
                dr["Name"] = "";
                dr["District"] = 0; 
                dr["Sex"] = 0;
                dr["Age"] = 0;
                dr["Qzone"] = 0;
                dr["VipLevel"] = 0;
                dr["Score"] = 0;
                dr["Title"] = "";
                dr["State"] = 0;

                if (item.ContainsKey("nickname"))
                {
                    var name = item["nickname"].ToString();
                    if (name.Length > 50) name = name.Substring(0, 50);
                    dr["name"] = name;
                }
                if (item.ContainsKey("qzone")) dr["qzone"] = item["qzone"];
                if (item.ContainsKey("viplevel")) dr["viplevel"] = item["viplevel"];
                if (item.ContainsKey("score")) dr["score"] = item["score"];
                if (item.ContainsKey("offsetBirth")) dr["age"] = item["offsetBirth"];
                if (item.ContainsKey("title")) dr["title"] = item["title"];
                
                dtTemplate.Rows.Add(dr);
            }
        }
    }
}