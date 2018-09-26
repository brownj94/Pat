using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using PriorityHealth.DB.OracleConnectionFactory;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Threading;

namespace Pat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string text1;
        string text2;
        SwitchView switchView = new SwitchView();

        public MainWindow()
        {
            this.DataContext = this;
            this.InitializeComponent();
            this.switchView.setVals(this.myDataGrid,
            this.myDataGrid2,
            this.recCountText,
            this.colCountText,
            this.anyMatchText,
            this.exactMatchText,
            this.recCountNum,
            this.colCountNum,
            this.anyMatchNum,
            this.exactMatchNum,
            this.textBox3,
            this.textBox4,
            this.pathTextBox,
            this.resultSetScroll,
            this.headerRecScroll,
            this.errorScroll,
            this.codeScroll,
            "datagrid");

        }

        public void SetStats(Stats s)
        {
            recCountNum.Text = s.xtotalRows;
            colCountNum.Text = s.xcolumnCount;
            anyMatchNum.Text = s.xrecsWMatches;
            exactMatchNum.Text = s.xrecsWExactMatches;
        }

        private void view_code_matches(object sender, RoutedEventArgs e)
        {
            this.switchView.view_code_matches();
        }

        private void view_errors(object sender, RoutedEventArgs e)
        {
            this.switchView.view_errors();            
        }

        private void view_datagrid(object sender, RoutedEventArgs e)
        {
            this.switchView.view_datagrid();            
        }

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            this.pathTextBox.Text = " ";

        }
         
        private void Scroll_Change(object sender, RoutedEventArgs e)
        {
            resultSetScroll.ScrollToHorizontalOffset(headerRecScroll.HorizontalOffset);
        }
        
        private void clearGrid()
        {
            try {
                this.Dispatcher.Invoke(() =>
                {
                    myDataGrid.ItemsSource = null;
                    myDataGrid2.ItemsSource = null;
                    myDataGrid.Items.Clear();
                    myDataGrid2.Items.Clear();
                });
            }
            catch(Exception ex)
            {

            }
        }

        //
        private void getData()
        {
            
            this.Dispatcher.Invoke(() =>
            { 
                DBAccess matchResults = new DBAccess();
                System.Threading.Thread findPatterns = new System.Threading.Thread(matchResults.Begin);
                try
                {      //find patterns between data sets provided by user          
                       matchResults.FindPatterns(this.text1, this.text2, this.myDataGrid, this.myDataGrid2);
                }
                catch (Exception ex)
                {
                    textBox3.Text = textBox3.Text + "\r\n" + "\r\n" + Convert.ToDateTime(DateTime.Now.ToString("dd-MMM-yyyy")) + "\r\n"
                    + "-------------------------------" + "\r\n" + ex;
                    return;
                }

                findPatterns.Start();

                Stats stats = matchResults.Stats;
                this.SetStats(stats);

                myDataGrid.UpdateLayout();
                myDataGrid2.UpdateLayout();

                //Find patterns between data sets and code
                if (String.Equals(pathTextBox.Text, "Path") == false)
                {
                    if (String.Equals(pathTextBox.Text, " ") == false)
                    {
                        if (pathTextBox.Text != null)
                        {
                            CodeSearcher codeSearch = new CodeSearcher();
                            System.Threading.Thread searchCodeThread = new System.Threading.Thread(codeSearch.searchCode);
                            codeSearch.directory = pathTextBox.Text;
                            searchCodeThread.Start();
                            searchCodeThread.Join();
                            foreach (String line in codeSearch.filenames)
                            {
                                textBox4.Text = textBox4.Text + line;
                            }
                        }
                    }
                }
            });            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {           
            this.text1 = textBox1.Text.TrimEnd(';');
            this.text2 = textBox2.Text.TrimEnd(';');

            //when the user clicks find patterns start searching for patterns
            if (myDataGrid.ItemsSource != null)
            {
               System.Threading.Thread clear = new System.Threading.Thread(this.clearGrid);
               clear.Start();
               //clear.Join();
               System.Threading.Thread.Sleep(1000);
               System.Threading.Thread getDataBegin = new System.Threading.Thread(this.getData);
               getDataBegin.Start();

            }
            System.Threading.Thread getDataBegin2 = new System.Threading.Thread(getData);
            getDataBegin2.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void LeftMouseDown_Event(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
        }
    }
}
