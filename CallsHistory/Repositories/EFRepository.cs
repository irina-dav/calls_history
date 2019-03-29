using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallsHistory.Models;
using CallsHistory.Services;

namespace CallsHistory.Repositories
{
    public class EFRepository : IRepository
    {
        private AppDbContext context;
        private DateTime problemEncodeStart = new DateTime(2019, 03, 04, 13, 30, 0);
        private DateTime problemEncodeEnd = new DateTime(2020, 01, 01, 00, 0, 0);
        private TextService textService;

        public EFRepository(AppDbContext context, TextService textService)
        {
            this.context = context;
            this.textService = textService;
        }

        public IEnumerable<Call> Calls => context.Calls;

        public string GetCallerName(Call call)
        {
            if (call.CallDate >= problemEncodeStart && call.CallDate <= problemEncodeEnd)
                return textService.ContvertToUtf8(call.SrcName);
            else
                return call.SrcName;
        }

        public IEnumerable<Call> SearchCalls(CallsFilter filter)
        {
            List<Call> calls = Calls.Where(c => 
                c.CallDate >= filter.CallDateFrom && c.CallDate < filter.CallDateTo && 
                (c.Dst == filter.CallNumber || c.Src == filter.CallNumber)).ToList();

            return calls;
        }

    }

    public class EFUserRepository : IUsersRepository
    {
        private UserDbContext context;

        public EFUserRepository(UserDbContext context)
        {
            this.context = context;
        }


        public IEnumerable<User> Users => context.Users;

        public User GetUser(string ext)
        {
            return Users.FirstOrDefault(u => u.Id == ext);
        }
     
    }
}
