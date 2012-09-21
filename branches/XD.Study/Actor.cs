using System;
using System.Collections.Generic;
using System.Text;

namespace XD.Study
{
    /// <summary>
    /// 人员
    /// </summary>
    public class Actor
    {
        private int _id;
        private string _password;
        private string _name;
        /// <summary>
        /// Id
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
    }
}
