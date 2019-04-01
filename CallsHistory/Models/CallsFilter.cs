using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CallsHistory.Models
{
    public class CallsFilter
    {
        public DateTime CallDateFrom { get; set; }
        public DateTime CallDateTo { get; set; }
        public string SrcCallNumber { get; set; }       
        public string DstCallNumber { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Sort { get; set; }
        public string Order { get; set; }
    }
}
