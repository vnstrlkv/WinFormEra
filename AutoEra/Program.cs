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
using System.Threading;
namespace AutoEra
{
    class Program
    {

        static ForConsole test = new ForConsole();
        static System.Timers.Timer t;

        static void Main(string[] args)
        {
            string path = @"D:\PHOENIX\dbc";
            test.Start();
            if (test.InitTable())
            {
                test.InsertDoc();
                test.ToXML();
                test.ToFTP();
            }
            // ChangeDB();
            //  MonitorDirectory(path);


            t = new System.Timers.Timer();
            t.AutoReset = false;
            t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
            t.Interval = GetInterval();
            t.Start();
            Console.ReadLine();




            // Console.ReadKey();
        }

        static double GetInterval()
        {
            //    DateTime now = DateTime.Now;
            return 1080000;
        }

        static void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine(DateTime.Now.ToString("o"));
            ChangeDB();
            t.Interval = GetInterval();
            t.Start();
        }


        private static void MonitorDirectory(string path)

        {

            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();

            fileSystemWatcher.Path = path;
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileSystemWatcher.Filter = "duty.FPT";
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.EnableRaisingEvents = true;

        }
        private static void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)

        {

            // ChangeDB();

        }

        static void ChangeDB()
        {
            Console.Clear();
            if (test.InitTable())
            {
                test.InsertDoc();
                test.ToXML();
                test.ToFTP();
            }
            else
            {
                Console.WriteLine("База была занята, попробуем позже =)");
                using (var w = new StreamWriter("log.txt", true, Encoding.UTF8))
                {
                    w.WriteLine("{0} База была занята, попробуем позже =)", DateTime.Now);
                    w.Flush();
                }
            }
        }



    }
}
