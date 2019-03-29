using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CallsHistory.Models;

namespace CallsHistory.Repositories
{
    public class EFRepository : IRepository
    {
        private AppDbContext context;

        public EFRepository(AppDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Call> Calls => context.Calls;

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
