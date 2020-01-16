﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector.classes
{
    class Exporter
    {
        private string magazinePath = @"\\192.168.7.79\Konterbont_Produktioun\Magazines\";
        private string customAudioPath = @"\\192.168.7.79\Konterbont_Audio\";
        public List<ExporterClassObject> wordlist = new List<ExporterClassObject>();
        public string globrgb = "";

        public async Task<bool> Init()
        {

            return true;
        }

        public void Finish()
        {

        }

        public void PrepareMp3(string mp3file)
        {
            string temppath = Path.GetTempPath() + @"_KBLODCONN\" + @"WebResources\popupbase-web-resources\audio\";
            string source = customAudioPath + mp3file;
            string dest = temppath + mp3file;
            BackgroundWorker.AddWork(source, dest);
        }

        public void DoExport(classes.WordOverview _wo)
        {
            ExporterClassObject exporterClassObject = new ExporterClassObject();
            classes.WordBase _wb = new WordBase();
            int wordPointer = _wo.WordPointer - 1;
            int meaningPointer = _wo._wordPossibleMeanings[wordPointer].meaningPointer - 1;
            string wordBasePointer = _wo._wordPossibleMeanings[wordPointer].wordBasePointer;
            frmMainProgram.getInstance()._articleFile.article._WordBase.TryGetValue(wordBasePointer, out _wb);
            if (_wb != null)
            {
                //Lu
                
                if (_wb.meanings[meaningPointer].LU != null)
                {
                    exporterClassObject.LU = _wb.meanings[meaningPointer].LU;
                } else
                {
                    exporterClassObject.LU = _wb.baseWordLu;
                }

                exporterClassObject.Occurence = _wo._wordPossibleMeanings[wordPointer].occurence;
                exporterClassObject.DE = _wb.meanings[meaningPointer].DE;
                exporterClassObject.FR = _wb.meanings[meaningPointer].FR;
                exporterClassObject.EN = _wb.meanings[meaningPointer].EN;
                exporterClassObject.PT = _wb.meanings[meaningPointer].PT;
                exporterClassObject.WuertForm = _wb.wordForm.WordFormStringLu;

                switch (exporterClassObject.WuertForm)
                {
                    case "Verb":
                    case "Hëllefsverb":
                    case "Modalverb": exporterClassObject.LUs = _wb.wordForm.pastParticiple;
                        exporterClassObject.LUpretend = _wb.wordForm.WordFormHelperVerb;
                        break;
                    case "Adjektiv":
                    case "Adverb":
                    case "Partikel": exporterClassObject.LUs = null;
                        break;
                    default: //Default with plural
                        if (_wb.wordForm.WordFormPlurals != null && _wb.meanings[meaningPointer].NoPlural == true)
                        {
                            exporterClassObject.LUs = "(kee Pluriel)";
                        } else
                        {
                            if (_wb.meanings[meaningPointer].LUs != null)
                            {
                                exporterClassObject.LUs = _wb.meanings[meaningPointer].LUs;
                            } else
                            {
                                string plural = "";
                                if (_wb.wordForm.WordFormPlurals == null)
                                {
                                    exporterClassObject.LUs = "(kee Pluriel)";
                                } else
                                {
                                    foreach (string wfp in _wb.wordForm.WordFormPlurals)
                                    {
                                        plural += wfp;
                                    }
                                    exporterClassObject.LUs = plural;
                                }
                            }

                        }
                        break;
                }

                exporterClassObject.MP3 = _wb.baseMp3;

                if (_wo._wordPossibleMeanings[wordPointer].customMeaning != null)
                {
                     if (_wo._wordPossibleMeanings[wordPointer].customMeaning.MP3 != null)
                        exporterClassObject.MP3 = _wo._wordPossibleMeanings[wordPointer].customMeaning.MP3;
                    /*
                    *  TODO
                    */
                }

                PrepareMp3(exporterClassObject.MP3);

                wordlist.Add(exporterClassObject);
            } else
            {
                throw new Exception("Wuert konnt net fonnt ginn: " + wordBasePointer + "\n" + _wo.ToString());
            }
            
        }
    }

    class ExporterClassObject
    {
        public string Occurence = "";
        public string WuertForm = "";
        public string LU = "";
        public string LUs = "";
        public string LUpretend; //Hellefsverb zb.
        public string eiffler;
        public string DE = "";
        public string FR = "";
        public string EN = "";
        public string PT = "";
        public string MP3 = "";
    }
}
