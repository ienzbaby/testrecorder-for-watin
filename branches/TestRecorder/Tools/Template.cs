using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Runtime.InteropServices;
using TestRecorder.Core;
using TestRecorder.Tools;

namespace TestRecorder.Tools
{
    /// <summary>
    /// 模板
    /// </summary>
    public class Template : Settings
    {
        public string Name = "";
        string TemplatePath = "";
        public AppSettings.CodeLanguages CodeLanguage = AppSettings.CodeLanguages.CSharp;
        public string FileExtension = "*.cs";
        public bool CanCompile = true;
        public bool CanRun = true;
        public StringCollection ReferencedAssemblies = new StringCollection();
        readonly string UsingFormat = "using USINGNAMESPACE;";
        public StringCollection IncludedFiles = new StringCollection();
        readonly string CodePage = "";
        readonly string MethodBlock = "";
        public string StartupApplication = "";
        public ICodeFormatter CodeFormatter = null;

        public void ResetFormatter()
        {
            switch (CodeLanguage)
            {
                case AppSettings.CodeLanguages.CSharp: CodeFormatter = new CSharpCodeFormatter(); break;
                case AppSettings.CodeLanguages.VBNet: CodeFormatter = new VBNetCodeFormatter(); break;
                case AppSettings.CodeLanguages.Perl: CodeFormatter = new PerlCodeFormatter(); break;
                case AppSettings.CodeLanguages.PHP: CodeFormatter = new PhpCodeFormatter(); break;
                case AppSettings.CodeLanguages.Python: CodeFormatter = new PythonCodeFormatter(); break;
                case AppSettings.CodeLanguages.Ruby: CodeFormatter = new RubyCodeFormatter(); break;
            }
        }

        public Template(string path)
            : base(path)
        {
            Name = GetSetting("TemplateName", "(Name Not Set)");
            StartupApplication = GetSetting("StartupApplication", "");
            FileExtension = GetSetting("FileExtension", "*.cs");
            TemplatePath = path;

            switch (GetSetting("CodeLanguage", "CSharp"))
            {
                case "CSharp": CodeLanguage = AppSettings.CodeLanguages.CSharp; break;
                case "VBNet": CodeLanguage = AppSettings.CodeLanguages.VBNet; break;
                case "PHP": CodeLanguage = AppSettings.CodeLanguages.PHP; break;
                case "Python": CodeLanguage = AppSettings.CodeLanguages.Python; break;
                case "Perl": CodeLanguage = AppSettings.CodeLanguages.Perl; break;
            }

            ResetFormatter();

            CanCompile = GetSetting("CanCompile", 1) == 1 ? true : false;
            CanRun = GetSetting("CanRun", 1) == 1 ? true : false;

            string[] arrAssemblies = GetSetting("ReferencedAssemblies", "").Split(Environment.NewLine.ToCharArray());
            for (int i = 0; i < arrAssemblies.Length; i++)
            {
                if (arrAssemblies[i].Trim() != "")
                {
                    ReferencedAssemblies.Add(arrAssemblies[i]);
                }
            }

            string[] arrInclude = GetSetting("IncludedFiles", "").Split(Environment.NewLine.ToCharArray());
            for (int i = 0; i < arrInclude.Length; i++)
            {
                if (arrInclude[i].Trim() != "")
                {
                    IncludedFiles.Add(arrInclude[i]);
                }
            }

            UsingFormat = GetSetting("UsingFormat", "");
            CodePage = GetSetting("code", "");
            MethodBlock = GetSetting("Method", "");
        }

        public void SaveTemplate()
        {
            PutSetting("Name", Name);
            switch (CodeLanguage)
            {
                case AppSettings.CodeLanguages.CSharp: PutSetting("CodeLanguage", "CSharp"); break;
                case AppSettings.CodeLanguages.VBNet: PutSetting("CodeLanguage", "VBNet"); break;
                case AppSettings.CodeLanguages.PHP: PutSetting("CodeLanguage", "PHP"); break;
                case AppSettings.CodeLanguages.Python: PutSetting("CodeLanguage", "Python"); break;
                case AppSettings.CodeLanguages.Perl: PutSetting("CodeLanguage", "Perl"); break;
            }

            PutSetting("CanCompile", CanCompile ? 1 : 0);
            PutSetting("CanRun", CanRun ? 1 : 0);
            PutSetting("ReferencedAssemblies", JoinList(ReferencedAssemblies));
            PutSetting("IncludedFiles", JoinList(IncludedFiles));
            PutSetting("UsingFormat", UsingFormat);
            PutSetting("code", CodePage);
            PutSetting("Method", MethodBlock);
            PutSetting("StartupApplication", StartupApplication);
        }

