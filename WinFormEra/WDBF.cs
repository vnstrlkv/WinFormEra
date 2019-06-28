using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;

namespace WDBFNS
{
    public class WDBF
    {
        // private OdbcConnection _connection = null;
        private OleDbConnection _connection = null;




        public DataTable Execute(string command)
        {
           DataTable t= _connection.GetSchema();
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
                    MessageBox.Show(e.Message);
                }
            }
            //return dt;
            return t;
        }
        public DataTable DBSelect(string command, string dbpath, string command2)
        {
            //   DateTime tm = DateTime.Parse(DateTime.Today.ToString("dd.MM.yyyy"));
            //    MessageBox.Show(DateTime.Today.ToString("dd.MM.yyyy"));
            return Execute("SELECT " + command + " FROM " + dbpath + " " + command2);
        }

        public DataTable DBSelectCustom(string command)
        {
        
            return Execute(command);
        }

        public WDBF()
        {
            //  this._connection = new System.Data.Odbc.OdbcConnection();
            this._connection = new OleDbConnection();
            string conf;
            using (StreamReader w = new StreamReader("conf.txt"))
                conf = w.ReadLine();

            _connection.ConnectionString = @"Provider=vfpoledb;" + conf + ";Collating Sequence=machine; Exclusive=No";
        }
    }
}

