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
}
