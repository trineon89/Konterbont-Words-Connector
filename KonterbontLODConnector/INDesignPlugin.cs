using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KonterbontLODConnector
{
    public class INDesignPlugin
    {
        public string scriptname =  "01_create_buttons.jsx";
        public string scriptname2 = "02_Export_Articles_From_Book.jsx";
        public string scriptname3 = "02_Export_Book.jsx";
        public string scriptname4 = "zz_twixlForServer.jsx";
        public string scriptname5 = "02_export_article.jsx";
        public string AppDataPath;
        public List<string> AdobeScriptsSubFolders = new List<string>();

        public INDesignPlugin()
        {
            AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            foreach (string dir in Directory.GetDirectories(AppDataPath+"\\Adobe\\InDesign", "Version*"))
            {
                foreach (string innerdir in Directory.GetDirectories(dir))
                {
                    string tmpdir = innerdir + "\\Scripts\\Scripts Panel\\";
                    AdobeScriptsSubFolders.Add(tmpdir);
                }
            }
            PushScriptFile();
        }

        public void PushScriptFile()
        {
            //string _filecontent = Properties.Resources._01_prepare_doc;
            //string _filecontent2 = Properties.Resources._02_Export_Articles_From_Book;
            //string _filecontent3 = Properties.Resources._02_Export_Book;
            //string _filecontent4 = Properties.Resources.zz_twixlForServer;
            //string _filecontent5 = Properties.Resources._02_export_article;

            foreach (string adbscriptfolder in AdobeScriptsSubFolders)
            {
                if (Directory.Exists(adbscriptfolder + "KB4"))
                {
                    try
                    {
                        Directory.Delete(adbscriptfolder + "KB4", true);
                    }
                    catch (Exception ea)
                    {
                        Console.WriteLine("{0} Exception caught.", ea);
                    }
                }
                Directory.CreateDirectory(adbscriptfolder + "KB4");
                //File.WriteAllText(adbscriptfolder + "KB4\\" + scriptname, _filecontent, new UTF8Encoding(true));
                //File.WriteAllText(adbscriptfolder + "KB4\\" + scriptname2, _filecontent2, new UTF8Encoding(true));
               // File.WriteAllText(adbscriptfolder + "KB4\\" + scriptname3, _filecontent3, new UTF8Encoding(true));
                //File.WriteAllText(adbscriptfolder + "KB4\\" + scriptname4, _filecontent4, new UTF8Encoding(true));
                //File.WriteAllText(adbscriptfolder + "KB4\\" + scriptname5, _filecontent5, new UTF8Encoding(true));
            }
        }

        public bool UpToDate()
        {
            return true;
        }
    }
}