        public static string JoinList(StringCollection slist)
        {
            var sbList = new StringBuilder();
            for (int i = 0; i < slist.Count; i++)
            {
                if (slist[i].Trim() == "")
                {
                    continue;
                }
                sbList.AppendLine(slist[i]);
            }
            return sbList.ToString();
        }

        public static string JoinList(string[] slist)
        {
            var sbList = new StringBuilder();
            for (int i = 0; i < slist.Length; i++)
            {
                if (slist[i].Trim() == "")
                {
                    continue;
                }
                sbList.AppendLine(slist[i]);
            }
            return sbList.ToString();
        }

        public StringCollection GetAssemblyList()
        {
            var scAssemblies = new StringCollection();
            string NetPath = RuntimeEnvironment.GetRuntimeDirectory();

            for (int i = 0; i < ReferencedAssemblies.Count; i++)
            {
                if (ReferencedAssemblies[i].Trim() == "")
                {
                    continue;
                }

                // Replace NETPATH and APPPath
                string assembly = ReferencedAssemblies[i].Trim().Replace("%NETPATH%", NetPath);
                assembly = assembly.Replace("%APPPATH%", AppDirectory);
                assembly = assembly.Replace(@"\\", @"\");

                scAssemblies.Add(assembly);
            }
            return scAssemblies;
        }

        public static bool AllFilesExistInList(StringCollection fileList)
        {
            string NetPath = RuntimeEnvironment.GetRuntimeDirectory();

            for (int i = 0; i < fileList.Count; i++)
            {
                // Replace NETPATH and APPPath
                string assembly = fileList[i].Trim().Replace("%NETPATH%", NetPath);
                assembly = assembly.Replace("%APPPATH%", AppDirectory);
                assembly = assembly.Replace(@"\\", @"\");

                if (!System.IO.File.Exists(assembly))
                {
                    return false;
                }
            }
            return true;
        }

        public void ModifyAssemblyPath(string OldPath, string NewPath)
        {
            for (int i = 0; i < ReferencedAssemblies.Count; i++)
            {
                if (System.IO.Path.GetFileName(OldPath).ToLower() == System.IO.Path.GetFileName(ReferencedAssemblies[i]).ToLower())
                {
                    ReferencedAssemblies[i] = NewPath;
                    SaveTemplate();
                    return;
                }
            }
        }

        public void ModifyIncludePath(string OldPath, string NewPath)
        {
            for (int i = 0; i < IncludedFiles.Count; i++)
            {
                if (System.IO.Path.GetFileName(OldPath).ToLower() == System.IO.Path.GetFileName(IncludedFiles[i]).ToLower())
                {
                    IncludedFiles[i] = NewPath;
                    SaveTemplate();
                    return;
                }
            }
        }

        public void ModifyStartupApplication(string NewPath)
        {
            StartupApplication = NewPath;
            SaveTemplate();
        }

        private static string TabAllLinesTwice(string TestCode)
        {
            string[] lines = TestCode.Split(Environment.NewLine.ToCharArray());
            var sbcode = new StringBuilder();
            for (int i = 0; i < lines.Length; i++)
            {
                sbcode.Append("\t\t" + lines[i] + Environment.NewLine);
            }
            return sbcode.ToString();
        }

        public string PrepareScript(NameValueCollection testcode)
        {
            ResetFormatter();

            var sbCode = new StringBuilder();
            sbCode.AppendLine(CodePage);

            var sbNames = new StringBuilder();
            var sbMethods = new StringBuilder();
            for (int i = 0; i < testcode.Count; i++)
            {
                sbMethods.AppendLine(MethodBlock);
                sbMethods.Replace("TESTNAME", testcode.GetKey(i));
                sbMethods.Replace("TESTCODE", TabAllLinesTwice(JoinList(testcode.GetValues(i))));
            }

            sbCode.Replace("TESTMETHODLIST", sbNames.ToString());
            sbCode.Replace("TESTMETHODCODE", sbMethods.ToString());

            return sbCode.ToString();
        }
    }
}
