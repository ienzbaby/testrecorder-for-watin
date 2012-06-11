using System.Collections.Generic;
using System.Data;
using log4net;
using XD.Tools;
using XD.Tools.DBUtility;
using System;
using System.Text;

namespace XD.QQ
{
    /// <summary>
    /// 管理类
    /// </summary>
    public sealed class ActorManager:AbstractSingleton<ActorManager>
    {
        IDataService dal = DataFactory.Instance();
        ILog log=null;
        private long Curror = long.MinValue;//初始游标
        private ActorManager()
        {
            log = LogManager.GetLogger(this.GetType());
        }
        /// <summary>
        /// 取得实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Actor GetModel(int id)
        {
            return dal.GetModel<Actor>(id);
        }
        /// <summary>
        /// 生成一个角色实体，非系统角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public Actor NewActor(string uin)
        {
            Actor actor = new Actor();
            actor.Id = GetUniqueCode(uin);
            actor.Uin = uin;
            return actor;
        }
        /// <summary>
        /// 取得行数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            DataTable dt = dal.ExecuteSql("sp_spaceused [qq_actor]").Tables[0];
            return int.Parse(dt.Rows[0]["rows"].ToString());
        }
        /// <summary>
        /// 取得唯一编码
        /// </summary>
        /// <param name="uin"></param>
        /// <returns></returns>
        public long GetUniqueCode(string uin)
        {
            string md5 = Function.MD5Encrypt(uin);
            return uin.GetHashCode() * (long)Math.Pow(2, 32) + md5.GetHashCode();
        }
        /// <summary>
        /// 取得实体
        /// </summary>
        public Actor GetModel(string uin)
        {
            string hsql = string.Format("from Actor where Id='{0}'", this.GetUniqueCode(uin));

            IList<Actor> list = dal.GetObjectsByHsql<Actor>(hsql);
            return list.Count == 0 ? null : list[0];
        }
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Actor model)
        {
            int iReturn = 0;
            if (this.GetModel(model.Uin) == null)
            {
                iReturn = (int)dal.Add(model);
            }
            return iReturn;
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public void Update(Actor model)
        {
            dal.Update(model);
        }
        /// <summary>
        /// 取得未使用的Actor的Uin
        /// </summary>
        /// <returns></returns>
        public IList<string> GetUnUsed(int num)
        {
            if (num < 0) num = 0;
            if (num > 10) num = 10;

            DataTable dt = dal.ExecuteSql("exec XD_GetUnUsed").Tables[0];
            if (dt.Rows.Count == 0) return this.GetUnUsedFrmoCache(num);
            
            IList<string> list = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(dr["uin"].ToString());
            }
            return list;
        }
        private void InitCurror()
        {
            if (this.Curror == long.MinValue)
            {
                DataTable dt = dal.ExecuteSql("select top 1 id  from QQ_Actor where state=0").Tables[0];
                if (dt.Rows.Count > 0) this.Curror = long.Parse(dt.Rows[0][0].ToString());
            }
        }
        /// <summary>
        /// 生成缓存,随着数据量的增大，需要调整检索的步伐
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private IList<string> GetUnUsedFrmoCache(int num)
        {
            this.InitCurror();//初始化游标
            long offset = long.MaxValue / 1000;

            long caches = 0;
            while (caches <= 0)
            {
                if (Curror + offset > long.MaxValue) throw new OverflowException("超出长整形最大范围！");

                string sql = string.Format(@"insert QQ_Cache (Id,Uin) 
                select id,uin from QQ_Actor where id between {0}  and  {1} and state=0
                update QQ_Actor set state=1 where id between {0}  and  {1} and state=0
                select count(*) as cnt from QQ_Cache",
                Curror, Curror + offset);

                DataTable dt = dal.ExecuteSql(sql).Tables[0];
                caches = long.Parse(dt.Rows[0][0].ToString());
                Curror += offset;
            }
            return this.GetUnUsed(num);
        }
        /// <summary>
        /// 删除
        /// </summary>
        public void Delete(Actor model)
        {
            dal.Delete(model);
        }
        /// <summary>
        /// 是否存在指定Id
        /// </summary>
        /// <param name="ids">Ids，逗号分开</param>
        /// <returns></returns>
        public IList<long> Exists(ICollection<long> ids)
        {
            IList<long> list = new List<long>();
            if (ids.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (long id in ids) sb.Append(id + ",");

                string sql = string.Format("select id from QQ_Actor where Id in ({0})", sb.ToString(0, sb.Length - 1));
                DataTable dt = dal.ExecuteSql(sql).Tables[0];

                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(long.Parse(dr["Id"].ToString()));
                }
            }
            return list;
        }
        /// <summary>
        /// 取得分页数据
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="strOrder"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public DataSet GetRecordByRowNumber(int pageSize, int pageIndex,string strWhere)
        {
            return DataFactory.GetRecordByRowNumber("QQ_Actor", "*", pageSize, pageIndex,0, "order by Id desc", strWhere);
        }
    }
}
