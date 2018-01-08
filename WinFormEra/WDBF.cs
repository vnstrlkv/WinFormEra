using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using System.Data.OleDb;

namespace WDBFNS
{
    class WDBF
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
                    MessageBox.Show(e.Message);
                }
            }
            return dt;
        }
        public DataTable GetAll(string dbpath)
        {
            return Execute("SELECT FIRST_LAST_NAME FROM " + dbpath);
        }

        public WDBF()
        {
            //  this._connection = new System.Data.Odbc.OdbcConnection();
            this._connection = new OleDbConnection();
            _connection.ConnectionString = @"Provider=vfpoledb;Data Source=C:\123\phoenix.dbc;Collating Sequence=machine; Exclusive=No";
        }
    }
}

