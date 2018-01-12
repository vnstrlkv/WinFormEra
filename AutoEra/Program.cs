using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using PersonalDuty;
using System.IO;
using WDBFNS;
using AutoEra;
namespace AutoEra
{
    class Program
    {

       static ForConsole test = new ForConsole();


        static void Main(string[] args)
        {
            string path = @"D:\PHOENIX\dbc";
            test.Start();
            test.InitTable();
            test.InsertDoc();
            test.ToFTP();
            MonitorDirectory(path);

           
            
            Console.ReadKey();
        }
        private static void MonitorDirectory(string path)

        {

            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();

            fileSystemWatcher.Path = path;
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileSystemWatcher.Filter = "duty.FPT";
            fileSystemWatcher.Changed+= FileSystemWatcher_Changed;
            fileSystemWatcher.EnableRaisingEvents = true;

        }
        private static void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)

        {

            ChangeDB();
            
        }

      static  void ChangeDB()
        {

            test.InitTable();
            test.InsertDoc();
            test.ToFTP();
        }



    }
}
