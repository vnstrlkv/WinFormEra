﻿using System;
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
using System.Diagnostics;
using System.Threading;
using WDBFNS;
namespace WinFormEra
{
    public partial class Form1 : Form
    {
        public DataTable dutyDT = new DataTable();
        public DataTable personalDT = new DataTable();
        public WDBF WDBF = new WDBF();
        public Clinic clinic = new Clinic();
        public List<Doctors> doctors = new List<Doctors>();
        public Doct_shedule doct_shedule = new Doct_shedule();
        public SheduleCollect sheduleCollect = new SheduleCollect();
        public Form1()
        {
            InitializeComponent();



            dataGridView1.Anchor = (AnchorStyles.Left | AnchorStyles.Top);
            //   button1.Anchor = AnchorStyles.Top;
            dataGridView1.Dock = (DockStyle.Top);
            //  button1.Dock = DockStyle.Top;
            button1.Text = "Далее";
            button2.Text = "Назад";
            button3.Text = "Готово";
            button1.Hide();
            button3.Hide();
            button2.Hide();

            BackgroundWorker backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            backgroundWorker1.RunWorkerAsync();

        }




        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //заполняем datagridview1 инфой из базы
            InitTable();
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //после выполнения заполнения datagridview1 закроем финормационное окошко
            MainView();
        }





        void MainView()
        {

            button2.Hide();
            button3.Hide();
            dataGridView1.DataSource = personalDT;
            dataGridView1.Show();
            button1.Show();
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }



        private void button1_Click(object sender, EventArgs e)
        {
            DutyCheked();
            button2.Show();
            button1.Hide();
            button3.Text = "Готово";
            button3.Show();


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


            personalDT = personalDTclone.Copy();


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

            ChekLastPersonal();

            dutyDT.Columns.Add("FIRST_LAST_NAME", typeof(string));
            DateTime today = DateTime.Today;
            DateTime lastday = today.AddDays(8);

            var duty = WDBF.DBSelect("ind_code, date, st_time, end_time, client_cod", "duty", " WHERE DATE BETWEEN {" + today.ToString("MM.dd.yyyy") + "} AND  {" + lastday.ToString("MM.dd.yyyy") + "}");
            dutyDT.Merge(duty);
            dutyDT.Columns.Add("BusyTime", typeof(bool));
            foreach (DataRow data in dutyDT.Rows)
            {
                data[0] = WDBF.DBSelect("FIRST_LAST_NAME", "personal", "WHERE IND_CODE = '" + data[1].ToString() + "'").Rows[0][0].ToString();
                string s = DateTime.Parse(data["DATE"].ToString()).ToString("dd.MM.yyyy") + " " + data["st_time"].ToString();
                if (DateTime.Parse(s) <= DateTime.Now)
                {
                    data[6] = true;
                    continue;
                }
                int k;
                if (int.TryParse(data[5].ToString(), out k))
                {
                    data[6] = true;
                }
            }

        }

        void ChekLastPersonal()
        {

            try
            {
                DataSet personalDSWithChek = new DataSet();
                personalDSWithChek.ReadXml("personalDSWithChek.xml");
                DataTable personalDTWithChek = personalDSWithChek.Tables["Table1"];

                for (int i = 0; i < personalDT.Rows.Count; i++)
                    for (int j = 0; j < personalDTWithChek.Rows.Count; j++)
                    {

                        if (personalDT.Rows[i]["First_last_name"].ToString() == personalDTWithChek.Rows[j]["First_last_name"].ToString())

                        {
                            personalDT.Rows[i]["Selected"] = personalDTWithChek.Rows[j]["Selected"];
                            break;
                        }
                    }

            }
            catch (Exception ex)
            {

            }

        }


        private void button3_Click(object sender, EventArgs e)
        {


            DataTable dutyDTWithChek = dutyDT.Clone();
            dutyDTWithChek = (DataTable)dataGridView1.DataSource;

            sheduleCollect.InsertShedule(dutyDTWithChek, personalDT);


            DataSet dutyDSWithChek = new DataSet();
            dutyDSWithChek.Tables.Add(dutyDTWithChek);
            // Save to disk
            dutyDSWithChek.WriteXml("dutyDSWithChek.xml");
            dutyDSWithChek.WriteXml(@"\\192.168.1.100\reg\Out\dutyDSWithChek.xml");

            DataSet personalDSWithChek = new DataSet();
            personalDSWithChek.Tables.Add(personalDT);
            personalDSWithChek.WriteXml("personalDSWithChek.xml");
            personalDSWithChek.WriteXml(@"\\192.168.1.100\reg\Out\personalDSWithChek.xml");

            //   Process prc = new Process(); // Объявляем объект
            //    prc.StartInfo.FileName = "AutoEra.exe"; // Полное имя файла, включая путь к файлу, к примеру "C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe", не забудь про собаку "@", что бы в строку слежи можно было записывать
            //   prc.Start(); // Запускаем процесс
            this.Close();


            // dutyDTclone.Columns.Remove("client_cod");
            //dataSet.Tables.Add(dutyDTclone);
            // Save to disk
            //dataSet.WriteXml(@"C:\MyDataset.csv");

            // Read from disk
            //     dataSet.ReadXml(@"C:\MyDataset.xml");

        }
    }




}
