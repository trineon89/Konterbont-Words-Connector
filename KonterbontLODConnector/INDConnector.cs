//using InDesign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector
{
    static class INDConnector
    {
        static Type inDesignAppType = Type.GetTypeFromProgID("InDesign.Application");
        //static InDesign.Application myInDesign;
        //static InDesign.Book book;

        static INDConnector()
        {
            //myInDesign = (InDesign.Application)Activator.CreateInstance(inDesignAppType);
        }

        public static Boolean getBook()
        {
            /*if (myInDesign.ActiveBook == null) { return false; }
            else {
                book = myInDesign.ActiveBook;
                return true;
            }*/
            return false;
        }

        public static List<String> getBookContent()
        {
            //if (book == null) { getBook(); }
            List<String> strings = new List<String>();
            /*foreach (BookContent bc in myInDesign.ActiveBook.BookContents )
            {
                Console.WriteLine(bc.Name);
                strings.Add(bc.Name);
            }*/
            return strings;
        }

        public static string getPathOfArticleInBook(String Article)
        {
            String path = null;
           /* foreach (BookContent bc in myInDesign.ActiveBook.BookContents)
            {
                if (bc.Name == Article)
                {
                    path = bc.FilePath;
                }
            }*/
            return path;
        }
    }
}
