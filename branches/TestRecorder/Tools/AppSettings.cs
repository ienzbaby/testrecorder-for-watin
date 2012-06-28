using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using TestRecorder.Core.Actions;

namespace TestRecorder.Tools
{
    /// <summary>
    /// ”¶”√≥Ã–Ú≈‰÷√
    /// </summary>
    public class AppSettings : Settings
    {
        public string BaseIEName = "ie";
        public string PopupIEName = "iepopup";
        public double TypingTime = 1000;
        public bool WarnWhenUnsaved = true;
        public bool SetMaxSize = false;
        public int GlobalWaitTime = 100;
        public enum ScriptFormats { Snippet, Console, NUnit, MBUnit, VS2005Library }
        public ScriptFormats ScriptFormatting = ScriptFormats.Snippet;
        public string CompilePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath)+"\\Compilations\\";

        public Font ScriptWindowFont;
        public Color DOMHighlightColor = Color.Yellow;
        public string NUnitFrameworkPath = "";
        public string MBUnitFrameworkPath = "";
        public string VS2005FrameworkPath = "";
        public System.Collections.Specialized.StringCollection ReferencedAssemblies = new System.Collections.Specialized.StringCollection();
        public bool HideDOSWindow;
        public RichTextBox rtbTarget;
        public List<FindMethods> FindPattern = new List<FindMethods>();
        public enum CodeLanguages { CSharp, VBNet, PHP, Python, Perl, Ruby }
        public CodeLanguages CodeLanguage = CodeLanguages.CSharp;
        public string DefaultSaveTemplate = "";
        public string DefaultRunTemplate = "";
        public string DefaultCompileTemplate = "";
        public int RunCount;

        public AppSettings() : base()
        {
            LoadSettings();
        }
        
        public AppSettings(string path) : base(path)
        {
            LoadSettings();
        }

        private void LoadFindPattern(string PatternSetting)
        {
            string[] arrFindMethod = PatternSetting.Split(",".ToCharArray());
            foreach (string method in arrFindMethod)
            {
                FindMethods f = (FindMethods)Enum.Parse(typeof(FindMethods), method);
                if (FindPattern.Contains(f) == false) FindPattern.Add(f);
            }
        }

        private string GetFindPattern()
        {
            var builder = new StringBuilder();
            foreach (var method in FindPattern)
            {
                if (builder.Length > 0) builder.Append(",");
                builder.Append(method);
            }
            return builder.ToString();
        }

        public void LoadSettings()
        {
            BaseIEName = GetSetting("BaseIEName", "ie");
            PopupIEName = GetSetting("PopupIEName", "iepopup");
            RunCount = Convert.ToInt32(GetSetting("RunCount", "0"));
            TypingTime = Convert.ToDouble(GetSetting("TypingTime", "1000"));
            WarnWhenUnsaved = GetSetting("WarnWhenUnsaved", 0) == 1 ? true : false;
            SetMaxSize = GetSetting("SetMaxSize", 0) == 1 ? true : false;
            GlobalWaitTime =int.Parse(GetSetting("GlobalWaitTime", "100"));
            HideDOSWindow = GetSetting("HideDOSWindow", 1) == 1 ? true : false;
            CompilePath = GetSetting("CompilePath", AppDirectory);
            LoadFindPattern(GetSetting("FindPattern", " Id, Name, Href, Url, Src, Value, Text,Class,Index"));
            DOMHighlightColor = Color.FromName(GetSetting("DOMHighlightColor", "Yellow"));
            ScriptFormatting = (ScriptFormats)Enum.Parse(typeof(ScriptFormats), GetSetting("ScriptFormatting", "Snippet"), true);
            CodeLanguage = (CodeLanguages)Enum.Parse(typeof(CodeLanguages), GetSetting("CodeLanguage", "CSharp"), true);

            DefaultSaveTemplate = GetSetting("DefaultSaveTemplate", "");
            DefaultRunTemplate = GetSetting("DefaultRunTemplate", "");
            DefaultCompileTemplate = GetSetting("DefaultCompileTemplate", "");
        }

        public void SaveSettings()
        {
            try
            {
                PutSetting("BaseIEName", BaseIEName);
                PutSetting("PopupIEName", PopupIEName);
                PutSetting("TypingTime", TypingTime.ToString());
                PutSetting("FindPattern", GetFindPattern());
                PutSetting("WarnWhenUnsaved", WarnWhenUnsaved ? 1 : 0);
                PutSetting("SetMaxSize", SetMaxSize ? 1 : 0);
                PutSetting("GlobalWaitTime", GlobalWaitTime);
                PutSetting("HideDOSWindow", HideDOSWindow ? 1 : 0);
                PutSetting("CompilePath", CompilePath);
                PutSetting("DOMHighlightColor", DOMHighlightColor.ToKnownColor().ToString());
                PutSetting("CodeLanguage", Enum.GetName(typeof(CodeLanguages), CodeLanguage));
                PutSetting("ScriptFormatting", Enum.GetName(typeof(ScriptFormats), ScriptFormatting));
                PutSetting("RunCount", ++RunCount);

                PutSetting("DefaultSaveTemplate", DefaultSaveTemplate);
                PutSetting("DefaultRunTemplate", DefaultRunTemplate);
                PutSetting("DefaultCompileTemplate", DefaultCompileTemplate);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error saving settings: {0}",ex.Message));
            }

        }
    }
}
