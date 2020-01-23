using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector.classes
{
    class INDesignPlugin
    {
        public string scriptname = "01_create_buttons.jsx";
        public string AppDataPath;
        public List<string> AdobeScriptsSubFolders = new List<string>();

        public INDesignPlugin()
        {
            AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            foreach (string dir in Directory.GetDirectories(AppDataPath + "\\Adobe\\InDesign", "Version*"))
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
            foreach (string adbscriptfolder in AdobeScriptsSubFolders)
            {
                if (Directory.Exists(adbscriptfolder + "KB5"))
                {
                    try
                    {
                        Directory.Delete(adbscriptfolder + "KB5", true);
                    }
                    catch (Exception ea)
                    {
                        Console.WriteLine("{0} Exception caught.", ea);
                    }
                }
                Directory.CreateDirectory(adbscriptfolder + "KB5");
                File.WriteAllText(adbscriptfolder + @"KB5\" + scriptname, Properties.Resources.create_popups_script, new UTF8Encoding(true));
            }
        }
    }
}
