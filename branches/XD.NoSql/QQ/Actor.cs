using System;
using System.Collections.Generic;
using System.Text;

namespace XD.QQ
{
    public class Actor
    {
        private long _id;
        private string _uin;
        private string _name;
        private int _district;
        private int _sex;
        private int _age;
        private string _photo;
        private int _title;
        private int _state;
        /// <summary>
        /// Id
        /// </summary>
        public long Id
        {
            get { return _id; }
            internal set { _id = value; }
        }
        /// <summary>
        /// 加密Id
        /// </summary>
        public string Uin
        {
            get { return _uin; }
            internal set { _uin = value; }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// 图像
        /// </summary>
        public string Photo
        {
            get { return _photo; }
            set { _photo = value; }
        }
        /// <summary>
        /// 区域
        /// </summary>
        public int District
        {
            get { return _district; }
            set { _district = value; }
        }
        /// <summary>
        /// 姓名
        /// </summary>
        public int Sex
        {
            get { return _sex ; }
            set { _sex = value; }
        }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }
        /// <summary>
        /// 达人头衔
        /// </summary>
        public int Title
        {
            get { return _title; }
            set { _title = value; }
        }
        /// <summary>
        /// 状态位
        /// </summary>
        private int State
        {
            get { return _state; }
            set { _state = value; }
        }
        /// <summary>
        /// 重写Equals函数
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == this) return true;

            Actor actor = obj as Actor;
            return actor != null && actor.Id == this.Id;
        }
        /// <summary>
        /// Hash编码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.Uin.GetHashCode();
        }
    }
}
