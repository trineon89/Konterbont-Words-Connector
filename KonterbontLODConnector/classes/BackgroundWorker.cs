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
        private static int _counter = 0;
        private static Dictionary<string, string> _counterDic = new Dictionary<string, string>();
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
           // WorkerLogger.WriteLog("transfered " + _counter.ToString() + " files"); ;
            string res = "transfered: ";
            foreach (var ic in _counterDic)
            {
                res += Environment.NewLine + "Type: " + ic.Key + " # " + ic.Value;
            }
            WorkerLogger.WriteLog(res);
            WorkerLogger.CloseLog();
            _isWorking = false;
            _counterDic.Clear();
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="copy">if false move instead of copy</param>
       /// <returns></returns>
        public static async Task DoWork(bool copy = true)
        {
            await internalDoWork(copy);
        }

        private static async Task internalDoWork(bool copy = true)
        {
            while (hasWork())
            {
                WorkingClassElement wo = workingClassElements.First();
                if (copy)
                {
                    await wo.CopyFile();
                } else
                {
                    await wo.MoveFile();
                }
                
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
            
            if (_counterDic.ContainsKey(Path.GetExtension(sourcePath)) )
            {
                _counterDic[Path.GetExtension(sourcePath)] = (Int32.Parse(_counterDic[Path.GetExtension(sourcePath)]) + 1).ToString();
            } else
            {
                _counterDic.Add(Path.GetExtension(sourcePath), "1");
            }
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



        public async Task CopyFile()
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
                    using (var response = await request.GetResponseAsync())
                    {
                        if ((response as HttpWebResponse).StatusCode == HttpStatusCode.OK)
                        {
                            using (var responseStream = response.GetResponseStream())
                            {
                                using (var fileToDownload = new System.IO.FileStream(sourcePath, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
                                {
                                    await responseStream.CopyToAsync(fileToDownload);
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
                        await source.CopyToAsync(destination).ConfigureAwait(false);
                        WorkerLogger.WriteLog("copied " + sourcePath + " to " + destinationPath);
                    }
                }
            } else
            {
                //file not found on Server
                WorkerLogger.WriteLog("[ERROR] file not found on " + sourcePath);
            }
        }

        public async Task MoveFile()
        {
            if (File.Exists(sourcePath))
            {
                using (Stream source = File.Open(sourcePath, FileMode.Open))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                    using (Stream destination = File.Create(destinationPath))
                    {
                        await source.CopyToAsync(destination).ConfigureAwait(false);
                        WorkerLogger.WriteLog("moved " + sourcePath + " to " + destinationPath);
                    }
                }
                File.Delete(sourcePath);
            } else
            {
                WorkerLogger.WriteLog("[ERROR] file not found on " + sourcePath);
            }
        }
    }



    static class WorkerLogger
    {
        static string FileName = "";
        static StreamWriter Log = null;
        static string _internalLog;

        private static void CreateLog()
        {
            FileName = frmMainProgram.getInstance()._articleFile.ArticlePath + @"\"
                        + frmMainProgram.getInstance()._articleFile.ArticleId + ".log";
            if (File.Exists(FileName))
                Log = File.AppendText(FileName);
            else
                Log = File.CreateText(FileName);

            Log.AutoFlush = true;
            //Log.WriteLine("[" + DateTime.Now.ToString() + "] Begin Log");
            _internalLog = "";
        }

        public static void WriteLog(string Text, bool _internal = true)
        {
            if (Log == null)
                CreateLog();

            if (_internal)
            {
                if (_internalLog != "") { _internalLog += Environment.NewLine; }
                _internalLog += "[" + DateTime.Now.ToString() + "] " + Text;
            } else
            {
                Log.WriteLine("[" + DateTime.Now.ToString() + "] " + Text);
            }
        }

        public static void CloseLog()
        {
            if (Log != null)
            {
                //Log.WriteLine("[" + DateTime.Now.ToString() + "] Close Log");
                Log.Write(_internalLog + Environment.NewLine);
                Log.Flush();
                Log.Close();
                Log = null;
            }
        }
    }
}
