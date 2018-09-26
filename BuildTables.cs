using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Pat
{
    class BuildTables
    {
        /// <summary>
        /// build data set tables
        /// </summary>
        public int buildComparisonRec(Matches p_mcol, DataGrid p_dataGrid)
        {
            int colcount = 0;
            DataTable dt = new DataTable();
            DataRow dr = null;

            //builds subject dataset
            for (int b = 0; b < p_mcol.row[0].column.Count; b++)
            {
                dt.Columns.Add(p_mcol.row[0].column[b].Value, typeof(String));
                colcount++;
            }

            //builds comparison dataset
            for (int i = 0; i < p_mcol.row.Count; i++)
            {
                dr = dt.NewRow();

                for (int b = 0; b < p_mcol.row[i].column.Count; b++)
                {
                    dr[b] = p_mcol.row[i].column[b].Value;
                }

                dt.Rows.Add(dr);

                p_dataGrid.ItemsSource = dt.DefaultView;
            }
            return colcount;
        }
    }
}
