using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
    }

    [XmlRoot(ElementName = "slots")]
    public class Slots
    {
        [XmlElement(ElementName = "slot")]
        public List<Slot> Slot { get; set; }
    }
}
