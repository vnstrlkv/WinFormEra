using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AutoEra.Bitrix
{
    public class Bitrix_Client
    {
       private string first_name;
       private string second_name;
       private string last_name;
       private Phone phone;
       private DateTime add_date;
        private bool status;
 
        public string First_name
        {
            get { return first_name; }
            set { first_name = value; }
        }
  
        public string Second_name
        {
            get { return second_name; }
            set { second_name = value; }
        }
 
        public string Last_name
        {
            get { return last_name; }
            set { last_name = value; }
        }
      
        public Phone Phone
        {
            get { return phone; }
            set { phone = value; }
        }
        public DateTime Add_date
        {
            get { return add_date; }
            set { add_date = value; }
        }
        public bool Status
        {
            get { return status; }
            set { status = value; }
        }

     public Bitrix_Client()
        {
            Phone = new Phone();
        }
        public Bitrix_Client(string FIRST_NAME, string FAMILY, string LAST_NAME, string mobile_ph_number, string ADDDATE)
        {
            First_name = FIRST_NAME;
            Second_name = LAST_NAME;
            Last_name =  FAMILY;
            Phone = new Phone { VALUE = mobile_ph_number };
            Add_date = DateTime.Parse(ADDDATE);
            Status = false;
        }
        
        public  bool Equal(object obj)
        {
            if (Last_name == (obj as Bitrix_Client).Last_name)
                return true;
            return false;
        }
        
        

    }

    public class Phone
    {
        public string VALUE_TYPE { get; set; }
        public string TYPE_ID {get; set;} 
        public string VALUE { get; set; }

        public Phone()
        {
            VALUE_TYPE = "";
            TYPE_ID = "";
            VALUE = "";
        }
    }

    public class Bitrix_Clients
    {
        public List <Bitrix_Client> bitrix_Clients=new List<Bitrix_Client>();
    }

}
