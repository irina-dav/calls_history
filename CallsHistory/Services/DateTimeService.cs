using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CallsHistory.Services
{
    public class DateTimeService
    {
        public Dictionary<string, int> TimeZonesOffsetUTC;
        private Dictionary<string, int> serversOffsetsUTC;

        public DateTimeService(Dictionary<string, int> serversOffsetsUTC, Dictionary<string, int> timeZonesOffsetUTC)
        {
            this.serversOffsetsUTC = serversOffsetsUTC;
            TimeZonesOffsetUTC = timeZonesOffsetUTC;
        }

        public int GetServerOffsetUTC(string serverName)
        {
            return serversOffsetsUTC.GetValueOrDefault(serverName, 0);
        }
    }
}
