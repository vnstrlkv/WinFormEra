using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;

namespace PersonalDuty
{
    public class Clinic  // список клиник
    {
        public int FILID { get; set; }
        string FULLNAME { get; set; }
        string DIS_IN_SHEDULE { get; set; }
        public int VIEWINWEB { get; set; }

        public Clinic()
        {
            FILID = 1;
            FULLNAME = "Медицинский центр \"ЭРА\"";
            DIS_IN_SHEDULE = null;
            VIEWINWEB = 0;
        }
        public string OutString()
        {
            return string.Format("{0};{1};{2};{3}", FILID, FULLNAME, DIS_IN_SHEDULE, VIEWINWEB);
        }
        public void OutToCSV()
        {
            using (var w = new StreamWriter("infoclinic_clinics.csv", false, Encoding.UTF8))
            {
                w.WriteLine(OutString());
                w.Flush();
            }
        }
    }
    public class Doctors  // список докторов
    {
        public int ID { get; set; }
        public string DCODE { get; set; }
        string FULLNAME { get; set; }
        int FILIAL { get; set; }
        string DEPNUM { get; set; }
        string CHAIR { get; set; }
        int VIEWINSCHED { get; set; }
        int STDTYPE { get; set; }
        string DOCTPOST { get; set; }
        string VIEWINWEB { get; set; }

        public Doctors InsertDoctor(DataRow row, Clinic clinic)
        {
            if (row["ind_code"] != null)
            {
                if (row["FIRST_LAST_NAME"] != null)
                {
                    ID = int.Parse(row["ID"].ToString());
                    DCODE = row["ind_code"].ToString();
                    FULLNAME = row["FIRST_LAST_NAME"].ToString();
                    FILIAL = clinic.FILID;
                    DEPNUM = null;
                    CHAIR = null;
                    VIEWINSCHED = 1;
                    STDTYPE = 13;
                    DOCTPOST = null;
                    VIEWINWEB = null;
                }
            }
            return this;
        }
        string OutString()
        {
            string outputstr = string.Format("1000{0};{1};{2};{3};{4};{5};{6};{7};{8}"
                               , ID
                               , FULLNAME.Trim()
                               , FILIAL
                               , DEPNUM
                               , CHAIR
                               , VIEWINSCHED
                               , STDTYPE
                               , DOCTPOST
                               , VIEWINWEB
                               );

            return outputstr;
        }
        string OutString(Doctors doc)
        {
            string outputstr = string.Format("1000{0};{1};{2};{3};{4};{5};{6};{7};{8}"
                               , doc.ID
                               , doc.FULLNAME.Trim()
                               , doc.FILIAL
                               , doc.DEPNUM
                               , doc.CHAIR
                               , doc.VIEWINSCHED
                               , doc.STDTYPE
                               , doc.DOCTPOST
                               , doc.VIEWINWEB
                               );

            return outputstr;
        }
        public void OutToCSV(DataTable personalTable, Clinic clinic, bool flagwrite)
        {
            /*запись в csv*/

            using (var w = new StreamWriter("infoclinic_doctors.csv", flagwrite, Encoding.UTF8))
            {
                w.WriteLine("\"DCODE\";\"FULLNAME\";\"FILIAL\";\"DEPNUM\";\"CHAIR\";\"VIEWINSCHED\";\"STDTYPE\";\"DOCTPOST\";\"VIEWINWEB\"");
                w.Flush();
                foreach (DataRow personal in personalTable.Rows)
                {
                    this.InsertDoctor(personal, clinic);
                    w.WriteLine(this.OutString());
                    w.Flush();
                }
            }
            /*запись в csv*/
        }
        public void OutToCSV(List<Doctors> doctors, Clinic clinic, bool flagwrite)
        {
            /*запись в csv*/

            using (var w = new StreamWriter("infoclinic_doctors.csv", flagwrite, Encoding.UTF8))
            {
                w.WriteLine("\"DCODE\";\"FULLNAME\";\"FILIAL\";\"DEPNUM\";\"CHAIR\";\"VIEWINSCHED\";\"STDTYPE\";\"DOCTPOST\";\"VIEWINWEB\"");
                w.Flush();
                foreach (Doctors personal in doctors)
                {
                    // this.InsertDoctor(doctors, clinic);
                    w.WriteLine(this.OutString(personal));
                    w.Flush();
                }
            }
            /*запись в csv*/
        }
    }
    public class Date_shedule
    {
        DateTime DATE { get; set; }
        int ST_HOUR { get; set; }
        int ST_MIN { get; set; }
        int END_HOUR { get; set; }
        int END_MIN { get; set; }
        int INTERVAL { get; set; }

