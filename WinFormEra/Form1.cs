using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalDuty;
using System.IO;
using System.Windows.Forms;
using WDBFNS;
namespace WinFormEra
{
public partial class Form1 : Form
    {
        public DataTable dutyDT = new DataTable();
        public DataTable personalDT = new DataTable();
        public WDBF WDBF = new WDBF();
        public Clinic clinic = new Clinic();
        public List <Doctors> doctors = new List <Doctors>();
        public Doct_shedule doct_shedule = new Doct_shedule();
        public SheduleCollect sheduleCollect = new SheduleCollect();
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
         //   clinic.OutToCSV();
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
            doctors = new List<Doctors>();

            DataTable dutyChekList = new DataTable();
            dutyChekList = dutyDT.Clone();
            var personalDTclone = personalDT.Clone();
            personalDTclone = (DataTable)dataGridView1.DataSource;
            foreach (DataRow data in personalDTclone.Rows)
            {
                if (data["Selected"].ToString() == "True")
                {
                   
                    DataRow[] foundRows = dutyDT.Select("FIRST_LAST_NAME = '" + data["FIRST_LAST_NAME"].ToString() + "'");
                    if (foundRows.Length != 0)
                    {
                        Doctors tmp = new Doctors();
                        doctors.Add(tmp.InsertDoctor(data, clinic));

                        for (int i = 0; i < foundRows.Length; i++)
                        {
                            dutyChekList.ImportRow(foundRows[i]);
                        }
                    }
                }
            }

            doctors[0].OutToCSV(doctors, clinic, false); // сюда вставить  добавление доктора в класс для выгрузки в csv
            doct_shedule.OutToCSV(dutyDT, doctors, false);
                                                           
            dataGridView1.DataSource = dutyChekList;
        }


        void InitTable()
        {
            
            personalDT = WDBF.DBSelect("IND_CODE, FIRST_LAST_NAME", "personal", "");
            personalDT.Columns.Add(new DataColumn("ID", typeof(int)));
            personalDT.Columns.Add(new DataColumn("Selected", typeof(bool)));
            int i = 0;
            foreach (DataRow dt in personalDT.Rows)
            {
                dt["ID"] = i++;
            }

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
            DataTable dutyDTwithChek = dutyDT.Clone(); 
            dutyDTwithChek = (DataTable)dataGridView1.DataSource;

            sheduleCollect.InsertShedule(dutyDTwithChek, personalDT);
            sheduleCollect.OutToCSV(false);
            
            // doct_shedule.Insert_doct_shedule(dutyDTclone, doctors);
           // doct_shedule.OutCSV();

           // dutyDTclone.Columns.Remove("client_cod");
            //dataSet.Tables.Add(dutyDTclone);
            // Save to disk
            //dataSet.WriteXml(@"C:\MyDataset.csv");

            // Read from disk
       //     dataSet.ReadXml(@"C:\MyDataset.xml");

        }
    }




}
