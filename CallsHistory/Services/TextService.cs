using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallsHistory.Services
{
    public class TextService
    {
        public string ContvertToUtf8(string text)
        {
            return Encoding.UTF8.GetString(Encoding.GetEncoding("iso-8859-1").GetBytes(text));
        }
    }
}
