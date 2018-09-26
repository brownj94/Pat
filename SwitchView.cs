using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Pat
{
     class SwitchView
    {

        private DataGrid myDataGrid;
        private DataGrid myDataGrid2;
        private TextBlock recCountText;
        private TextBlock colCountText;
        private TextBlock anyMatchText;
        private TextBlock exactMatchText;
        private TextBlock recCountNum;
        private TextBlock colCountNum;
        private TextBlock anyMatchNum;
        private TextBlock exactMatchNum;
        private TextBox textBox3;
        private TextBox textBox4;
        private TextBox pathTextBox;
        private ScrollViewer resultSetScroll;
        private ScrollViewer headerRecScroll;
        private ScrollViewer errorScroll;
        private ScrollViewer codeScroll;
        private string currentView;

        public SwitchView()
        {

        }

        /// <summary>
        /// instatiate objects
        /// </summary>
        public void setVals(DataGrid p_myDataGrid,
            DataGrid p_myDataGrid2,
            TextBlock p_recCountText,
            TextBlock p_colCountText,
            TextBlock p_anyMatchText,
            TextBlock p_exactMatchText,
            TextBlock p_recCountNum,
            TextBlock p_colCountNum,
            TextBlock p_anyMatchNum,
            TextBlock p_exactMatchNum,
            TextBox p_textBox3,
            TextBox p_textBox4,
            TextBox p_pathTextBox,
            ScrollViewer p_resultSetScroll,
            ScrollViewer p_headerRecScroll,
            ScrollViewer p_errorScroll,
            ScrollViewer p_codeScroll,
            string p_currentView)
        {
            this.myDataGrid = p_myDataGrid;
            this.myDataGrid2 = p_myDataGrid2;
            this.recCountText = p_recCountText;
            this.colCountText = p_colCountText;
            this.anyMatchText = p_anyMatchText;
            this.exactMatchText = p_exactMatchText;
            this.recCountNum = p_recCountNum;
            this.colCountNum = p_colCountNum;
            this.anyMatchNum = p_anyMatchNum;
            this.textBox3 = p_textBox3;
            this.textBox4 = p_textBox4;
            this.pathTextBox = p_pathTextBox;
            this.exactMatchNum = p_exactMatchNum;
            this.resultSetScroll = p_resultSetScroll;
            this.headerRecScroll = p_headerRecScroll;
            this.errorScroll = p_errorScroll;
            this.codeScroll = p_codeScroll;
            this.currentView = p_currentView;
        }

        /// <summary>
        /// Clear all visual objects and set visual objects of the code matches errors window
        /// </summary> 
        public void view_code_matches()
        {
            if (this.currentView == "datagrid")
            {
                this.hide_datagrid();

            }
            else if (this.currentView == "errors")
            {
                this.textBox3.Visibility = Visibility.Hidden;
                this.errorScroll.Visibility = Visibility.Hidden;
            }

            this.codeScroll.Visibility = Visibility.Visible;
            this.textBox4.Visibility = Visibility.Visible;
            this.pathTextBox.Visibility = Visibility.Visible;
            this.currentView = "code_matches";
        }

        /// <summary>
        /// Clear all visual objects and set visual objects of the view errors window
        /// </summary>
        public void view_errors()
        {
            if (this.currentView == "datagrid")
            {
                this.hide_datagrid();
            }
            else if (this.currentView == "code_matches")
            {
                this.hide_code_matches();
            }

            this.textBox3.Visibility = Visibility.Visible;
            this.errorScroll.Visibility = Visibility.Visible;
            this.currentView = "errors";
        }

        /// <summary>
        /// Clear all visual objects and set visual objects of the datagrid window
        /// </summary>
        public void view_datagrid()
        {
            if (this.currentView == "errors")
            {
                this.textBox3.Visibility = Visibility.Hidden;
                this.errorScroll.Visibility = Visibility.Hidden;
            }
            else if (this.currentView == "code_matches")
            {
                this.hide_code_matches();
            }

            this.make_grid_visible();
            this.currentView = "datagrid";

        }

        /// <summary>
        /// hide extra objects from the datagrid window
        /// </summary>
        private void hide_code_matches()
        {
            this.codeScroll.Visibility = Visibility.Hidden;
            this.textBox4.Visibility = Visibility.Hidden;
            this.pathTextBox.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// hide datagrid1
        /// </summary>
        private void make_grid_visible()
        {
            this.myDataGrid.Visibility = Visibility.Visible;
            this.myDataGrid2.Visibility = Visibility.Visible;
            this.recCountText.Visibility = Visibility.Visible;
            this.colCountText.Visibility = Visibility.Visible;
            this.anyMatchText.Visibility = Visibility.Visible;
            this.exactMatchText.Visibility = Visibility.Visible;
            this.recCountNum.Visibility = Visibility.Visible;
            this.colCountNum.Visibility = Visibility.Visible;
            this.anyMatchNum.Visibility = Visibility.Visible;
            this.exactMatchNum.Visibility = Visibility.Visible;
            this.resultSetScroll.Visibility = Visibility.Visible;
            this.headerRecScroll.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// hide data grid2
        /// </summary>
        private void hide_datagrid()
        {
            this.myDataGrid.Visibility = Visibility.Hidden;
            this.myDataGrid2.Visibility = Visibility.Hidden;
            this.recCountText.Visibility = Visibility.Hidden;
            this.colCountText.Visibility = Visibility.Hidden;
            this.anyMatchText.Visibility = Visibility.Hidden;
            this.exactMatchText.Visibility = Visibility.Hidden;
            this.recCountNum.Visibility = Visibility.Hidden;
            this.colCountNum.Visibility = Visibility.Hidden;
            this.anyMatchNum.Visibility = Visibility.Hidden;
            this.exactMatchNum.Visibility = Visibility.Hidden;
            this.resultSetScroll.Visibility = Visibility.Hidden;
            this.headerRecScroll.Visibility = Visibility.Hidden;
        }

    }
}
