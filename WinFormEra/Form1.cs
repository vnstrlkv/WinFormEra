using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalDuty;
using System.Windows.Forms;
using WDBFNS;
namespace WinFormEra
{
public partial class Form1 : Form
    {
        public DataTable dutyDT = new DataTable();
        public DataTable personalDT = new DataTable();
        public WDBF WDBF = new WDBF();
        public Form1()
        {
            InitializeComponent();
         
           

            dataGridView1.Anchor = (AnchorStyles.Left | AnchorStyles.Top);
         //   button1.Anchor = AnchorStyles.Top;
              dataGridView1.Dock=(DockStyle.Top);
          //  button1.Dock = DockStyle.Top;
            button1.Text = "Готово";
            button2.Text = "Назад";
            button2.Hide();

            InitTable();
            MainView();





            //         DataTable dutyDT=new DataTable();
         


            //  dataGridView1.AutoGenerateColumns = false;
          //  dataGridView1.EditingPanel.BorderStyle = BorderStyle.Fixed3D;
        //    dataGridView1.DataSource = dutyDT;

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
          
        }

      
 
        private void button1_Click(object sender, EventArgs e)
        {
            DutyCheked();
            button2.Show();
            button1.Hide();
            button3.Show();
      
            //   Form2 next = new Form2();            
            //   this.Hide();
            //   next.Owner = this;
            //   next.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            MainView();
        }

        private void DutyCheked()
        {
            DataTable dutyCheked = new DataTable();
            dutyCheked = dutyDT.Clone();
            var personalDTclone = personalDT.Clone();
            personalDTclone = (DataTable)dataGridView1.DataSource;
            foreach (DataRow data in personalDT.Rows)
            {
                if (data[1].ToString() == "True")
                {
                    DataRow[] foundRows = dutyDT.Select("FIRST_LAST_NAME = '" + data[0].ToString() + "'");
                    if (foundRows != null)
                        for (int i = 0; i < foundRows.Length; i++)
                        {
                            dutyCheked.ImportRow(foundRows[i]);
                        }
                }
            }

            dataGridView1.DataSource = dutyCheked;
        }


        void InitTable()
        {
            personalDT = WDBF.DBSelect("FIRST_LAST_NAME", "personal", "");
            personalDT.Columns.Add(new DataColumn("Selected", typeof(bool)));

            dutyDT.Columns.Add("FIRST_LAST_NAME", typeof(string));
            var duty = WDBF.DBSelect("ind_code, date, st_time, end_time, client_cod", "duty", " WHERE DATE >= {" + DateTime.Parse(DateTime.Today.ToString("MM.dd.yyyy")) + "}");
            dutyDT.Merge(duty);
            dutyDT.Columns.Add("BusyTime", typeof(bool));
            foreach (DataRow data in dutyDT.Rows)
            {
                data[0] = WDBF.DBSelect("FIRST_LAST_NAME", "personal", "WHERE IND_CODE = '" + data[1].ToString() + "'").Rows[0][0].ToString();
                int k;
                if (int.TryParse(data[5].ToString(), out k))
                {
                    data[6] = true;
                }
            }
            dutyDT.Columns.Remove("ind_code");
        }

        void MainView()
        {
            button1.Show();
            button2.Hide();
            button3.Hide();
            dataGridView1.DataSource = personalDT;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            DataSet dataSet = new DataSet();
            DataTable dutyDTclone = dutyDT.Clone(); 
            dutyDTclone= (DataTable)dataGridView1.DataSource;
            dutyDTclone.Columns.Remove("client_cod");
            dataSet.Tables.Add(dutyDTclone);
            // Save to disk
            dataSet.WriteXml(@"C:\MyDataset.xml");

            // Read from disk
       //     dataSet.ReadXml(@"C:\MyDataset.xml");

        }
    }




}
