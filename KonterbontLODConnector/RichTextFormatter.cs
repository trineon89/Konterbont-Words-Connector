﻿using KonterbontLODConnector.classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using wpf = System.Windows.Controls;

namespace KonterbontLODConnector
{
    static class RichTextFormatter
    {
        private static Article article;
        private static Dictionary<string, Word> clickItemDic = new Dictionary<string, Word>();

        private static Brush brush_notSelected = Brushes.LightSlateGray;
        private static Brush brush_notVerified = new SolidColorBrush(Color.FromArgb(255, 102, 217, 255));
        private static Brush brush_withError = Brushes.OrangeRed;
        private static Brush brush_Verified = Brushes.LimeGreen;

        public static string activeWord;

        public static wpf.RichTextBox richText;
        public static ElementHost elementHost;

        public static Article Article { get => article; set => article = value; }

        public static void LoadArticle(ArticleFile articleFile)
        {
            // _richText_WinformHost.LoadDocument(ArticlePath);

            TextRange textRange;
            System.IO.FileStream fileStream;
            if (System.IO.File.Exists(articleFile.article.RtfPath))
            {
                   
            } else
            {
                //maybe the Directory changed?
                string newpath = frmMainProgram.getSettings().ArticlePath + @"\" + articleFile.ArticleId + @"_" + articleFile.ArticleName + @"\" + articleFile.ArticleId + @"_Text\Text.rtf";
                if (File.Exists(newpath))
                {
                    articleFile.article.RtfPath = newpath;
                } else
                {
                    //not found, exit
                    MessageBox.Show("Den Artikel existéiert net", "Article not found!");
                    return;
                }
                // article.RtfPath

                
            }

            textRange = new TextRange(richText.Document.ContentStart, richText.Document.ContentEnd);
            string oldcontent = textRange.Text;
            Console.WriteLine(oldcontent);
            using (fileStream = new System.IO.FileStream(articleFile.article.RtfPath, System.IO.FileMode.Open))
            {
                textRange.Load(fileStream, System.Windows.DataFormats.Rtf);
            }
        }

        public static void LoadRtfHandler(wpf.RichTextBox obj)
        {
            richText = obj;
        }

        public static string getClickedWord()
        {
            if (clickItemDic.Count == 0)
            {
                activeWord = null;
                return null;
            }

            
            TextPointer tp1 = richText.Selection.Start;
            if (tp1.Parent is Run)
            {
                var a = (tp1.Parent as Run).Parent;
                if (a is Span)
                {
                    Span b = a as Span;
                    if (b.Background != null)
                    {
                        string inlineTextElement = new TextRange(b.ContentStart, b.ContentEnd).Text;
                        activeWord = inlineTextElement;
                    }
                    else
                    {
                        //maybe next Element?
                        Span c = (b.NextInline as Span);
                        if (b.NextInline == null)
                        {
                            activeWord = null;
                            return null;
                        }
                        // check distance (5 if it has d')
                        int distance = tp1.GetOffsetToPosition(c.ContentStart);
                        if (distance>5)
                        {
                            activeWord = null;
                            return null;
                        }
                        
                        if (c.Background != null)
                        {
                            string inlineTextElement = new TextRange(c.ContentStart, c.ContentEnd).Text;
                            activeWord = inlineTextElement;
                        }
                    }
                }
                else
                {
                    activeWord = null;
                }

            }
            else
            {
                activeWord = null;
            }
            

            //richText.Selection.Start
            
            if (clickItemDic.TryGetValue(activeWord, out _))
            {
                return activeWord;
            } else
            {
                return null;
            }
        }

        public static void ResetWordList()
        {
            clickItemDic.Clear();
        }

        public static void Decorate()
        {
            foreach (Block block in richText.Document.Blocks)
            {
                if (block is Paragraph)
                {
                    Paragraph p = block as Paragraph;
                    foreach (Inline inline in p.Inlines)
                    {
                        TextRange tx = new TextRange(inline.ContentStart, inline.ContentEnd);
                        string inlineTextElement = tx.Text;
                        if (inline is Span)
                        {
                            if (inline.Background != null)
                            {
                                //should be word
                                Console.WriteLine(inlineTextElement);
                                addWordToList(inlineTextElement);
                                inline.Background = brush_notSelected;
                            } else
                            {
                                Console.WriteLine("#NOBG# - "+ inlineTextElement);
                            }
                        }
                        else
                        {
                            Console.WriteLine("##NOSPAN##");
                        }
                    }
                }
            }
        }

        public static void ReDecorate()
        {
            foreach (Block block in richText.Document.Blocks)
            {
                if (block is Paragraph)
                {
                    Paragraph p = block as Paragraph;
                    Random arrr = new Random();
                    foreach (Inline inline in p.Inlines)
                    {
                        TextRange tx = new TextRange(inline.ContentStart, inline.ContentEnd);
                        string inlineTextElement = tx.Text;
                        if (inline is Span)
                        {
                            if (inline.Background != null)
                            {
                                //should be word

                                //If is not set
                                Brush brush;
                                //Brush brush = Brushes.Red;

                                WordOverview wo = new WordOverview();
                                frmMainProgram.getInstance()._articleFile.article._Words.TryGetValue(inlineTextElement, out wo);
                               if (wo!= null)
                               { 
                                    switch (wo.state)
                                    {
                                        case 0: brush = brush_notSelected; break;
                                        case 1: brush = brush_notVerified; break;
                                        case 2: brush = brush_Verified; break;
                                        case 3:
                                        default: brush = brush_withError; break;
                                    }

                                    inline.Background = brush;

                               }
                            }
                            else
                            {
                                Console.WriteLine("#NOBG# - " + inlineTextElement);
                            }
                        }
                        else
                        {
                            Console.WriteLine("##NOSPAN##");
                        }
                    }
                }
            }
        }

        public static void addWordToList(string wuert)
        {
            if (clickItemDic.ContainsKey(wuert)) { MessageBox.Show("D'wuert "+wuert+" existéiert schonn am Artikel!"); }
            clickItemDic.Add(wuert, new Word(wuert));
            Word wo = new Word(wuert);
            var v = frmMainProgram.getInstance();
            //v._articleFile.article._workingWords.Add(wo);
        }
    }

    public class RichText_WinformHost : System.Windows.Forms.Integration.ElementHost
    {
        protected RichTextWPF m_richTextWPF = new RichTextWPF();

        public RichText_WinformHost()
        {
            base.Child = m_richTextWPF;
            //string text = new TextRange(m_richTextWPF.richTextBox.Document.ContentStart, m_richTextWPF.richTextBox.Document.ContentEnd).Text;
            //Console.WriteLine(text);
        }
    }
}
