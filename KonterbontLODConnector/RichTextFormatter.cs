using KonterbontLODConnector.classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using wpf = System.Windows.Controls;

namespace KonterbontLODConnector
{
    static class RichTextFormatter
    {
        private static Article article;
        private static Dictionary<string, Word> clickItemDic = new Dictionary<string, Word>();

        public static string activeWord;

        public static wpf.RichTextBox richText;
        public static ElementHost elementHost;

        public static Article Article { get => article; set => article = value; }

        public static void LoadArticle(string ArticlePath)
        {
            // _richText_WinformHost.LoadDocument(ArticlePath);

            TextRange textRange;
            System.IO.FileStream fileStream;
            if (System.IO.File.Exists(ArticlePath))
            {
                textRange = new TextRange(richText.Document.ContentStart, richText.Document.ContentEnd);
                string oldcontent = textRange.Text;
                Console.WriteLine(oldcontent);
                using (fileStream = new System.IO.FileStream(ArticlePath, System.IO.FileMode.Open))
                {
                    textRange.Load(fileStream, System.Windows.DataFormats.Rtf);
                }
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

        public static void ReDecorate()
        {
            /*
             *  Document : Blocks (aka Paragraph) : Inlines
             * 
             */

            foreach (Block block in richText.Document.Blocks)
            {
                //string textElement = new TextRange(block.ContentStart, block.ContentEnd).Text;
                
                if (block is Paragraph)
                {
                    Paragraph p = block as Paragraph;
                    foreach (Inline inline in p.Inlines)
                    {
                        string inlineTextElement = new TextRange(inline.ContentStart, inline.ContentEnd).Text;
                        if (inline is Span)
                        {
                            if (inline.Background != null)
                            {
                                //should be word
                                Console.WriteLine(inlineTextElement);
                                addWordToList(inlineTextElement);
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

        public static void addWordToList(string wuert)
        {
            if (clickItemDic.ContainsKey(wuert)) { MessageBox.Show("D'wuert "+wuert+" existéiert schonn am Artikel!"); }
            clickItemDic.Add(wuert, new Word(wuert));
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
