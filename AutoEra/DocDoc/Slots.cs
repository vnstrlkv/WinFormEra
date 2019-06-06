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
        public DateTime From { get; set; }
        [XmlElement(ElementName = "to")]
        public DateTime To { get; set; }
        [XmlElement(ElementName = "interval")]
        public string Interval { get; set; }

        public Slot(){}
        public Slot (Shedule shedule)
            {
            DoctorId=shedule.DCODE;
            ClinicId="1";
            string s=shedule.DATE.ToString("dd.MM.yyyy")+" "+shedule.ST_HOUR+":"+shedule.ST_MIN+":00";
            From=DateTime.Parse(s);
            To=DateTime.Parse(shedule.DATE.ToString("dd.MM.yyyy")+" "+shedule.END_HOUR+":"+shedule.END_MIN+":00");
            }
    }

    [XmlRoot(ElementName = "slots")]
    public class Slots
    {
        [XmlElement(ElementName = "slot")]
        public List<Slot> Slot { get; set; }
    }
}
