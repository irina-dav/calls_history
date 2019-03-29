using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CallsHistory.Models;

namespace CallsHistory.Repositories
{
    public interface IRepository
    {
        IEnumerable<Call> Calls { get; }

        IEnumerable<Call> SearchCalls(CallsFilter filter);
    }

    public interface IUsersRepository
    {
        IEnumerable<User> Users { get; }

        User GetUser(string ext);
       

    }
}
