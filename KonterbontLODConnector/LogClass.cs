using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector
{
    public class LogClass
    {
        public string FileName = @"";
        public string ArticlePath = "";

        public void CreateLog(string ArticlePath, string Article)
        {
            FileName = ArticlePath + Article + ".log";
            StreamWriter Log = null;
            if (File.Exists(FileName))
            {
                Log = File.AppendText(FileName);
            }    
            else
            {
                Log = File.CreateText(FileName);
            }
           
        }

        public void CloseLog()
        {

        }


        public void WriteToLog(string LogEvent, string Word = "",string SelWord = "", string Meaning = "")
        {
            switch (LogEvent)
            {

                case "new":
                    {
                        break;
                    }
                case "add":
                    {
                        Console.Write("test");
                        break;
                    }

                case "changeMeaning":
                    {
                        break;
                    }
                case "changeWord":
                    {
                        break;
                    }
                case "createPopup":
                    {
                        break;
                    }

                case "addToMag":
                    {
                        break;
                    }
            }

        }
    }
}
