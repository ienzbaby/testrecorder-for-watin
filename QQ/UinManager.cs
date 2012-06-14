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
    public sealed class UinManager:AbstractSingleton<UinManager>
    {
        private IDataService dal = DataFactory.Instance();
        private ILog log = null;
        private long Curror = 0;//初始游标
        
        private UinManager()
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
        /// 取得行数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            DataTable dt = dal.ExecuteSql("sp_spaceused [qq_uin]").Tables[0];
            return int.Parse(dt.Rows[0]["rows"].ToString());
        }
        private readonly string SQL_UNUSED = @"
declare @max bigint declare @min bigint
select @max=max(id),@min=min(id) from( select top 20 * from QQ_Uin_Cache)sss
select Id from QQ_Uin_Cache where Id between @min and @max
delete QQ_Uin_Cache where Id between @min and @max
";
        /// <summary>
        /// 取得未使用的Actor的Uin
        /// </summary>
        /// <returns></returns>
        public IList<string> GetUnUsed(int num)
        {
            if (num < 0) num = 0;
            if (num > 10) num = 10;

            DataTable dt = dal.ExecuteSql(SQL_UNUSED).Tables[0];
            if (dt.Rows.Count == 0) return this.GetUnUsedFrmoCache(num);

            IList<string> list = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(dr["Id"].ToString());
            }
            return list;
        }
        private void InitCurror()
        {
            if (this.Curror == 0)
            {
                DataTable dt = dal.ExecuteSql("select top 1 id  from QQ_Uin where state=0 order by id asc").Tables[0];
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

            string sql = string.Format(@"
                insert QQ_Uin_Cache 
                select id from QQ_Uin where id between {0}  and  {1} and state=0
                update QQ_Uin set state=1 where id between {0}  and  {1} and state=0
                select count(*) as cnt from QQ_Cache",
            Curror, int.MaxValue);

            DataTable dt = dal.ExecuteSql(sql).Tables[0];
            long nums = long.Parse(dt.Rows[0][0].ToString());

            Curror += offset;

            return this.GetUnUsed(num);
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
            return DataFactory.GetRecordByRowNumber("QQ_Uin", "*", pageSize, pageIndex,0, "order by Id desc", strWhere);
        }
    }
}
