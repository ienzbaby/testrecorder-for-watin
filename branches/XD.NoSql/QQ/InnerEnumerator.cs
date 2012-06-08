using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

namespace XD.QQ
{
    partial class FileDirectoryEnumerable
    {
        /// <summary>
        /// 文件和目录的遍历器
        /// </summary>
        /// <remarks>本对象为Win32API函数 FindFirstFile , FindNextFile 
        /// 和 FindClose 的一个包装
        /// 
        /// 以下代码演示使用了 FileDirectoryEnumerator 
        /// 
        /// FileDirectoryEnumerator e = new FileDirectoryEnumerator();
        /// e.SearchPath = @"c:\";
        /// e.Reset();
        /// e.ReturnStringType = true ;
        /// while (e.MoveNext())
        /// {
        ///     System.Console.WriteLine
        ///         ( e.LastAccessTime.ToString("yyyy-MM-dd HH:mm:ss")
        ///         + "   " + e.FileLength + "  \t" + e.Name );
        /// }
        /// e.Close();
        /// System.Console.ReadLine();
        /// 
        /// 编写 袁永福 （ http://www.xdesigner.cn ）2006-12-8</remarks>
        class InnerEnumerator : IEnumerator
        {
            #region 表示对象当前状态的数据和属性 **********************************

            /// <summary>
            /// 当前对象
            /// </summary>
            private object objCurrentObject = null;

            private bool bolIsEmpty = false;
            /// <summary>
            /// 该目录为空
            /// </summary>
            public bool IsEmpty
            {
                get { return bolIsEmpty; }
            }
            private int intSearchedCount = 0;
            /// <summary>
            /// 已找到的对象的个数
            /// </summary>
            public int SearchedCount
            {
                get { return intSearchedCount; }
            }
            private bool bolIsFile = true;
            /// <summary>
            /// 当前对象是否为文件,若为true则当前对象为文件,否则为目录
            /// </summary>
            public bool IsFile
            {
                get { return bolIsFile; }
            }
            private int intLastErrorCode = 0;
            /// <summary>
            /// 最后一次操作的Win32错误代码
            /// </summary>
            public int LastErrorCode
            {
                get { return intLastErrorCode; }
            }
            /// <summary>
            /// 当前对象的名称
            /// </summary>
            public string Name
            {
                get
                {
                    if (this.objCurrentObject != null)
                    {
                        if (objCurrentObject is string)
                            return (string)this.objCurrentObject;
                        else
                            return ((FileSystemInfo)this.objCurrentObject).Name;
                    }
                    return null;
                }
            }
            /// <summary>
            /// 当前对象属性
            /// </summary>
            public FileAttributes Attributes
            {
                get { return (FileAttributes)myData.dwFileAttributes; }
            }
            /// <summary>
            /// 当前对象创建时间
            /// </summary>
            public System.DateTime CreationTime
            {
                get
                {
                    long time = ToLong(myData.ftCreationTime_dwHighDateTime, myData.ftCreationTime_dwLowDateTime);
                    System.DateTime dtm = System.DateTime.FromFileTimeUtc(time);
                    return dtm.ToLocalTime();
                }
            }
            /// <summary>
            /// 当前对象最后访问时间
            /// </summary>
            public System.DateTime LastAccessTime
            {
                get
                {
                    long time = ToLong(myData.ftLastAccessTime_dwHighDateTime, myData.ftLastAccessTime_dwLowDateTime);
                    System.DateTime dtm = System.DateTime.FromFileTimeUtc(time);
                    return dtm.ToLocalTime();
                }
            }
            /// <summary>
            /// 当前对象最后保存时间
            /// </summary>
            public System.DateTime LastWriteTime
            {
                get
                {
                    long time = ToLong(myData.ftLastWriteTime_dwHighDateTime, myData.ftLastWriteTime_dwLowDateTime);
                    System.DateTime dtm = System.DateTime.FromFileTimeUtc(time);
                    return dtm.ToLocalTime();
                }
            }
            /// <summary>
            /// 当前文件长度,若为当前对象为文件则返回文件长度,若当前对象为目录则返回0
            /// </summary>
            public long FileLength
            {
                get
                {
                    if (this.bolIsFile)
                        return ToLong(myData.nFileSizeHigh, myData.nFileSizeLow);
                    else
                        return 0;
                }
            }

