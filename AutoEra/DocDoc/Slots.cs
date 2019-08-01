using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PersonalDuty;
using System.Data;

namespace AutoEra.DocDoc
{
    [XmlRoot(ElementName = "slot")]
    public class Slot
    {
        [XmlElement(ElementName = "doctorId")]
        public string DoctorId { get; set; }
        [XmlElement(ElementName = "clinicId")]
        public string ClinicId { get; set; }
        [XmlElement(ElementName = "from")]
        public string From { get; set; }
        [XmlElement(ElementName = "to")]
        public string To { get; set; }
        [XmlElement(ElementName = "interval")]
        public string Interval { get; set; }

        public Slot() { }
        public Slot(DataRow shedule)
        {
           
            DoctorId = shedule["ind_code"].ToString();
            ClinicId = "1";
            string ST_MIN = shedule["st_time"].ToString();
            string END_MIN = shedule["end_time"].ToString();
            if (shedule["st_time"].ToString() == "0" || shedule["st_time"].ToString() == "5")
                ST_MIN = ST_MIN + "0";
            if (shedule["end_time"].ToString() == "0" || shedule["end_time"].ToString() == "5")
                END_MIN = END_MIN + "0";
            string s= DateTime.Parse(shedule["DATE"].ToString()).ToString("yyyy-MM-dd") + " "+ST_MIN;
            From=s;
            To= DateTime.Parse(shedule["DATE"].ToString()).ToString("yyyy-MM-dd") + " "+END_MIN;
           var  xInterval = (DateTime.Parse(To) - DateTime.Parse(s));
            Interval = xInterval.ToString("mm");
            if (Interval == "0")
                Interval = "60";

            }
    }

    [XmlRoot(ElementName = "slots")]
    public class Slots
    {
        [XmlElement(ElementName = "slot")]
        public List<Slot> Slot { get; set; }
    }
}
