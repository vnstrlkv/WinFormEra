using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PersonalDuty;

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
        public Slot(Shedule shedule)
        {
            DoctorId = shedule.DCODE;
            ClinicId = "1";
            string ST_MIN = shedule.ST_MIN.ToString();
            string END_MIN = shedule.END_MIN.ToString();
            if (shedule.ST_MIN == 0 || shedule.ST_MIN == 5)
                ST_MIN = ST_MIN + "0";
            if (shedule.END_MIN == 0 || shedule.END_MIN == 5)
                END_MIN = END_MIN + "0";
            string s=shedule.DATE.ToString("yyyy-MM-dd")+" "+shedule.ST_HOUR+":"+ST_MIN;
            From=s;
            To=shedule.DATE.ToString("yyyy-MM-dd") +" "+shedule.END_HOUR+":"+END_MIN;
            }
    }

    [XmlRoot(ElementName = "slots")]
    public class Slots
    {
        [XmlElement(ElementName = "slot")]
        public List<Slot> Slot { get; set; }
    }
}
