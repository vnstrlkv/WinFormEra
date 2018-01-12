using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDBFNS;
using System.Data;
using PersonalDuty;
using System.Diagnostics;
using System.Net;
using System.IO;
namespace AutoEra
{
    class ForConsole
    {
        public DataTable dutyDT;
        public DataTable personalDT;
        public WDBF WDBF = new WDBF();
        public Clinic clinic = new Clinic();
        public List<Doctors> doctorsList;
        public Doct_shedule doct_shedule;
        public SheduleCollect sheduleCollect;
        DataSet personalDSWithChek;
        DataSet dutyDSWithChek;
        DataTable personalDTWithChek;
        DataTable dutyDTWithChek;

       public void Start()
        {
             personalDSWithChek = new DataSet();
            personalDSWithChek.ReadXml("personalDSWithChek.xml");
             dutyDSWithChek = new DataSet();
            dutyDSWithChek.ReadXml("dutyDSWithChek.xml");
            personalDTWithChek = personalDSWithChek.Tables["Table1"];
             dutyDTWithChek = dutyDSWithChek.Tables["Table1"];

        }

        public void InsertDoc()
        {
            doctorsList = new List<Doctors>();
            sheduleCollect = new SheduleCollect();
            doct_shedule = new Doct_shedule();
            foreach (DataRow data in personalDTWithChek.Rows)
            {
                if (data["Selected"].ToString() == "true")
                {

                    {
                        Doctors tmp = new Doctors();
                        doctorsList.Add(tmp.InsertDoctor(data, clinic));
                    }
                }
            }
    
            sheduleCollect.InsertShedule(dutyDT, personalDTWithChek);

            //Ниже файлы надо выгружать на фтп
           
        }
     //   AND  {" + DateTime.Parse(today.AddDays(14).ToString("MM.dd.yyyy")) + "}
    public bool InitTable()
        {
            personalDT = new DataTable();
            bool flag = false;
            personalDT = WDBF.DBSelect("IND_CODE, FIRST_LAST_NAME", "personal", "");
            if (personalDT != null)
            {
                personalDT.Columns.Add(new DataColumn("ID", typeof(int)));
                personalDT.Columns.Add(new DataColumn("Selected", typeof(bool)));
                int i = 0;
                foreach (DataRow dt in personalDT.Rows)
                {
                    dt["ID"] = i++;
                }
                dutyDT = new DataTable();
                dutyDT.Columns.Add("FIRST_LAST_NAME", typeof(string));
                DateTime today = DateTime.Today;
                DateTime lastday = today.AddDays(14);
              
                var duty = WDBF.DBSelect("ind_code, date, st_time, end_time, client_cod", "duty", " WHERE DATE BETWEEN {" + today.ToString("MM.dd.yyyy") + "} AND  {" + lastday.ToString("MM.dd.yyyy") + "}");
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
                CheckFakeTime();
                flag = true;
            }
            return flag;
        }

        public void CheckFakeTime()
        {
        
            //  printDataTable(dutyDT);

           //   printDataTable(dutyDTWithChek);
          //  Console.Read();
           
        //    for (int j = 0; j < dutyDTWithChek.Rows.Count; j++)
       //         Console.WriteLine("j = {0}    {1}", dutyDTWithChek.Rows[j][5].ToString(), dutyDTWithChek.Rows[j][5]);
         //   Console.Read();

            for (int i = 0; i < dutyDT.Rows.Count; i++)
            {
                for (int j=0;j<dutyDTWithChek.Rows.Count;j++)
                    {
                        if ((dutyDTWithChek.Rows[j]["BusyTime"].ToString() == "true" || dutyDTWithChek.Rows[j]["BusyTime"].ToString() == "True") && (dutyDT.Rows[i]["BusyTime"].ToString() != "true" || dutyDT.Rows[i]["BusyTime"].ToString() != "True"))
                            if (dutyDT.Rows[i]["ind_code"].ToString() == dutyDTWithChek.Rows[j]["ind_code"].ToString())
                                if (DateTime.Parse(dutyDT.Rows[i]["date"].ToString()).ToString("yyyyMMdd") == DateTime.Parse(dutyDTWithChek.Rows[j]["date"].ToString()).ToString("yyyyMMdd"))
                                    if (dutyDT.Rows[i]["st_time"].ToString() == dutyDTWithChek.Rows[j]["st_time"].ToString())
                                        dutyDT.Rows[i]["BusyTime"] = dutyDTWithChek.Rows[j]["BusyTime"];

                    }
            }
    

            //Время, за которое выполнился Ваш код будет храниться в этой переменной:
     
        }

      public  void ToFTP()
        {
            doctorsList[0].OutToCSV(doctorsList, clinic, false);
            doct_shedule.OutToCSV(dutyDT, doctorsList, false);
            sheduleCollect.OutToCSV(false);


            FTPUploadFile("infoclinic_doctors.csv");
            FTPUploadFile("infoclinic_doctschedule.csv");
            FTPUploadFile("infoclinic_schedule.csv");
           

        }
            private void FTPUploadFile(string filename)
        {
            FileInfo fileInf = new FileInfo(filename);
            string uri = "ftp://" + "prodoctorov.ru:2121" + "/" + fileInf.Name;
            FtpWebRequest reqFTP;
            // Создаем объект FtpWebRequest
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + "prodoctorov.ru:2121" + "/" + fileInf.Name));
            // Учетная запись
            reqFTP.Credentials = new NetworkCredential("med-center-era-novosibirsk", "ca2bceeca9771142a908ea808b60331c");
            reqFTP.KeepAlive = false;
            // Задаем команду на закачку
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            // Тип передачи файла
            reqFTP.UseBinary = true;
            // Сообщаем серверу о размере файла
            reqFTP.ContentLength = fileInf.Length;
            // Буффер в 2 кбайт
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            // Файловый поток
            FileStream fs = fileInf.OpenRead();
            try
            {
                Stream strm = reqFTP.GetRequestStream();
                // Читаем из потока по 2 кбайт
                contentLen = fs.Read(buff, 0, buffLength);
                // Пока файл не кончится
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                // Закрываем потоки
                strm.Close();
                fs.Close();
                Console.WriteLine("Выгрузка Успешна в:{0}", DateTime.Now);
                using (var w = new StreamWriter("log.txt", true, Encoding.UTF8))
                {
                    w.WriteLine("{0} Console : Выгрузка успешна ", DateTime.Now );
                    w.Flush();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message, "Ошибка");
                using (var w = new StreamWriter("log.txt", true, Encoding.UTF8))
                {
                    w.WriteLine("{0} Console : ", DateTime.Now+ex.Message);
                    w.Flush();
                }

            }

        }

    
                       

    }
}
