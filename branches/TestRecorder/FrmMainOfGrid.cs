using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SourceGrid;
using TestRecorder.Core.Actions;
using ColumnHeader = SourceGrid.Cells.ColumnHeader;
using TestRecorder.Tools;


namespace TestRecorder
{
    public partial class frmMain : Form
    {

        /// <summary>
        /// Set up the grid header
        /// </summary>
        void SetGridHeader()
        {
            gridSource.RowsCount = 0;
            gridSource.ColumnsCount = 3;
            gridSource.FixedRows = 1;
            gridSource.Rows.Insert(0);
            const int rowIndex = 0;
            gridSource[rowIndex, 0] = new ColumnHeader("Status");
            ((ColumnHeader)gridSource[rowIndex, 0]).AutomaticSortEnabled = false;
            gridSource[rowIndex, 0].Column.Width = 50;
            gridSource[rowIndex, 1] = new ColumnHeader("Type");
            ((ColumnHeader)gridSource[rowIndex, 1]).AutomaticSortEnabled = false;
            gridSource[rowIndex, 1].Column.Width = 45;
            gridSource[rowIndex, 2] = new ColumnHeader("Description");
            ((ColumnHeader)gridSource[rowIndex, 2]).AutomaticSortEnabled = false;
            gridSource[rowIndex, 2].Column.Width = splitContainer1.Panel1.Width - 95;
        }

        private void RefreshGrid()
        {
            this.ClearGrid();
            foreach (ActionBase action in wsManager.ActiveList)
            {
                try
                {
                    this.AddGridRowItem(action);
                }
                catch (Exception)
                {
                    // swallow the exception
                }
            }
        }

        void AddGridRowItem(ActionBase actionObject)
        {
            if (InvokeRequired)
            {
                Invoke(new GridAddEvent(AddGridRowItem), actionObject);
                return;
            }
            InsertGridRowItem(gridSource.RowsCount, actionObject);
        }
        void InsertGridRowItem(int rowIndex, ActionBase action)
        {
            if (InvokeRequired)
            {
                Invoke(new GridInsertEvent(InsertGridRowItem), rowIndex, action);
                return;
            }

            if (gridSource.RowsCount <= rowIndex) 
                gridSource.RowsCount++;
            else 
                gridSource.Rows.Insert(rowIndex);

            SetCellBreakpoint(rowIndex, action.Breakpoint);

            gridSource[rowIndex, 1] = new SourceGrid.Cells.Image(action.GetIcon());

            string description = action.Description;
            // Set the column width to the maximum needed to display 
            // all of the description text
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                SizeF sz = g.MeasureString(description, gridSource.Font);
                Int32 myMaxWidth = Convert.ToInt32(sz.Width);
                if (myMaxWidth > gridSource[0, 2].Column.Width)
                {
                    gridSource[0, 2].Column.Width = myMaxWidth;
                }
            }

            gridSource[rowIndex, 2] = new SourceGrid.Cells.Cell(description);
            gridSource.Rows.SetHeight(rowIndex, 30);
        }
        private void SetCellBreakpoint(int rowIndex, BreakpointIndicators breakpointType)
        {
            if (InvokeRequired)
            {
                Invoke(new SetCellBreakpointEvent(SetCellBreakpoint), rowIndex, breakpointType);
                return;
            }

            string filename = Settings.IconDirectory;

            if (breakpointType == BreakpointIndicators.ActiveBreakpoint)
            {
                filename = Path.Combine(filename, "breakpoint.bmp");
            }
            else if (breakpointType == BreakpointIndicators.InactiveBreakpoint)
            {
                filename = Path.Combine(filename, "inactivebreak.bmp");
            }
            else
            {
                filename = "";
            }

            if (filename != "")
            {
                var bmp = new Bitmap(filename);
                bmp.MakeTransparent(Color.Fuchsia);
                gridSource[rowIndex, 0] = new SourceGrid.Cells.Image(bmp);
            }
            else gridSource[rowIndex, 0] = new SourceGrid.Cells.Image();

            gridSource.InvalidateCell(new Position(rowIndex, 0));
        }
        private void ClearGrid()
        {
            gridSource.Rows.Clear();
            SetGridHeader();
        }
        private void DeleteGridRow(int index)
        {
            gridSource.Rows.Remove(index + 1);
        }
        private void SelectGridRow(int index)
        {
            if (InvokeRequired)
            {
                Invoke(new ActionCounterIncrementedEvent(SelectGridRow), index);
                return;
            }
            currentIndex = index;
            gridSource.Selection.ResetSelection(false);
            gridSource.Selection.SelectRow(index, true);
        }

        /// <summary>
        /// 可见行改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void gridSource_VisibleChanged(object sender, System.EventArgs e)
        {
            if (gridSource.RowsCount > 0)
            {
                // Update the grid //有需要更新么？郝利鹏 2011-11-05
                //ClearGrid(); 
                //foreach (ActionBase localAction in wscript.CurrentActiveList)
                //{
                //    try
                //    {
                //        if (OnGridAdd != null) OnGridAdd(localAction);
                //    }
                //    catch (Exception)
                //    {
                //        // swallow the exception
                //    }
                //}
                gridSource.Refresh();
            }
        }
        /// <summary>
        /// 鼠标点击行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void gridSource_MouseClick(object sender, MouseEventArgs e)
        {
            if (
                   gridSource.Selection == null
                || gridSource.Selection.ActivePosition.Row < 1
                || gridSource.Selection.ActivePosition.Row >= gridSource.RowsCount
               ) return;

            var action = wsManager.ActiveList[gridSource.Selection.ActivePosition.Row - 1];
            if (action == null) return;

            if (gridSource.Selection.ActivePosition.Column == 0 && action.Breakpoint == BreakpointIndicators.NoBreakpoint)
            {
                action.Breakpoint = BreakpointIndicators.ActiveBreakpoint;
            }
            else if (gridSource.Selection.ActivePosition.Column == 0 && action.Breakpoint == BreakpointIndicators.ActiveBreakpoint)
            {
                action.Breakpoint = BreakpointIndicators.NoBreakpoint;
            }

            SetCellBreakpoint(gridSource.Selection.ActivePosition.Row, action.Breakpoint);
            currentIndex = gridSource.Selection.ActivePosition.Row - 1;
        }

        void gridSource_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (gridSource.Selection.ActivePosition.Row < 1) return;
            var action = wsManager.ActiveList[gridSource.Selection.ActivePosition.Row - 1];
            ShowEditAction(action);
        }

    }
}
