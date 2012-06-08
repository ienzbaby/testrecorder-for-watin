using System;
using System.Collections.Generic;
using System.Text;

namespace XD.QQ
{
    partial class FileDirectoryEnumerable
    {
        [Serializable,
        System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Sequential,
            CharSet = System.Runtime.InteropServices.CharSet.Auto
            ),
        System.Runtime.InteropServices.BestFitMapping(false)]
        private struct WIN32_FIND_DATA
        {
            public int dwFileAttributes;
            public int ftCreationTime_dwLowDateTime;
            public int ftCreationTime_dwHighDateTime;
            public int ftLastAccessTime_dwLowDateTime;
            public int ftLastAccessTime_dwHighDateTime;
            public int ftLastWriteTime_dwLowDateTime;
            public int ftLastWriteTime_dwHighDateTime;
            public int nFileSizeHigh;
            public int nFileSizeLow;
            public int dwReserved0;
            public int dwReserved1;
            [System.Runtime.InteropServices.MarshalAs
                (System.Runtime.InteropServices.UnmanagedType.ByValTStr,
                SizeConst = 260)]
            public string cFileName;
            [System.Runtime.InteropServices.MarshalAs
                (System.Runtime.InteropServices.UnmanagedType.ByValTStr,
                SizeConst = 14)]
            public string cAlternateFileName;
        }

    }
}
