using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallsHistory.Models
{
    public class CallViewModel
    {
        static private Dictionary<string, string> desctinations = new Dictionary<string, string>
        {
            {"ANSWERED", "Отвечен" } ,
            {"NO ANSWER", "Не отвечен" },
            {"BUSY", "Линия занята"}
        };

        public string Src { get; set; }

        public string SrcName { get; set; }

        public string Dst { get; set; }

        public string DstName { get; set; }

        public string CallDate { get; set; }

        public string Duration { get; set; }
        
        public string Disposition { get; set; }

        public CallViewModel(Call call)
        {
            Src = call.Src;
            Dst = call.Dst;
            CallDate = call.CallDateUTC.ToString("dd.MM.yy HH:mm:ss");
            Duration = TimeSpan.FromSeconds(call.Duration).ToString();
            Disposition = desctinations.GetValueOrDefault(call.Disposition, call.Disposition);   
        }
    }
}