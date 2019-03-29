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
        public string CallNumber { get; set; }
    }
}
