using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using WDBFNS;
using System.Data;
using System.IO;

namespace AutoEra.Bitrix
{

    class ToBitrix
    {
        public WDBF WDBF = new WDBF();
        Bitrix_Clients Allclients = new Bitrix_Clients();
        DateTime today = DateTime.Today;

        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(fileContents);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }



        public void Start()
        {
            Bitrix_Clients TMPclients = new Bitrix_Clients();
            var DT = WDBF.DBSelect("FIRST_NAME, FAMILY, LAST_NAME, mobile_ph_number , ADDDATE", "clndates", " WHERE ADDDATE = {" + DateTime.Today.ToString("MM.dd.yyyy") + "}");
            /* выгрузка всех клиентов
            var DT = WDBF.DBSelect("FIRST_NAME, FAMILY, LAST_NAME, mobile_ph_number , ADDDATE", "clndates", " WHERE ADDDATE < {" + DateTime.Today.ToString("MM.dd.yyyy") + "}");
            */
            
            foreach (DataRow dr in DT.Rows)
            {
                TMPclients.bitrix_Clients.Add(new Bitrix_Client(dr["FIRST_NAME"].ToString().Trim(' '), dr["FAMILY"].ToString().Trim(' '), dr["LAST_NAME"].ToString().Trim(' '), dr["mobile_ph_number"].ToString().Trim(' '), dr["ADDDATE"].ToString().Trim(' ')));
            }
            /*
             * Выгрузка всех контактов в csv
            using (var w = new StreamWriter("tmp11.csv", false, Encoding.UTF8))
            {
                //  w.WriteLine("-1;2017-11-10 00:00:00.0000;16;50;17;10");
                // w.WriteLine("DATE;ST_HOUR;_ST_MIN;END_HOUR;END_MIN");
                //  w.Flush();
                w.WriteLine("FirstName;SecondName;LastName;Phonenumber");
                w.Flush();


                foreach (Bitrix_Client client in TMPclients.bitrix_Clients)
                {
                    string outputstr = null;                   
                    w.WriteLine("{0};{1};{2};{3}", client.First_name, client.Second_name, client.Last_name, client.Phone.VALUE);
                    w.Flush();

                }
            }
            */
           

            if (Allclients.bitrix_Clients.Count==0)
            {

                if (File.Exists("clients.json"))
                  Allclients = ReadFromJsonFile<Bitrix_Clients>("clients.json");
                else 
                {
                    Allclients.bitrix_Clients = Allclients.bitrix_Clients.Concat(TMPclients.bitrix_Clients).ToList();
                }
               
            }
            if (Allclients.bitrix_Clients.Count < TMPclients.bitrix_Clients.Count)
            {
                List<Bitrix_Client> k =new List<Bitrix_Client>();
                int i = 0;
                foreach (var itemTMP in TMPclients.bitrix_Clients)
                {
                    foreach (var itemAll in Allclients.bitrix_Clients)
                    {
                      //  int n = 0;
                        if (itemTMP.Phone.VALUE == itemAll.Phone.VALUE)
                        {
                            k.Add(itemTMP);
                            break;
                        }
                       // n++;
                    }
                        i++;                    
                }
                foreach (var item in k)
                    TMPclients.bitrix_Clients.Remove(item);

              
                  Allclients.bitrix_Clients=Allclients.bitrix_Clients.Concat(TMPclients.bitrix_Clients).ToList();
            }
            
            AddClientToBitrix(Allclients).Wait();
             
        }



        public static object ClientToBitrixFormat(Bitrix_Client client)
        {
            return new
            {
                fields = new
                {
                    NAME = client.First_name,
                    SECOND_NAME = client.Second_name,
                    LAST_NAME = client.Last_name,
                    OPENED = "Y",
                    ASSIGNED_BY_ID = 1,
                    TYPE_ID = "CLIENT",
                    SOURCE_ID = "SELF",
                    PHONE = new List<Bitrix.Phone>() { new Bitrix.Phone() { VALUE_TYPE = "WORK", TYPE_ID = "PHONE", VALUE = client.Phone.VALUE} }.ToArray(),
                },
                @params = new
                {
                    REGISTER_SONET_EVENT = "Y"
                }
            };
        }




        private static async Task AddClientToBitrix(Bitrix_Clients clients)
        {

            INIManager BTX = new INIManager("c:\\config.ini");
            string BitrixName = BTX.GetPrivateString("BTX", "logPD");
            string BitrixPass = BTX.GetPrivateString("BTX", "pasPD");

         
            string url = "https://" + BitrixName + "/rest/1/" + BitrixPass + "/crm.contact.add.json";
            // Serialize our concrete class into a JSON String

            foreach (Bitrix_Client client in clients.bitrix_Clients)
            {
                if (!client.Status)
                { 
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(ClientToBitrixFormat(client)));

                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {

                    // Do the actual request and await the response
                    var httpResponse = await httpClient.PostAsync(url, httpContent);

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();
                        Console.WriteLine(responseContent);
                        if (responseContent.Contains("\"result\":"))
                        client.Status = true;
                        // From here on you could deserialize the ResponseContent back again to a concrete C# type using Json.Net
                    }

                }
                }
               
                
            }

            List<Bitrix_Client> k = new List<Bitrix_Client>();
            int i = 0;
           foreach (Bitrix_Client cl in clients.bitrix_Clients)
            {
                if (clients.bitrix_Clients==null || clients.bitrix_Clients[i].Add_date != DateTime.Today)
                {
                    k.Add(cl);
                }
                i++;
            }
           foreach(var n in k)
            clients.bitrix_Clients.Remove(n);

            WriteToJsonFile<Bitrix_Clients>("clients.json", clients);

        }


    }
}
