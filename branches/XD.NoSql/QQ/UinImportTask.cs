﻿using System;
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
    public class UinImportTask : ITask
    {
        private int Total = 0; //数量
        private UinManager manager = UinManager.Instance();
        private string SearchPath = @"E:\nodejs\Data";
        private DataSet dsTemplate;
        private string ConnStr = ConfigurationManager.AppSettings["ConnectionString"];
        private int PerBatchSize = 1000;
        private int MaxBatchSize = 5000;
        private Stopwatch sw = new Stopwatch();
        private ILog log = LogManager.GetLogger(typeof(UinImportTask));
        
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
            if (dsTemplate == null)
            {
                dsTemplate = new DataSet();
                for (int i = 0; i < 100; i++)
                {
                    DataTable dt = new DataTable("QQ_Uin_" + i);
                    dt.Columns.Add("id", typeof(long));
                    dt.Columns.Add("name", typeof(string));
                    dt.Columns.Add("district", typeof(int));
                    dt.Columns.Add("sex", typeof(int));
                    dt.Columns.Add("age", typeof(int)); 
                    dt.Columns.Add("title", typeof(int));
                    dt.Columns.Add("state", typeof(int));
                    dsTemplate.Tables.Add(dt);
                }
            }
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

                this.SqlBulkFromDataSet(MaxBatchSize);
            }
            this.SqlBulkFromDataSet(0);
        }
        private void SqlBulkFromDataSet(int num)
        {
            int inserts = 0;
            int count1 = manager.Count();

            foreach (DataTable dt in dsTemplate.Tables)
            {
                if (dt.Rows.Count > num)
                {
                    inserts += dt.Rows.Count;
                    this.SqlBulkFromDataTable(dt);
                }
            }

            if (inserts > 0)
            {
                int count2 = manager.Count();
                Total += count2 - count1;
                log.WarnFormat("total insert={0},current insert={1}/{2},row count={3}", Total, count2 - count1, inserts, count2);
            }
        }
        /// <summary>
        /// 批量导入数据
        /// </summary>
        /// <param name="dtImport"></param>
        private void SqlBulkFromDataTable(DataTable dtTemplate)
        {
            // Create the SqlBulkCopy object using a connection string. 
            // In the real world you would not use SqlBulkCopy to move
            // data from one table to the other in the same database.
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(ConnStr, SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.TableLock))
            {
                bulkCopy.DestinationTableName = dtTemplate.TableName;
                bulkCopy.BatchSize = PerBatchSize;
                bulkCopy.BulkCopyTimeout = 60000;
                bulkCopy.NotifyAfter = PerBatchSize;

                // Set up the event handler to notify after 50 rows.
                bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);
                
                try
                {
                    bulkCopy.WriteToServer(dtTemplate);
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
            SqlBulkCopy obj = sender as SqlBulkCopy;
            log.WarnFormat("[{0}]当前进度：tablename={1},insert+={2}", sw.Elapsed,obj.DestinationTableName, e.RowsCopied);
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
            foreach (string key in root.Keys)
            {
                JavaScriptObject item = root[key] as JavaScriptObject;
                long id = long.Parse(item["uin"].ToString());
                DataTable dt = dsTemplate.Tables[this.GetTableName(id)];
                DataRow dr = dt.NewRow();
                
                dr["id"] =id;
                dr["state"] = 0;
                dr["title"] = item["time"];

                var name = item["name"].ToString();
                if (name.Length > 50) name = name.Substring(0, 50);
                dr["name"] = name;//========截断长名称=======

                dt.Rows.Add(dr);
            }
        }

        private string GetTableName(long id)
        {
            long offset = 30000000;
            if (id < offset)
                return "QQ_Uin_0";
            else if (id > offset * 100)
                return "QQ_Uin_99";
            else
                return "QQ_Uin_" + (id / offset);
        }
    }
}