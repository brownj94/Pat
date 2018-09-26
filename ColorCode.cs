using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Pat
{
    class ColorCode
    {

        /// <summary>
        /// helper method that gets the visual child
        /// </summary>       
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {     
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, 0);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }        
            return child;
        }

        /// <summary>
        /// This will set a grid cell to green if theres a match
        /// </summary> 
        public static void colorCode(int colcount, Matches mainRec, DataGrid p_datagrid)
        {
            DataGrid recDatagr1 = p_datagrid;
            foreach (var item in recDatagr1.Items)
            {
                //get the current row
                DataGridRow row = recDatagr1.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                //if there is no row object refresh the grid and get the current row
                if (row == null)
                {
                    p_datagrid.UpdateLayout();
                    row = recDatagr1.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                }

                if (row != null)
                {
                    for (int b = 0; b < colcount; b++)
                    {
                        DataGridCell cell = GetCell(recDatagr1, row, b);
                        //get the value of the cell and compare it to the corresponding subject data set
                        //if its a match set it to green
                        string content = ((TextBlock)(cell.Content)).Text.ToString();
                        string patRec = mainRec.row[1].column[b].Value;
                        if (content == patRec)
                        {
                            cell.Background = Brushes.Green;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the cell and its data for decision making
        /// </summary> 
        public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                //get the childern of the visual presenter 
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    rowContainer.ApplyTemplate();
                    presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                if (presenter != null)
                {
                    //get the findal cell object
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    //if the cell object is null refresh grid and get the current cell object
                    if (cell == null)
                    {
                        dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                        dataGrid.UpdateLayout();
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }

                    return cell;
                }
            }
            return null;
        }
    }
}