            #endregion

            #region 控制对象特性的一些属性 ****************************************

            private bool bolThrowIOException = true;
            /// <summary>
            /// 发生IO错误时是否抛出异常
            /// </summary>
            public bool ThrowIOException
            {
                get { return this.bolThrowIOException; }
                set { this.bolThrowIOException = value; }
            }
            private bool bolReturnStringType = true;
            /// <summary>
            /// 是否以字符串方式返回查询结果,若返回true则当前对象返回为字符串,
            /// 否则返回 FileInfo或DirectoryInfo类型
            /// </summary>
            public bool ReturnStringType
            {
                get { return bolReturnStringType; }
                set { bolReturnStringType = value; }
            }

            private string strSearchPattern = "*";
            /// <summary>
            /// 要匹配的文件或目录名,支持通配符
            /// </summary>
            public string SearchPattern
            {
                get { return strSearchPattern; }
                set { strSearchPattern = value; }
            }
            private string strSearchPath = null;
            /// <summary>
            /// 搜索的父目录,必须为绝对路径,不得有通配符,该目录必须存在
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

            #endregion

            /// <summary>
            /// 关闭对象,停止搜索
            /// </summary>
            public void Close()
            {
                this.CloseHandler();
            }

            #region IEnumerator 成员 **********************************************

            /// <summary>
            /// 返回当前对象
            /// </summary>
            public object Current
            {
                get { return objCurrentObject; }
            }
            /// <summary>
            /// 找到下一个文件或目录
            /// </summary>
            /// <returns>操作是否成功</returns>
            public bool MoveNext()
            {
                bool success = false;
                while (true)
                {
                    if (this.bolStartSearchFlag)
                        success = this.SearchNext();
                    else
                        success = this.StartSearch();
                    if (success)
                    {
                        if (this.UpdateCurrentObject())
                            return true;
                    }
                    else
                    {
                        this.objCurrentObject = null;
                        return false;
                    }
                }
            }

            /// <summary>
            /// 重新设置对象
            /// </summary>
            public void Reset()
            {
                if (this.strSearchPath == null)
                    throw new System.ArgumentNullException("SearchPath can not null");
                if (this.strSearchPattern == null || this.strSearchPattern.Length == 0)
                    this.strSearchPattern = "*";

                this.intSearchedCount = 0;
                this.objCurrentObject = null;
                this.CloseHandler();
                this.bolStartSearchFlag = false;
                this.bolIsEmpty = false;
                this.intLastErrorCode = 0;
            }

            #endregion

            #region 声明WIN32API函数以及结构 **************************************

            [System.Runtime.InteropServices.DllImport
                ("kernel32.dll",
                CharSet = System.Runtime.InteropServices.CharSet.Auto,
                SetLastError = true)]
            private static extern IntPtr FindFirstFile(string pFileName, ref WIN32_FIND_DATA pFindFileData);

            [System.Runtime.InteropServices.DllImport
                ("kernel32.dll",
               CharSet = System.Runtime.InteropServices.CharSet.Auto,
                SetLastError = true)]
            private static extern bool FindNextFile(IntPtr hndFindFile, ref WIN32_FIND_DATA lpFindFileData);

