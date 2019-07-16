﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector
{
    public class LogClass
    {
        public string FileName = "";
        StreamWriter Log = null;

        public void CreateLog(string ArtPath, string Art)
        {
            FileName = ArtPath +"\\"+ Art + ".log";
            if (File.Exists(FileName))
            {
                Log = File.AppendText(FileName);
            }
            else
            {
                Log = File.CreateText(FileName);
            }
            Log.WriteLine("[" + DateTime.Now.ToString() + "] Begin Log");
        }

        public void CloseLog()
        {
            Log.WriteLine("[" + DateTime.Now.ToString() + "] Close Log");
            Log.Close();
        }

        public void WriteToLog(string CustomText)
        {
            if (!CustomText.Contains("_ART_"))
            {
                Log.WriteLine("[" + DateTime.Now.ToString() + "] " + CustomText);
            }
        }
    }
}
