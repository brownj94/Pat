using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Windows.Controls;

namespace Pat
{
    class DBAccess
    {
        private Stats stats;

        private static OracleConnection setConnection()
        {

            string database = ConfigurationManager.AppSettings["Database"];
            string schema = ConfigurationManager.AppSettings["Schema"];

            return OracleConnections.GetOracleConnection(database, schema);
        }

        public Stats Stats
        {
            get
            {
               return this.stats;
            }

        }

        public void Begin()
        {
            
        }

        /// <summary>
        /// Calls Oracle stored procedures that will do the actual dataset comparisons
        /// </summary>
        public void FindPatterns(string q1, string q2, DataGrid myDataGrid, DataGrid myDataGrid2)
        {
            int colcount;

                OracleConnection con = setConnection();

                if (con.State.ToString() == "Closed")
                {
                    con.Open();
                }

                OracleCommand cmd = new OracleCommand("PAT2.find_patterns", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.BindByName = true;

                OracleParameter sql_stmt = new OracleParameter();
                sql_stmt.OracleDbType = OracleDbType.Varchar2;
                sql_stmt.ParameterName = "sql_stmt";
                sql_stmt.Direction = ParameterDirection.Input;
                sql_stmt.Value = q1;
                cmd.Parameters.Add(sql_stmt);

                OracleParameter sql_stmt2 = new OracleParameter();
                sql_stmt2.OracleDbType = OracleDbType.Varchar2;
                sql_stmt2.ParameterName = "sql_stmt2";
                sql_stmt2.Direction = ParameterDirection.Input;
                sql_stmt2.Value = q2;
                cmd.Parameters.Add(sql_stmt2);

                OracleParameter collec1 = new OracleParameter();
                collec1.OracleDbType = OracleDbType.Clob;
                collec1.ParameterName = "collec1";
                collec1.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(collec1);

                OracleParameter collec2 = new OracleParameter();
                collec2.OracleDbType = OracleDbType.Clob;
                collec2.ParameterName = "collec2";
                collec2.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(collec2);

                cmd.ExecuteNonQuery();

                OracleClob xmldata = (OracleClob)cmd.Parameters["collec1"].Value;
                string convert = (string)xmldata.Value;

                OracleClob xmldata2 = (OracleClob)cmd.Parameters["collec2"].Value;
                string convert2 = (string)xmldata2.Value;

                //turn xml file that represents subject record from Oracle into C# objects
                Matches mcol = new Matches();
                mcol = Deserializer.deserialize(convert, mcol);
                DataGrid recDatagrid = myDataGrid2;

                //build tables using searialized xml objects 
                BuildTables bTables1 = new BuildTables();
                bTables1.buildComparisonRec(mcol, recDatagrid);

                //turn xml file that represents comaprison dataset from Oracle into C# objects
                Matches mcol2 = new Matches();
                mcol2 = Deserializer.deserialize(convert2, mcol2);
                DataGrid recDatagrid2 = myDataGrid;
                this.stats = new Stats(mcol2);

                //build tables using searialized xml objects 
                BuildTables bTables2 = new BuildTables();
                colcount = bTables2.buildComparisonRec(mcol2, recDatagrid2);

                //change color of cells based on if the cell has a match with the subject record
                ColorCode.colorCode(colcount, mcol, recDatagrid2);
                con.Close();

        }   
    }
}
