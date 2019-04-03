using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CallsHistory.Models;

namespace CallsHistory.Repositories
{
    public interface IRepository
    {
        IQueryable<Call> Calls { get; }

        SearchResult SearchCalls(CallsFilter filter);
    }

    public interface IUsersRepository
    {
        List<User> Users { get; }

        User GetUser(string ext);
    }
}
