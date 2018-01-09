using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data;
namespace PersonalDuty
{
    class PersonalDuty
    {

        private Collection<Duty> duty { get; set; }
      
    }

    class Duty
    {
        DataTable name { get; set; }
        DataTable Date { get; set; }

        public void AddDuty (DataTable dutyDT , DataTable personal)
        {
            foreach (DataRow data in dutyDT.Rows)
            {

            }

        }
        
    }
    class Date
    {
        DateTime st_time { get; set; }
        DateTime end_time { get; set; }
    }
}
