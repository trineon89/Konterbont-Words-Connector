using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KonterbontLODConnector
{
    public class INDesignPlugin
    {
        public string scriptname = "01_create_buttons.jsx";
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
            string _filecontent = Properties.Resources._01_prepare_doc;

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
                File.WriteAllText(adbscriptfolder + "KB4\\" + scriptname, _filecontent, new UTF8Encoding(true));
            }
        }

        public bool UpToDate()
        {
            return true;
        }
    }
}
