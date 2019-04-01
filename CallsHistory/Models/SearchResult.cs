using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CallsHistory.Models
{
    public class SearchResult
    {
        public int TotalCalls { get; set; }
        public IEnumerable<Call> CallsPage { get; set; }
    }
}