            [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
            private static extern bool FindClose(IntPtr hndFindFile);

            private static long ToLong(int height, int low)
            {
                long v = (uint)height;
                v = v << 0x20;
                v = v | ((uint)low);
                return v;
            }

            private static void WinIOError(int errorCode, string str)
            {
                switch (errorCode)
                {
                    case 80:
                        throw new IOException("IO_FileExists :" + str);
                    case 0x57:
                        throw new IOException("IOError:" + MakeHRFromErrorCode(errorCode));
                    case 0xce:
                        throw new PathTooLongException("PathTooLong:" + str);
                    case 2:
                        throw new FileNotFoundException("FileNotFound:" + str);
                    case 3:
                        throw new DirectoryNotFoundException("PathNotFound:" + str);
                    case 5:
                        throw new UnauthorizedAccessException("UnauthorizedAccess:" + str);
                    case 0x20:
                        throw new IOException("IO_SharingViolation:" + str);
                }
                throw new IOException("IOError:" + MakeHRFromErrorCode(errorCode));
            }

            private static int MakeHRFromErrorCode(int errorCode)
            {
                return (-2147024896 | errorCode);
            }

            #endregion

            #region 内部代码群 ****************************************************

            private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
            /// <summary>
            /// 查找处理的底层句柄
            /// </summary>
            private System.IntPtr intSearchHandler = INVALID_HANDLE_VALUE;

            private WIN32_FIND_DATA myData = new WIN32_FIND_DATA();
            /// <summary>
            /// 开始搜索标志
            /// </summary>
            private bool bolStartSearchFlag = false;
            /// <summary>
            /// 关闭内部句柄
            /// </summary>
            private void CloseHandler()
            {
                if (this.intSearchHandler != INVALID_HANDLE_VALUE)
                {
                    FindClose(this.intSearchHandler);
                    this.intSearchHandler = INVALID_HANDLE_VALUE;
                }
            }
            /// <summary>
            /// 开始搜索
            /// </summary>
            /// <returns>操作是否成功</returns>
            private bool StartSearch()
            {
                bolStartSearchFlag = true;
                bolIsEmpty = false;
                objCurrentObject = null;
                intLastErrorCode = 0;

                string strPath = Path.Combine(strSearchPath, this.strSearchPattern);
                this.CloseHandler();
                intSearchHandler = FindFirstFile(strPath, ref myData);
                if (intSearchHandler == INVALID_HANDLE_VALUE)
                {
                    intLastErrorCode = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    if (intLastErrorCode == 2)
                    {
                        bolIsEmpty = true;
                        return false;
                    }
                    if (this.bolThrowIOException)
                        WinIOError(intLastErrorCode, strSearchPath);
                    else
                        return false;
                }
                return true;
            }
            /// <summary>
            /// 搜索下一个
            /// </summary>
            /// <returns>操作是否成功</returns>
            private bool SearchNext()
            {
                if (bolStartSearchFlag == false)
                    return false;
                if (bolIsEmpty)
                    return false;
                if (intSearchHandler == INVALID_HANDLE_VALUE)
                    return false;
                intLastErrorCode = 0;
                if (FindNextFile(intSearchHandler, ref myData) == false)
                {
                    intLastErrorCode = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                    this.CloseHandler();
                    if (intLastErrorCode != 0 && intLastErrorCode != 0x12)
                    {
                        if (this.bolThrowIOException)
                            WinIOError(intLastErrorCode, strSearchPath);
                        else
                            return false;
                    }
                    return false;
                }
                return true;
            }//private bool SearchNext()

            /// <summary>
            /// 更新当前对象
            /// </summary>
            /// <returns>操作是否成功</returns>
            private bool UpdateCurrentObject()
            {
                if (intSearchHandler == INVALID_HANDLE_VALUE)
                    return false;
                bool Result = false;
                this.objCurrentObject = null;
                if ((myData.dwFileAttributes & 0x10) == 0)
                {
                    // 当前对象为文件
                    this.bolIsFile = true;
                    if (this.bolSearchForFile)
                        Result = true;
                }
                else
                {
                    // 当前对象为目录
                    this.bolIsFile = false;
                    if (this.bolSearchForDirectory)
                    {
                        if (myData.cFileName == "." || myData.cFileName == "..")
                            Result = false;
                        else
                            Result = true;
                    }
                }
                if (Result)
                {
                    if (this.bolReturnStringType)
                        this.objCurrentObject = myData.cFileName;
                    else
                    {
                        string p = Path.Combine(this.strSearchPath, myData.cFileName);
                        if (this.bolIsFile)
                        {
                            this.objCurrentObject = new FileInfo(p);
                        }
                        else
                        {
                            this.objCurrentObject = new DirectoryInfo(p);
                        }
                    }
                    this.intSearchedCount++;
                }
                return Result;
            }//private bool UpdateCurrentObject()

            #endregion

        }
    }
}