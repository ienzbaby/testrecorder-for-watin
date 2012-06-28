using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections.Specialized;
using TestRecorder.Tools;

namespace TestRecorder
{
    public partial class frmLocateResource : Form
    {
        private bool OnlyNotFound = true;
        private Template CurrentTemplate;
        private StringCollection FunctionAssemblies;
        private readonly List<UnlocatedItem> ItemList = new List<UnlocatedItem>();

        public frmLocateResource()
        {
            InitializeComponent();
        }

        public void ShowResourceList(Template TemplateFile, StringCollection functionAssemblies, bool OnlyIfNotFound)
        {
            OnlyNotFound = OnlyIfNotFound;
            lblLocate.Text = OnlyIfNotFound ? Properties.Resources.SomeItemsCannotBeFound : Properties.Resources.ListOfResources;

            CurrentTemplate = TemplateFile;
            FunctionAssemblies = functionAssemblies;

            StringCollection files = TemplateFile.GetAssemblyList();
            AddFileCollection("Assembly", files, OnlyIfNotFound);
            AddFileCollection("Includes", TemplateFile.IncludedFiles, OnlyIfNotFound);
            AddFileCollection("Functions", functionAssemblies, OnlyIfNotFound);
            AddFile("Startup", TemplateFile.StartupApplication);

            ShowDialog();
        }

        private void AddFileCollection(string source, StringCollection Files, bool OnlyIfNotFound)
        {
            for (int i = 0; i < Files.Count; i++)
            {
                if (Files[i] == null || Files[i].Trim() == "")
                {
                    continue;
                }

                if (!File.Exists(Files[i]))
                {
                    AddFile(source, Files[i]);
                }
                else if (!OnlyIfNotFound)
                {
                    AddFile(source, Files[i]);
                }
            }
        }

        private void AddFile(string source, string Filename)
        {
            if (Filename.Trim() == "")
            {
                return;
            }

            dgvResources.Rows.Add();
            DataGridViewRow row = dgvResources.Rows[dgvResources.Rows.Count - 1];

            row.Cells[0].Value = File.Exists(Filename) ? imageList1.Images[1] : imageList1.Images[0];
            
            row.Cells[1].Value = source;
            row.Cells[2].Value = Filename.Replace(@"\\", @"\");
            row.Cells[3].Value = "...";

            var uitem = new UnlocatedItem {Filename = Filename.Replace(@"\\", @"\"), Source = source};
            ItemList.Add(uitem);
        }

        public void ModifyFunctionAssemblyPath(string OldPath, string NewPath)
        {
            for (int i = 0; i < FunctionAssemblies.Count; i++)
            {
                if (System.IO.Path.GetFileName(OldPath).ToLower() == System.IO.Path.GetFileName(FunctionAssemblies[i]).ToLower())
                {
                    FunctionAssemblies[i] = NewPath;
                    return;
                }
            }
        }

        private void dgvResources_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                openFileDialog1.FileName = ItemList[e.RowIndex].Filename;
                openFileDialog1.Title = string.Format("Find {0}",System.IO.Path.GetFileName(ItemList[e.RowIndex].Filename));
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (!File.Exists(openFileDialog1.FileName))
                    {
                        return;
                    }

                    // change the information
                    switch (ItemList[e.RowIndex].Source)
                    {
                        case "Assembly": CurrentTemplate.ModifyAssemblyPath(ItemList[e.RowIndex].Filename, openFileDialog1.FileName); break;
                        case "Includes": CurrentTemplate.ModifyIncludePath(ItemList[e.RowIndex].Filename, openFileDialog1.FileName); break;
                        case "Functions": ModifyFunctionAssemblyPath(ItemList[e.RowIndex].Filename, openFileDialog1.FileName); break;
                        case "Startup": CurrentTemplate.ModifyStartupApplication(openFileDialog1.FileName); break;
                    }

                    dgvResources.Rows[e.RowIndex].Cells[2].Value = openFileDialog1.FileName;
                    ItemList[e.RowIndex].Filename = openFileDialog1.FileName;
                    dgvResources.Rows[e.RowIndex].Cells[0].Value = imageList1.Images[1];
                }

                var scFiles = new StringCollection();
                for (int i = 0; i < ItemList.Count; i++)
                {
                    scFiles.Add(ItemList[i].Filename);
                }
                if (Template.AllFilesExistInList(scFiles) && OnlyNotFound)
                {
                    Close();
                }
            }
        }
    }

    class UnlocatedItem
    {
        public string Filename;
        //public bool Found;
        public string Source = "";
    }
}