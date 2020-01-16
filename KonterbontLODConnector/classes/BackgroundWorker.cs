using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector.classes
{
    public static class BackgroundWorker
    {
        private static bool _isWorking = false;
        static List<WorkingClassElement> workingClassElements = new List<WorkingClassElement>();

        /// <summary>
        /// Check if Worker has a Queue
        /// </summary>
        /// <returns>true if has a queue, false if not</returns>
        public static bool hasWork()
        {
            if (workingClassElements.Count == 0)
            {
                return false;
            } else
            {
                return true;
            }
        }

        private static bool isWorking()
        {
            if (_isWorking)
                return true;
            else return false;
        }

        private static void finishWork()
        {
            WorkerLogger.CloseLog();
            _isWorking = false;
        }

        /// <summary>
        /// Start Worker
        /// </summary>
        public static async Task DoWork()
        {
            internalDoWork();
        }

        private static void internalDoWork()
        {
            while (hasWork())
            {
                WorkingClassElement wo = workingClassElements.First();
                wo.CopyFile();
                workingClassElements.Remove(wo);
            }
            finishWork();
        }

        /// <summary>
        /// Add Work to Worker
        /// </summary>
        /// <param name="sourcePath">The Source of the File to be copied</param>
        /// <param name="destinationPath">The Destination where the File should be copied over to</param>
        public static void AddWork(string sourcePath, string destinationPath)
        {
            WorkingClassElement wo = new WorkingClassElement();
            wo.sourcePath = sourcePath;
            wo.destinationPath = destinationPath;
            workingClassElements.Add(wo);
        }
    }

    class WorkingClassElement
    {
        internal string sourcePath;
        internal string destinationPath;
        private string lodmp3path = "https://www.lod.lu/audio/";

        public WorkingClassElement()
        {
            sourcePath = null;
            destinationPath = null;
        }

        public void CopyFile()
        {
            if (!File.Exists(sourcePath))
            {
                //download from LOD
                using (WebClient wc = new WebClient())
                {
                    string filename = Path.GetFileName(destinationPath);
                    //wc.DownloadFile(new Uri(lodmp3path + filename), destinationPath);

                    var request = (HttpWebRequest)WebRequest.Create(lodmp3path + filename);
                    request.Method = "GET";
                    using (var response = request.GetResponse())
                    {
                        if ((response as HttpWebResponse).StatusCode == HttpStatusCode.OK)
                        {
                            using (var responseStream = response.GetResponseStream())
                            {
                                using (var fileToDownload = new System.IO.FileStream(sourcePath, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
                                {
                                    responseStream.CopyTo(fileToDownload);
                                    WorkerLogger.WriteLog("downloaded from " + lodmp3path + filename + " to " + sourcePath);
                                }
                            }
                        }
                        else
                        {
                            //file not found on WebServer
                            WorkerLogger.WriteLog("file not found on " + lodmp3path + filename);
                        }

                    }
                }
            }

            if (File.Exists(sourcePath))
            {
                using (Stream source = File.Open(sourcePath, FileMode.Open))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                    using (Stream destination = File.Create(destinationPath))
                    {
                        source.CopyTo(destination);
                        WorkerLogger.WriteLog("copied " + sourcePath + " to " + destinationPath);
                    }
                }
            } else
            {
                //file not found on Server
                WorkerLogger.WriteLog("file not found on " + sourcePath);
            }
        }
    }



    static class WorkerLogger
    {
        static string FileName = "";
        static StreamWriter Log = null;

        private static void CreateLog()
        {
            FileName = frmMainProgram.getInstance()._articleFile.ArticlePath + @"\"
                        + frmMainProgram.getInstance()._articleFile.ArticleId + ".log";
            if (File.Exists(FileName))
                Log = File.AppendText(FileName);
            else
                Log = File.CreateText(FileName);

            Log.AutoFlush = true;
            Log.WriteLine("[" + DateTime.Now.ToString() + "] Begin Log");
        }

        public static void WriteLog(string Text)
        {
            if (Log == null)
                CreateLog();

            Log.WriteLine("[" + DateTime.Now.ToString() + "] " + Text);
        }

        public static void CloseLog()
        {
            if (Log != null)
            {
                Log.WriteLine("[" + DateTime.Now.ToString() + "] Close Log");
                Log.Flush();
                Log.Close();
                Log = null;
            }
        }
    }
}
