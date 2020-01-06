﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonterbontLODConnector.classes
{
    public class WordOverview
    {
        public bool valid = false;
        public List<Word> _wordPossibleMeanings;
        public int WordPointer = 0;
        public int state = 0;

        public WordOverview()
        {
            _wordPossibleMeanings = new List<Word>();
        }
    }
}
