using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using WatiN.Core;

namespace TestRecordUnit
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void FireEvent()
        {
            IE ie =IE.InternetExplorers()[0];
            ie.GoTo("http://192.168.1.104/st/Approval/System/Login.aspx");

            ie.Button("btnOk").FireEvent("ondblClick");
            
            //ie.Dispose();
        }
        [Test]
        public void MergeDll()
        {
            ILMerging.ILMerge merge = new ILMerging.ILMerge();
            merge.SetSearchDirectories(new string[] { @"D:\程序备份\ZQ_WaitN\lib" });
            merge.SetInputAssemblies(new string[] { 
                "WatiN.Core.dll", 
                "csExWB.dll",
                "DevAge.Core.dll",
                "DevAge.Windows.Forms.dll",
                "FileDlgExtenders.dll",
                "office.dll",
                "Interop.ComUtilitiesLib.dll",
                "Interop.SHDocVw.dll",
                //"Microsoft.mshtml.dll",
                "Microsoft.VisualBasic.dll",
                "Microsoft.Office.Interop.Excel.dll",
                "Microsoft.Vbe.Interop.dll",
                "Microsoft.Win32.Hooks.dll",
                "SourceGrid.dll"
            });
            merge.OutputFile = @"C:\WatiN.dll";
            merge.Merge();
        }
        [Test]
        public void WordToHtml()
        {
            string strSource =@"D:\我的文档\Downloads\iejoyswebos介绍及安装.doc";
            string strDestination=@"C:\hao.html";
            // Constant for WORD-TO-HTML exporting format
            const int WORD_HTML_FORMAT = 8;
            // Load COM-Metadata of Word application from registry
            Type tWordApplication = Type.GetTypeFromProgID("Word.Application");
            // Create new instance of Word
            object objWord = Activator.CreateInstance(tWordApplication);
            // List all documents
            object objDocuments = tWordApplication.InvokeMember("Documents", BindingFlags.IgnoreCase | BindingFlags.GetProperty | BindingFlags.Public, null, objWord, new object[0]);
            // Get COM-Metadata of Word Documents
            Type tWordDocuments = objDocuments.GetType();
            // Load source
            object objDocument = tWordDocuments.InvokeMember("Open", BindingFlags.IgnoreCase | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.OptionalParamBinding, null, objDocuments, new object[1] { strSource });
            // Get COM-Metadata of Word Documents
            Type tWordDocument = objDocument.GetType();
            // Create HTML
            tWordDocument.InvokeMember("SaveAs", BindingFlags.IgnoreCase | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.OptionalParamBinding, null, objDocument, new object[2] { strDestination, WORD_HTML_FORMAT });
            // Close Word
            tWordApplication.InvokeMember("Quit", BindingFlags.IgnoreCase | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.OptionalParamBinding, null, objWord, new object[0]);
        }
        [Test]
        public void GetUserName()
        {
            using (StreamReader sr = new StreamReader(@"D:\我的文档\Downloads\楼盘及房产品牌名称.txt", Encoding.Default))
            {
                string strWatiN = sr.ReadToEnd();
                Random rd = new Random();
                List<string> lSums = new List<string>();
                List<string> lNew = new List<string>();
                
                foreach(string s in strWatiN.Split(new char[]{'\n'}) ){
                    lSums.Add(s);
                }
                StringBuilder strSum = new StringBuilder("[");
                while(lNew.Count<2000 && lNew.Count<lSums.Count)
                {
                    string s=lSums[rd.Next(lSums.Count)];
                    //if (lNew.Contains(s) == false)
                    //{
                        lNew.Add(s);
                        strSum.Append("'"+s+"',");
                   // }
                }
                strSum.Append("];");
                Console.Write(strSum);

            }
        }
    }
}
