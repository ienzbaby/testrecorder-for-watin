using System;
using System.Collections.Generic;
using System.Text;

namespace TestRecorder.Tools
{
    /// <summary>
    /// 访问页面
    /// </summary>
    public sealed class VisitPage
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="url"></param>
        public VisitPage(string url)
        {
            this.Page = url;
        }
        private string _name;
        private long _count;
        private long _times;
        private DateTime _lastvisittime=DateTime.MinValue;
        private DateTime _lastloadtime = DateTime.MinValue;

        /// <summary>
        /// 名称
        /// </summary>
        public string Page
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// 访问次数
        /// </summary>
        public long Count
        {
            get { return _count; }
            set { _count = value; }
        }
        /// <summary>
        /// 总耗时
        /// </summary>
        public long Times
        {
            get { return _times; }
            set { _times = value; }
        }
        /// <summary>
        /// 平均耗时
        /// </summary>
        public string AvgTime
        {
            get
            {
                if (Count == 0)
                {
                    return "--";
                }
                else
                {
                    return (Times/(Count*1000+0.0)).ToString("F1");
                }
            }
        }
        /// <summary>
        /// 最后访问时间
        /// </summary>
        public DateTime LastStartTime
        {
            get { return _lastvisittime; }
            set { _lastvisittime = value; }
        }
        /// <summary>
        /// 最后加载完成时间
        /// </summary>
        public DateTime LastEndTime
        {
            get { return _lastloadtime; }
            set { _lastloadtime = value; }
        }
    }
}
