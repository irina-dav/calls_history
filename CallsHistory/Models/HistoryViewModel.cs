using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CallsHistory.Models
{
    public class HistoryViewModel
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string SrcCallNumber { get; set; }
        public string DstCallNumber { get; set; }

        public SelectList TimeZonesOffsetUTC { get; set; }
        public int OffsetUTC { get; set; }

        public bool GroupBy { get; set; }
    }
}
