using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace XD.QQ
{
    /// <summary>
    /// 文件或目录遍历器,本类型为 FileDirectoryEnumerator 的一个包装
    /// </summary>
    /// <remarks>
    /// 
    /// 编写 袁永福 （ http://www.xdesigner.cn ）2006-12-8
    /// 
    /// 以下代码演示使用这个文件目录遍历器
    /// 
    /// FileDirectoryEnumerable e = new FileDirectoryEnumerable();
    /// e.SearchPath = @"c:\";
    /// e.ReturnStringType = true ;
    /// e.SearchPattern = "*.exe";
    /// e.SearchDirectory = false ;
    /// e.SearchFile = true;
    /// foreach (object name in e)
    /// {
    ///     System.Console.WriteLine(name);
    /// }
    /// System.Console.ReadLine();
    /// 
    partial class FileDirectoryEnumerable :IEnumerable
    {
        private bool bolReturnStringType = true;
        /// <summary>
        /// 是否以字符串方式返回查询结果,若返回true则当前对象返回为字符串,
        /// 否则返回 System.IO.FileInfo或System.IO.DirectoryInfo类型
        /// </summary>
        public bool ReturnStringType
        {
            get { return bolReturnStringType; }
            set { bolReturnStringType = value; }
        }

        private string strSearchPattern = "*";
        /// <summary>
        /// 文件或目录名的通配符
        /// </summary>
        public string SearchPattern
        {
            get { return strSearchPattern; }
            set { strSearchPattern = value; }
        }
        private string strSearchPath = null;
        /// <summary>
        /// 搜索路径,必须为绝对路径
        /// </summary>
        public string SearchPath
        {
            get { return strSearchPath; }
            set { strSearchPath = value; }
        }

        private bool bolSearchForFile = true;
        /// <summary>
        /// 是否查找文件
        /// </summary>
        public bool SearchForFile
        {
            get { return bolSearchForFile; }
            set { bolSearchForFile = value; }
        }
        private bool bolSearchForDirectory = true;
        /// <summary>
        /// 是否查找子目录
        /// </summary>
        public bool SearchForDirectory
        {
            get { return bolSearchForDirectory; }
            set { bolSearchForDirectory = value; }
        }

        private bool bolThrowIOException = true;
        /// <summary>
        /// 发生IO错误时是否抛出异常
        /// </summary>
        public bool ThrowIOException
        {
            get { return this.bolThrowIOException; }
            set { this.bolThrowIOException = value; }
        }
        /// <summary>
        /// 返回内置的文件和目录遍历器
        /// </summary>
        /// <returns>遍历器对象</returns>
        public IEnumerator GetEnumerator()
        {
            InnerEnumerator e = new InnerEnumerator();
            e.ReturnStringType = this.bolReturnStringType;
            e.SearchForDirectory = this.bolSearchForDirectory;
            e.SearchForFile = this.bolSearchForFile;
            e.SearchPath = this.strSearchPath;
            e.SearchPattern = this.strSearchPattern;
            e.ThrowIOException = this.bolThrowIOException;
            myList.Add(e);
            return e;
        }
        /// <summary>
        /// 关闭对象
        /// </summary>
        public void Close()
        {
            foreach (InnerEnumerator e in myList)
            {
                e.Close();
            }
            myList.Clear();
        }

        private System.Collections.ArrayList myList = new System.Collections.ArrayList();
    }
}
