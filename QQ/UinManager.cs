using System.Collections.Generic;
using System.Data;
using log4net;
using XD.Tools;
using XD.Tools.DBUtility;
using System;
using System.Text;
using System.Threading;

namespace XD.QQ
{
    /// <summary>
    /// 管理类
    /// </summary>
    public sealed class UinManager:AbstractSingleton<UinManager>
    {
        private IDataService dal = DataFactory.Instance();
        private ILog log = null;
        private long LockNum = 0;
        private readonly string SQL_UNUSED = @"
declare @max bigint declare @min bigint
select @max=max(id),@min=min(id) from( select top 20 * from QQ_Uin_Cache)sss
select Id from QQ_Uin_Cache where Id between @min and @max
delete QQ_Uin_Cache where Id between @min and @max
";
        private readonly string SQL_INSERT = @"
declare @max bigint declare @min bigint
select @min=min(id),@max=max(id) from 
(select top 100000 * from QQ_Uin where state=0order by id asc)xxx
insert QQ_Uin_Cache select id from QQ_Uin where id between @min  and  @max and state=0
update QQ_Uin set state=1                 where id between @min  and  @max and state=0
select count(*) as cnt from QQ_Uin_Cache";

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
            //DataTable dt = dal.ExecuteSql("select top 1 id  from QQ_Uin where state=0 order by id asc").Tables[0];
            //if (dt.Rows.Count > 0) this.Curror = long.Parse(dt.Rows[0][0].ToString());
        }
        /// <summary>
        /// 生成缓存,随着数据量的增大，需要调整检索的步伐
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private IList<string> GetUnUsedFrmoCache(int num)
        {
            this.InitCurror();//初始化游标
            if (Interlocked.Read(ref LockNum) == 0)
            {
                Interlocked.Exchange(ref LockNum, 1);
                DataTable dt = dal.ExecuteSql(SQL_INSERT).Tables[0];
                Interlocked.Exchange(ref LockNum, 0);
            }
            return new List<string>();
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
            return DataFactory.ExecuteSql("select top "+pageSize+" * from QQ_Uin");
        }
    }
}
