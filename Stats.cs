using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pat
{
    public class Stats 
    {
        public string xtotalRows;
        public string xcolumnCount;
        public string xrecsWMatches;
        public string xrecsWExactMatches;
        
        public Stats(Matches p_mcol)
        {
            this.xtotalRows = p_mcol.stats.totalRows;
            this.xcolumnCount = p_mcol.stats.columnCount;
            this.xrecsWMatches = p_mcol.stats.recsWMatches;
            this.xrecsWExactMatches = p_mcol.stats.recsWExactMatches;
        }   
    }
}
