using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace AutoEra.DocDoc
{
    [XmlRoot(ElementName = "doctor")]
    public class Doctor
    {
        [XmlElement(ElementName = "doctorId")]
        public string DoctorId { get; set; }
        [XmlElement(ElementName = "clinicId")]
        public string ClinicId { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "doctors")]
    public class Doctors
    {
        [XmlElement(ElementName = "doctor")]
        public List<Doctor> Doctor { get; set; }
    }

}
