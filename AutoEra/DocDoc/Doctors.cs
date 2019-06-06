using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Data;


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

        public Doctor() {}

         public Doctor (DataRow row, Clinic clinic)
        {
            if (row["ind_code"] != null)
            {
                if (row["FIRST_LAST_NAME"] != null)
                {
                    DoctorId = row["ind_code"].ToString();
                   
                    Name = row["FIRST_LAST_NAME"].ToString();
                    ClinicId = clinic.Id;
                    
                }
            }
        
        }


    }

    [XmlRoot(ElementName = "doctors")]
    public class Doctors
    {
        [XmlElement(ElementName = "doctor")]
        public List<Doctor> Doctor { get; set; }
    }

}