        public Date_shedule GetDateTime(DataRow[] date)
        {
            this.DATE = DateTime.Parse(date[0][2].ToString());
            List<DateTime> tmpStTime = new List<DateTime>();
            List<DateTime> tmpEndTime = new List<DateTime>();
            foreach (DataRow rowDate in date)
            {
                string tmp = DateTime.Parse(rowDate["date"].ToString()).ToString("dd.MM.yyyy") + " " + rowDate["ST_TIME"].ToString();
                tmpStTime.Add(DateTime.Parse(tmp));
                tmp = DateTime.Parse(rowDate["date"].ToString()).ToString("dd.MM.yyyy") + " " + rowDate["END_TIME"].ToString();
                tmpEndTime.Add(DateTime.Parse(tmp));
            }
            string tm2p = tmpStTime.Min<DateTime>().ToString("HH");
            ST_HOUR = int.Parse(tm2p);
            ST_MIN = int.Parse(tmpStTime.Min<DateTime>().ToString("mm"));
            END_HOUR = int.Parse(tmpEndTime.Max<DateTime>().ToString("HH"));
            END_MIN = int.Parse(tmpEndTime.Max<DateTime>().ToString("mm"));
            var tmpinteral = tmpEndTime[0] - tmpStTime[0];
            if (int.Parse(tmpinteral.Minutes.ToString()) == 0 || int.Parse(tmpinteral.Minutes.ToString()) >= 59)
                INTERVAL = 30;
            else
                INTERVAL = int.Parse(tmpinteral.Minutes.ToString());
            return this;
        }
        public string OutString(Doct_shedule ds)
        {
            return ("1000" + ds.ID + ";" + ds.CHAIR + ";" + DATE.ToString("yyyy-MM-dd HH:mm:ss.fff") + ";" + ST_HOUR + ";" + ST_MIN + ";" + END_HOUR + ";" + END_MIN + ";" + INTERVAL + ";" + ds.FILID);
        }


    }
    public class Doct_shedule  // расписание докторов
    {
        public int ID;
        string DCODE { get; set; }
        public string CHAIR { get; set; }

        Collection<Date_shedule> DATElist { get; set; }

        public int FILID { get; set; }

        public void InsertDoctShedule(DataTable data, Doctors doctor)
        {
            ID = doctor.ID;
            DCODE = doctor.DCODE;
            CHAIR = null;
            FILID = 1;
            DataRow[] selectedDoctorRow = data.Select("ind_code = '" + DCODE + "'");
            DataTable selectedDoctorTable = data.Clone();

            DATElist = new Collection<Date_shedule>();
            List<DateTime> alldate = new List<DateTime>();

            foreach (DataRow row in selectedDoctorRow)
            {
                DateTime tmp = DateTime.Parse(row["DATE"].ToString());
                alldate.Add(tmp);
                selectedDoctorTable.ImportRow(row);
            }
            if (alldate.Count != 0)
            {

                IEnumerable<DateTime> chekedDate = alldate.Distinct();
                foreach (DateTime day in chekedDate)
                {
                    DataRow[] selectedDateRow = selectedDoctorTable.Select("date = '" + day + "'");
                    Date_shedule tmpDate = new Date_shedule();
                    DATElist.Add(tmpDate.GetDateTime(selectedDateRow));
                }
            }
        }

        public string OutString()
        {
            string outputstr = null;
            foreach (Date_shedule ds in DATElist)
            {
                outputstr = ds.OutString(this);
            }
            return outputstr;

        }

