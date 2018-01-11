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
  public  class Clinic  // список клиник
    {
        public int FILID { get; set; }
        string FULLNAME { get; set; }
        string DIS_IN_SHEDULE { get; set; }
        public int VIEWINWEB { get; set; }

      public  Clinic()
        {
            FILID = 1;
            FULLNAME="Медицинский центр \"ЭРА\"";
            DIS_IN_SHEDULE = null;
            VIEWINWEB = 0;
        }
      public string OutString()
        {
            return string.Format("{0};{1};{2};{3}", FILID, FULLNAME, DIS_IN_SHEDULE, VIEWINWEB);
        }
      public void OutToCSV ()
        {
            using (var w = new StreamWriter("infoclinic_clinics.csv", false, Encoding.UTF8))
            {  
               w.WriteLine(OutString());
               w.Flush();                
            }
        }
    }
 public   class Doctors  // список докторов
    {
        public string DCODE { get; set; }
        string FULLNAME { get; set; }
        int FILIAL { get; set; }
        int DEPNUM { get; set; }
        int CHAIR { get; set; }
        int VIEWINSCHED { get; set; }
        int STDTYPE { get; set; }
        string DOCTPOST { get; set; }
        string VIEWINWEB { get; set; }

        public Doctors InsertDoctor (DataRow row, Clinic clinic)
        {
            if (row["ind_code"] != null)
            {
                if (row["FIRST_LAST_NAME"] != null)
                {
                    DCODE = row["ind_code"].ToString();
                    FULLNAME = row["FIRST_LAST_NAME"].ToString();
                    FILIAL = clinic.FILID;
                    DEPNUM = 0;
                    CHAIR = 0;
                    VIEWINSCHED = 1;
                    STDTYPE = 1;
                    DOCTPOST = null;
                    VIEWINWEB = null;
                }
            }
            return this;
        }
        string OutString()
        {
            string outputstr = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}"
                               ,DCODE.Trim()
                               ,FULLNAME.Trim()
                               ,FILIAL
                               ,DEPNUM
                               ,CHAIR
                               ,VIEWINSCHED
                               ,STDTYPE
                               ,DOCTPOST
                               ,VIEWINWEB
                               );

            return outputstr;
        }
        string OutString(Doctors doc)
        {
            string outputstr = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}"
                               , doc.DCODE.Trim()
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

            using (var w = new StreamWriter("infoclinic_clinics.csv", flagwrite, Encoding.UTF8))
            {
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

            using (var w = new StreamWriter("infoclinic_clinics.csv", flagwrite, Encoding.UTF8))
            {
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

        public Date_shedule GetDateTime (DataRow[] date)
        {
            this.DATE = DateTime.Parse(date[0][2].ToString());
            List <DateTime> tmpStTime = new List<DateTime>();
            List<DateTime> tmpEndTime = new List<DateTime>();
            foreach (DataRow rowDate in date)
            {
                string tmp = DateTime.Parse(rowDate["date"].ToString()).ToString("dd.MM.yyyy") + " " + rowDate["ST_TIME"].ToString();
                tmpStTime.Add(DateTime.Parse(tmp));
                tmp = DateTime.Parse(rowDate["date"].ToString()).ToString("dd.MM.yyyy") + " " + rowDate["END_TIME"].ToString();
                tmpEndTime.Add(DateTime.Parse(tmp));
            }
           string tm2p = tmpStTime.Min<DateTime>().ToString("HH");
            ST_HOUR =int.Parse(tm2p);
            ST_MIN = int.Parse(tmpStTime.Min<DateTime>().ToString("mm"));
            END_HOUR = int.Parse(tmpStTime.Max<DateTime>().ToString("HH"));
            END_MIN = int.Parse(tmpStTime.Max<DateTime>().ToString("mm"));
            var tmpinteral = tmpEndTime[0] - tmpStTime[0];
            INTERVAL = int.Parse(tmpinteral.Minutes.ToString());
            return this;
        }
     public string OutString ()
        {
            return (DATE.ToString() + ";" + ST_HOUR + ";" + ST_MIN + ";" + END_HOUR + ";" + END_MIN + ";" + INTERVAL);
        }

        
    }
 public   class Doct_shedule  // расписание докторов
    {
        string DCODE { get; set; }
        string CHAIR { get; set; }

        Collection<Date_shedule> DATElist { get; set; }

        int FILID { get; set; }

        public void InsertDoctShedule(DataTable data, Doctors doctor)
        {        
            DCODE = doctor.DCODE;
            CHAIR = null;
            FILID = 0;
            DataRow[] selectedDoctorRow = data.Select("ind_code = '"+ DCODE +"'");
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
                    DataRow[] selectedDateRow = selectedDoctorTable.Select("date = '" + day+"'");
                    Date_shedule tmpDate = new Date_shedule();
                    DATElist.Add(tmpDate.GetDateTime(selectedDateRow));
                }
            }
        }

      public  string OutString ()
        {
            string outputstr=null;
            foreach (Date_shedule ds in DATElist)
            {
                outputstr += DCODE + ";" + CHAIR + ";" + ds.OutString() + ";" + FILID + "\n\n";
            }
            return outputstr;

        }

        public void OutToCSV(DataTable dutyTable, List<Doctors> doctors, bool flagwrite)
        {
            /*запись в csv*/

            using (var w = new StreamWriter("infoclinic_doctshedule.csv", flagwrite, Encoding.UTF8))
            {
                  foreach (Doctors doctor in doctors)               
                {                    
                    this.InsertDoctShedule(dutyTable, doctor);
                    if (DATElist.Count != 0)
                        w.WriteLine(this.OutString());
                    w.Flush();
                }
            }
            /*запись в csv*/
        }

    }


    public class SheduleCollect 
    {
        List<Shedule> sheduleList = new List<Shedule>();

        public void InsertShedule(DataTable dutyWithChekTABLE)
        {
            foreach (DataRow duty in dutyWithChekTABLE.Rows)
            {
                Shedule tmp = new Shedule();
                if(duty["BusyTime"].ToString()=="True")
                sheduleList.Add(tmp.AddShedule(duty));
            }
        }

        public void OutToCSV(bool flagwrite)
        {
            using (var w = new StreamWriter("infoclinic_shedule.csv", flagwrite, Encoding.UTF8))
            {
                if (sheduleList.Count!=0)
                foreach (Shedule shedule in sheduleList)
                {
                        if (shedule != null)
                        {
                            string outputstr = shedule.DCODE +
                          ";" + shedule.DATE +
                          ";" + shedule.ST_HOUR +
                          ";" + shedule.ST_MIN +
                          ";" + shedule.END_HOUR +
                          ";" + shedule.END_MIN + "\n\n";
                            w.WriteLine(outputstr);
                        }
                        
                    w.Flush();
                }
            }
        }
    }

    class Shedule  //занятое время
    {
       public string DCODE { get; set; }
       public  DateTime DATE { get; set; }
       public   int ST_HOUR { get; set; }
       public   int ST_MIN { get; set; }
       public   int END_HOUR { get; set; }
       public   int END_MIN { get; set; }

           

        public Shedule AddShedule(DataRow dutyRow)
        {
            if (dutyRow["BusyTime"].ToString()=="True")
            {
                DCODE =dutyRow["IND_CODE"].ToString();
                DATE = DateTime.Parse(dutyRow["DATE"].ToString());

                //разбитие времени
                DateTime tmpStTime = DateTime.Parse( DateTime.Parse(dutyRow["date"].ToString()).ToString("dd.MM.yyyy") + " " + dutyRow["ST_TIME"].ToString());               
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
