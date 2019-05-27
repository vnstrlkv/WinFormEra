using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace AutoEra.DocDoc
{
    [XmlRoot(ElementName = "clinic")]
    public class Clinic
    {
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "phone")]
        public string Phone { get; set; }
        [XmlElement(ElementName = "city")]
        public string City { get; set; }
    }

    [XmlRoot(ElementName = "clinics")]
    public class Clinics
    {
        [XmlElement(ElementName = "clinic")]
        public List<Clinic> Clinic { get; set; }
    }

}