        public void OutToCSV(DataTable dutyTable, List<Doctors> doctors, bool flagwrite)
        {
            /*запись в csv*/

            using (var w = new StreamWriter("infoclinic_doctschedule.csv", flagwrite, Encoding.UTF8))
            {
                //  w.WriteLine("-1;2017-11-10 00:00:00.0000;16;50;17;10");
                // w.WriteLine("DATE;ST_HOUR;_ST_MIN;END_HOUR;END_MIN");
                //  w.Flush();
                w.WriteLine("-1;;2017-11-10 00:00:00.0000;16;50;17;10;0;0");
                w.Flush();


                foreach (Doctors doctor in doctors)
                {
                    this.InsertDoctShedule(dutyTable, doctor);
                    if (DATElist.Count != 0)
                    {
                        foreach (Date_shedule ds in DATElist)
                        {
                            string outputstr = null;
                            outputstr = ds.OutString(this);
                            w.WriteLine(outputstr);
                            w.Flush();
                        }

                    }

                }
            }
            /*запись в csv*/
        }

    }


    public class SheduleCollect
    {
        List<Shedule> sheduleList;

        public void InsertShedule(DataTable dutyWithChekTABLE, DataTable personalDT)
        {
            sheduleList = new List<Shedule>();
            foreach (DataRow duty in dutyWithChekTABLE.Rows)
            {
                Shedule tmp = new Shedule();
                if (duty["BusyTime"].ToString() == "True")
                    sheduleList.Add(tmp.AddShedule(duty, personalDT));
            }
        }

        public void OutToCSV(bool flagwrite)
        {
            using (var w = new StreamWriter("infoclinic_schedule.csv", flagwrite, Encoding.UTF8))
            {
                w.WriteLine("-1;2017-11-10 00:00:00.0000;16;50;17;10");
                // w.WriteLine("DATE;ST_HOUR;_ST_MIN;END_HOUR;END_MIN");
                w.Flush();
                if (sheduleList.Count != 0)
                    foreach (Shedule shedule in sheduleList)
                    {
                        if (shedule != null)
                        {
                            string outputstr = "1000" + shedule.ID +
                          ";" + shedule.DATE.ToString("yyyy-MM-dd HH:mm:ss.fff") +
                          ";" + shedule.ST_HOUR +
                          ";" + shedule.ST_MIN +
                          ";" + shedule.END_HOUR +
                          ";" + shedule.END_MIN;
                            w.WriteLine(outputstr);
                        }

                        w.Flush();
                    }
            }
        }
    }

    class Shedule  //занятое время
    {
        public int ID { get; set; }
        public string DCODE { get; set; }
        public DateTime DATE { get; set; }
        public int ST_HOUR { get; set; }
        public int ST_MIN { get; set; }
        public int END_HOUR { get; set; }
        public int END_MIN { get; set; }



        public Shedule AddShedule(DataRow dutyRow, DataTable personalDT)
        {
            if (dutyRow["BusyTime"].ToString() == "True")
            {
                DataRow[] tmp = personalDT.Select("IND_CODE = '" + dutyRow["IND_CODE"].ToString() + "'");
                if (tmp.Length > 0)
                    ID = int.Parse(tmp[0]["ID"].ToString());
                DCODE = dutyRow["IND_CODE"].ToString();
                DATE = DateTime.Parse(dutyRow["DATE"].ToString());

                //разбитие времени
                DateTime tmpStTime = DateTime.Parse(DateTime.Parse(dutyRow["date"].ToString()).ToString("dd.MM.yyyy") + " " + dutyRow["ST_TIME"].ToString());
                DateTime tmpEndTime = DateTime.Parse(DateTime.Parse(dutyRow["date"].ToString()).ToString("dd.MM.yyyy") + " " + dutyRow["END_TIME"].ToString());
                ST_HOUR = int.Parse(tmpStTime.ToString("HH"));
                ST_MIN = int.Parse(tmpStTime.ToString("mm"));
                END_HOUR = int.Parse(tmpEndTime.ToString("HH"));
                END_MIN = int.Parse(tmpEndTime.ToString("mm"));

                return this;
            }
            return null;
        }
    }
}
