using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using WDBFNS;
namespace WinFormEra
{
public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            WDBF WDBF = new WDBF();
            var dt = WDBF.GetAll("C:\\123\\personal.dbf");
            dataGridView1.DataSource = dt;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
