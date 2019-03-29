using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallsHistory.Models
{
    public class CallViewModel
    {
        public string Src { get; set; }

        public string SrcName { get; set; }

        public string Dst { get; set; }

        public string DstName { get; set; }

        public DateTime CallDate { get; set; }

        public int Duration { get; set; }
        
        public string Disposition { get; set; }

        public CallViewModel(Call call)
        {
            Src = call.Src;
            Dst = call.Dst;
            CallDate = call.CallDate;
            Duration = call.Duration;
            Disposition = call.Disposition;   
        }
    }
}