using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TestRecorder.Core.Actions;
using SourceGrid.Cells;
using Button=SourceGrid.Cells.Button;
using CheckBox=SourceGrid.Cells.CheckBox;
using ColumnHeader=SourceGrid.Cells.ColumnHeader;
using ComboBox=SourceGrid.Cells.Editors.ComboBox;
using TextBox=SourceGrid.Cells.Editors.TextBox;

namespace TestRecorder.UserControls
{
    public partial class ucBaseElement : ucBaseEditor
    {
        const int FIND_METHOD = 0;
        const int FIND_VALUE = 1;
        const int FIND_VALUE_DATA = 2;
        const int FIND_VALUE_REGEX = 3;
        const int ADD_REMOVE = 4;

        public ucBaseElement()
        {
            InitializeComponent();
        }

        private void SetHeader()
        {
            gridElement.RowsCount = 0;
            gridElement.ColumnsCount = 7;
            gridElement.FixedRows = 1;
            gridElement.Rows.Insert(0);
            const int RowIndex = 0;

            gridElement[RowIndex, FIND_METHOD] = new ColumnHeader("Find Method");
            ((ColumnHeader)gridElement[RowIndex, FIND_METHOD]).AutomaticSortEnabled = false;
            gridElement[RowIndex, FIND_METHOD].Column.Width = 100;

            gridElement[RowIndex, FIND_VALUE] = new ColumnHeader("Find Value");
            ((ColumnHeader)gridElement[RowIndex, FIND_VALUE]).AutomaticSortEnabled = false;
            gridElement[RowIndex, FIND_VALUE].Column.Width = 150;

            gridElement[RowIndex, FIND_VALUE_DATA] = new ColumnHeader("");
            ((ColumnHeader)gridElement[RowIndex, FIND_VALUE_DATA]).AutomaticSortEnabled = false;
            gridElement[RowIndex, FIND_VALUE_DATA].Column.Width = 25;

            gridElement[RowIndex, FIND_VALUE_REGEX] = new ColumnHeader("Regex");
            ((ColumnHeader)gridElement[RowIndex, FIND_VALUE_REGEX]).AutomaticSortEnabled = false;
            gridElement[RowIndex, FIND_VALUE_REGEX].Column.Width = 50;

            gridElement[RowIndex, ADD_REMOVE] = new ColumnHeader("");
            ((ColumnHeader)gridElement[RowIndex, ADD_REMOVE]).AutomaticSortEnabled = false;
            gridElement[RowIndex, ADD_REMOVE].Column.Width = 25;
        }

        internal FindAttribute GetRowValue(int RowIndex)
        {
            var attribute = new FindAttribute
                                {
                                    FindName = gridElement[RowIndex, FIND_METHOD].Value.ToString()
                                };
            if (Enum.IsDefined(typeof(FindMethods), attribute.FindName))
            {
                attribute.FindMethod = (FindMethods) Enum.Parse(typeof (FindMethods), attribute.FindName);
                attribute.FindName = null;
            }
            attribute.FindValue = gridElement[RowIndex, FIND_VALUE].ToString();
            return attribute;
        }

        internal void AddGridRow(FindAttribute attribute)
        {
            int RowIndex = gridElement.RowsCount++;

            gbFindElement.Height = (gridElement.RowsCount+1)*gridElement.Rows.GetHeight(RowIndex);

            var comboStandard = new ComboBox(typeof(FindMethods));
            string findstring = attribute.FindMethod.ToString();
            gridElement[RowIndex, FIND_METHOD] = new Cell(findstring, comboStandard);

            var txtFindValue = new TextBox(typeof (string));
            gridElement[RowIndex, FIND_VALUE] = new Cell(attribute.FindValue, txtFindValue);

            var bmpDB = new Bitmap(GetIconFullPath("database.bmp"));
            bmpDB.MakeTransparent(Color.Fuchsia);

            gridElement[RowIndex, FIND_VALUE_DATA] = new Cell("");
            gridElement[RowIndex, FIND_VALUE_DATA] = new Button("") {Image = bmpDB};
            var dbClickEvent = new SourceGrid.Cells.Controllers.Button();
            dbClickEvent.Executed += DBSelectButton_Click;
            gridElement[RowIndex, FIND_VALUE_DATA].Controller.AddController(dbClickEvent);
            gridElement[RowIndex, FIND_VALUE_REGEX] = new CheckBox(null, true);

            if (gridElement.RowsCount < 3)
            {
                gridElement[RowIndex, ADD_REMOVE] = new Button("");
                var bmpAdd = new Bitmap(GetIconFullPath("db-add.bmp"));
                bmpAdd.MakeTransparent(Color.Fuchsia);
                gridElement[RowIndex, ADD_REMOVE].Image = bmpAdd;
                var buttonClickEvent = new SourceGrid.Cells.Controllers.Button();
                buttonClickEvent.Executed += AddFindButton_Click;
                gridElement[RowIndex, ADD_REMOVE].Controller.AddController(buttonClickEvent);
            }
            else
            {
                var btnDelete = new Button("");
                gridElement[RowIndex, ADD_REMOVE] = btnDelete;
                var bmpDelete = new Bitmap(GetIconFullPath("db-Delete.bmp"));
                bmpDelete.MakeTransparent(Color.Fuchsia);
                gridElement[RowIndex, ADD_REMOVE].Image = bmpDelete;
                var buttonClickEvent = new SourceGrid.Cells.Controllers.Button();
                buttonClickEvent.Executed += DeleteFindButton_Click;
                gridElement[RowIndex, ADD_REMOVE].Controller.AddController(buttonClickEvent);
            }
        }

        private static string GetIconFullPath(string Filename)
        {
            Filename = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Icons\\" + Filename);
            return Filename;
        }

        public void AddItemsToMenu(ContextMenuStrip menu)
        {
            menu.Items.Clear();
            //if (ActionList.ReplacementTable.Rows.Count == 0) return;
            //foreach (DataColumn column in ActionList.ReplacementTable.Columns)
            //{
            //    ToolStripItem item = menu.Items.Add(column.ColumnName);
            //    item.Click += ReplacementDBItem_Click;
            //}
        }

        private void DBSelectButton_Click(object sender, EventArgs e)
        {
            AddItemsToMenu(menuFindValue);
            menuFindValue.Show(MousePosition.X, MousePosition.Y);
        }

        private void DeleteFindButton_Click(object sender, EventArgs e)
        {
            int Index = ((SourceGrid.CellContext) sender).Position.Row;
            gridElement.Rows.Remove(Index);
            gbFindElement.Height = (gridElement.RowsCount + 1) * gridElement.Rows.GetHeight(1);
        }

        private void AddFindButton_Click(object sender, EventArgs e)
        {
            var attribute = new FindAttribute(FindMethods.Id, "", false);
            AddGridRow(attribute);
        }

        public void ObjectToGui(ActionElementBase action)
        {
            gridElement.Rows.Clear();
            SetHeader();
            foreach (FindAttribute attribute in action.Context.FindMechanism)
            {
                AddGridRow(attribute);
            }
        }

        public FindAttributeCollection GuiToObject()
        {
            var collection = new FindAttributeCollection();
            for (int i = 1; i < gridElement.RowsCount; i++)
            {
                FindAttribute attribute = GetRowValue(i);
                collection.Add(attribute);             
            }
            return collection;
        }
    }
}
