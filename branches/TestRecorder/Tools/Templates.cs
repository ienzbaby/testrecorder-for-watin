using System.Collections.Generic;
using System.Text;

namespace TestRecorder.Tools
{
    /// <summary>
    /// 模板列表
    /// </summary>
    public sealed class Templates
    {
        private List<Template> TemplateList = new List<Template>();
        /// <summary>
        ///create loads the list of templates for language 
        /// </summary>
        /// <param name="path"></param>
        public Templates(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                return;
            }
            string[] arrFiles = System.IO.Directory.GetFiles(path, "*.trt");

            for (int i = 0; i < arrFiles.Length; i++)
            {
                var temp = new Template(arrFiles[i]);
                TemplateList.Add(temp);
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public Template GetTemplate(int Index)
        {
            var resultlist = new List<Template>();
            foreach (Template tfile in TemplateList)
            {
                resultlist.Add(tfile);
            }

            if (Index > resultlist.Count - 1)
            {
                return null;
            }
            return resultlist[Index];
        }
        /// <summary>
        /// 取得文件类型过滤
        /// </summary>
        /// <returns></returns>
        public string GetSaveFilter()
        {
            var sbResult = new StringBuilder();
            foreach (Template tfile in TemplateList)
            {
                sbResult.Append("|" + tfile.Name + " (" + tfile.FileExtension + ")|" + tfile.FileExtension);
            }
            sbResult = sbResult.Remove(0, 1);
            return sbResult.ToString();
        }

        public List<Template> GetList()
        {
            var resultlist = new List<Template>();
            foreach (Template tfile in TemplateList)
            {
                resultlist.Add(tfile);
            }
            return resultlist;
        }

        public List<Template> GetList(AppSettings.CodeLanguages language)
        {
            var resultlist = new List<Template>();
            foreach (Template tfile in TemplateList)
            {
                if (language != tfile.CodeLanguage)
                {
                    continue;
                }
                resultlist.Add(tfile);
            }
            return resultlist;
        }

        public Template GetTemplate(string templatename)
        {
            foreach (Template tfile in TemplateList)
            {
                if (templatename == tfile.Name)
                {
                    return tfile;
                }
            }
            return null;
        }
    }
}
