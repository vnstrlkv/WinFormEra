using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
//using System.Windows.Forms;
using System.Data.OleDb;

namespace WDBFNS
{
  public  class WDBF
    {
        // private OdbcConnection _connection = null;
        private OleDbConnection _connection = null;
        public DataTable Execute(string command)
        {
            DataTable dt = null;
            if (_connection != null)
            {
                try
                {
                    _connection.Open();
                    dt = new DataTable();
                    //    System.Data.Odbc.OdbcCommand oCmd = _connection.CreateCommand();
                    OleDbCommand oCmd = _connection.CreateCommand();
                    oCmd.CommandText = command;
                    dt.Load(oCmd.ExecuteReader());
                    _connection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    using (var w = new StreamWriter("log.txt", true, Encoding.UTF8))
                    {
                        w.WriteLine("{0} WDBF : ", DateTime.Now + e.Message);
                        w.Flush();
                    }
                }
            }
            return dt;
        }
        public DataTable DBSelect(string command,string dbpath, string command2)
        {
         //   DateTime tm = DateTime.Parse(DateTime.Today.ToString("dd.MM.yyyy"));
        //    MessageBox.Show(DateTime.Today.ToString("dd.MM.yyyy"));
            return Execute("SELECT " + command + " FROM " + dbpath + " " + command2);
        }

        public WDBF()
        {
            //  this._connection = new System.Data.Odbc.OdbcConnection();
            this._connection = new OleDbConnection();
            _connection.ConnectionString = @"Provider=vfpoledb;Data Source=D:\PHOENIX\dbc\phoenix.dbc;Collating Sequence=machine; Exclusive=No";
        }
    }
}

