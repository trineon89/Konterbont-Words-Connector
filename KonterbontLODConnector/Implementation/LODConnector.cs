using KonterbontLODConnector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector.Implementation
{
    class LODConnector : Interfaces.ILODConnectors
    {
        public classes.WordOverview searchWord(string occ)
        {
            classes.WordOverview wo = new classes.WordOverview();
            wo.valid = true;
            return wo;
        }

    }

    public class LodApiResults
    {
        public string searchString;

        public List<string> Results;


        public LodApiResults()
        {
            searchString = null;
            Results = new List<string>();
        }

        public LodApiResults(string strangeLodJson)
        {
            Results = new List<string>();

            if (strangeLodJson == null) return;
            strangeLodJson = DecodeEncodedNonAsciiCharacters(strangeLodJson);
            strangeLodJson = strangeLodJson.Replace(",", "");
            strangeLodJson = strangeLodJson.Replace("[", "");
            strangeLodJson = strangeLodJson.Replace("]", "");
            string[] splitted = @strangeLodJson.Split('"');
            List<string> clearedSplitted = new List<string>(); ;
            for (int i = 0; i < splitted.Length -1; i++)
            {
                if (splitted[i] != "") clearedSplitted.Add(splitted[i]);
            }

            searchString = clearedSplitted[0];
            if (clearedSplitted.Count>1)
            {
                //has results
                for (int i = 1; i< clearedSplitted.Count -1; i++)
                {
                    Results.Add(clearedSplitted[i]);
                }
            } 

        }


        //  https://stackoverflow.com/questions/1615559/convert-a-unicode-string-to-an-escaped-ascii-string/12563498
        static string DecodeEncodedNonAsciiCharacters(string value)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse(m.Groups["Value"].Value, System.Globalization.NumberStyles.HexNumber)).ToString();
                });
        }
    }
}
