using System;
using System.Collections.Generic;
using System.Text;
using XD.Tools;
using System.Web;
using Newtonsoft.Json;
using System.Data;

namespace XD.QQ
{
    public class JsonServices:JsonHelperBase
    {
        private static ActorManager manager = ActorManager.Instance();
        private static UinManager uin = UinManager.Instance();
        /// <summary>
        /// 处理上下文
        /// </summary>
        /// <param name="context"></param>
        public static void ProcessRequest(HttpContext context)
        {
            string mod = GetSecurityParam(context, "mod", "");
            if (mod == "actor")
            {
                string act = GetSecurityParam(context, "act", "");
                if (act == "getunused")
                {
                    string num = GetSecurityParam(context, "num", "1");
                    IList<string> list = manager.GetUnUsed(int.Parse(num));

                    JavaScriptObject root = new JavaScriptObject();
                    root.Add("errno", "0");
                    root.Add("items", DeserializeArray(list));
                    context.Response.Write(JavaScriptConvert.SerializeObject(root));
                }
                else if (act == "rows")
                {
                    JavaScriptObject root = new JavaScriptObject();
                    root.Add("errno", "0");
                    root.Add("item", manager.Count());
                    context.Response.Write(JavaScriptConvert.SerializeObject(root));
                }
                else if (act == "exist")
                {
                    string uins = GetSecurityParam(context, "uins", "");
                    IDictionary<long, string> dics = new Dictionary<long, string>();

                    foreach (string uin in uins.Split(new char[] { ',' }))
                    {
                        if (string.IsNullOrEmpty(uin) == false)
                        {
                            long key = manager.GetUniqueCode(uin);
                            if (dics.ContainsKey(key) == false)
                            {
                                dics.Add(key, uin);
                            }
                        }
                    }

                    JavaScriptArray items = new JavaScriptArray();
                    if (dics.Count > 0)
                    {
                        IList<long> list = manager.Exists(dics.Keys);
                        foreach (var p in dics)
                        {
                            JavaScriptObject item = new JavaScriptObject();
                            item.Add("uin", p.Value);
                            item.Add("exist", list.Contains(p.Key).ToString().ToLower());

                            items.Add(item);
                        }
                    }

                    JavaScriptObject root = new JavaScriptObject();
                    root.Add("errno", "0");
                    root.Add("items", items);
                    context.Response.Write(JavaScriptConvert.SerializeObject(root));
                }
                else
                {
                    throw new NotImplementedException(string.Format("未实现的接口mod=qq,act={0}", act));
                }
            }
            else if (mod == "uin")
            {
                string act = GetSecurityParam(context, "act", "");
                if (act == "getunused")
                {
                    string num = GetSecurityParam(context, "num", "1");
                    IList<string> list = uin.GetUnUsed(int.Parse(num));

                    JavaScriptObject root = new JavaScriptObject();
                    root.Add("errno", "0");
                    root.Add("items", DeserializeArray(list));
                    context.Response.Write(JavaScriptConvert.SerializeObject(root));
                }
                else if (act == "rows")
                {
                    JavaScriptObject root = new JavaScriptObject();
                    root.Add("errno", "0");
                    root.Add("item", uin.Count());
                    context.Response.Write(JavaScriptConvert.SerializeObject(root));
                }
                else if (act == "getlist")
                {
                    string where = "1=1 and name!=''";
                    string id = GetSecurityParam(context, "id", "");
                    if (id.Length > 0) where += string.Format(" and Id='{0}'", id);
                    DataTable dt = uin.GetRecordByRowNumber(50, 1, where).Tables[0];

                    JavaScriptObject root = new JavaScriptObject();
                    root.Add("errno", "0");
                    root.Add("items", DeserializeArray(dt.Rows));
                    context.Response.Write(JavaScriptConvert.SerializeObject(root));
                }
            }
            else
                throw new NotImplementedException(string.Format("没有实现的接口[mod='{0}']", mod));
            context.Response.End();
        }
        /// <summary>
        /// 自定义处理
        /// </summary>
        /// <param name="strRequest"></param>
        /// <returns></returns>
        public static string ProcessRequest(string strRequest)
        {
            return ProcessRequest(strRequest, ProcessRequest);
        }
    }
}
