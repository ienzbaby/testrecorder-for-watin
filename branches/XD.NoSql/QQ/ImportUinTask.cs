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
    public class ImportUinTask : ITask
    {
        public int Total = 0; //数量

        private UinManager manager = UinManager.Instance();
        private string SearchPath = @"E:\nodejs\Data";
        private DataTable dtTemplate;
        private long CurrentNum = 0;
        private string ConnStr = ConfigurationManager.AppSettings["ConnectionString"];
        private int PerBatchSize = 1000;
        private int MaxBatchSize = 10000;
        private Stopwatch sw = new Stopwatch();
        private ILog log = LogManager.GetLogger(typeof(ImportUinTask));
        
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
                dtTemplate.Columns.Add("id", typeof(long));
                dtTemplate.Columns.Add("name", typeof(string));
                dtTemplate.Columns.Add("visit", typeof(int));
                dtTemplate.Columns.Add("state", typeof(int));
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
                try
                {
                    this.ReadActorFromFile(path);
                    File.Delete(path);
                }
                catch (Exception err)
                {
                    log.Error("File [" + path + "] Read Error", err);
                }

                //数据超过一定的阀值，则导入数据
                if (dtTemplate.Rows.Count > MaxBatchSize)
                    this.SqlBulkFromDataTable(dtTemplate, "QQ_Uin");
            }
            this.SqlBulkFromDataTable(dtTemplate, "QQ_Uin");
        }
        /// <summary>
        /// 批量导入数据
        /// </summary>
        /// <param name="dtImport"></param>
        private void SqlBulkFromDataTable(DataTable dtImport, string tableName)
        {
            CurrentNum = dtImport.Rows.Count;
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
                    bulkCopy.WriteToServer(dtImport);
                    int count2 = manager.Count();

                    Total += count2 - count1;
                    log.WarnFormat("total add={0},current add={1}/{2},row count={3}", Total, count2 - count1, dtImport.Rows.Count, count2);
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
        private void ReadActorFromFile(string path)
        {
            string content = File.ReadAllText(path);
            content = Regex.Unescape(content);
            if (content.Length == 0) return;

            JavaScriptObject root = JavaScriptConvert.DeserializeObject(content) as JavaScriptObject;
            if (root != null && root.ContainsKey("items") && root.ContainsKey("modvisitcount"))
            {
                JavaScriptArray mods = root["modvisitcount"] as JavaScriptArray;
                if (mods != null && mods.Count > 0)
                {
                    var visit=(mods[0] as JavaScriptObject)["totalcount"];
                    foreach (JavaScriptObject item in root["items"] as JavaScriptArray)
                    {
                        DataRow dr = dtTemplate.NewRow();
                        dr["id"] = item["uin"];
                        dr["name"] = item["name"];
                        dr["state"] = 0;
                        dr["visit"] = visit;
                        dtTemplate.Rows.Add(dr);
                    }
                }
            }
            else
            {
                File.WriteAllText(path.Replace("data", "error"), content);
            }
        }
    }
}