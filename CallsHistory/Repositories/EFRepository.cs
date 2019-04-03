using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallsHistory.Models;
using CallsHistory.Services;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;

namespace CallsHistory.Repositories
{
    public class EFRepository : IRepository
    {
        private List<AppDbContext> contexts;
        private ILogger<EFRepository> logger;

        public EFRepository(TextService textService, IDbContextFactoryService dbFactory, ILogger<EFRepository> logger)
        {
            contexts = dbFactory.CreateDbs<AppDbContext>("cds");
            this.logger = logger;
        }

        public IQueryable<Call> Calls => contexts.SelectMany(c => c.Calls).AsQueryable();
  
        public SearchResult SearchCalls(CallsFilter filter)
        {
            logger.LogInformation($"SearchCalls with filter: dst={filter.DstCallNumber}, " +
                $"src={filter.SrcCallNumber}, " +
                $"dtFrom={filter.CallDateFrom}, " +
                $"dtTo={filter.CallDateTo}");

            var offset = filter.Offset;
            var pageSize = filter.Limit;
            var sortColumn = filter.Sort;
            var sortColumnDirection = filter.Order;

            IQueryable<Call> callsData = Enumerable.Empty<Call>().AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter.SrcCallNumber) && !string.IsNullOrWhiteSpace(filter.DstCallNumber))
                callsData = contexts.SelectMany(x => x.Calls.Where(c => 
                    c.CallDate >= filter.CallDateFrom  && c.CallDate < filter.CallDateTo 
                    && c.Src == filter.SrcCallNumber
                    && c.Dst == filter.DstCallNumber).AsQueryable()).AsQueryable();
            else if (!string.IsNullOrWhiteSpace(filter.SrcCallNumber))
                callsData = contexts.SelectMany(x => x.Calls.Where(c =>
                    c.CallDate >= filter.CallDateFrom && c.CallDate < filter.CallDateTo
                    && c.Src == filter.SrcCallNumber).AsQueryable()).AsQueryable();
            else if (!string.IsNullOrWhiteSpace(filter.DstCallNumber))
                callsData = contexts.SelectMany(x => x.Calls.Where(c =>
                    c.CallDate >= filter.CallDateFrom && c.CallDate < filter.CallDateTo
                    && c.Dst == filter.DstCallNumber).AsQueryable()).AsQueryable();

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                callsData = callsData.OrderBy(sortColumn + " " + sortColumnDirection);
            }

            int recordsTotal = callsData.Count();
            var data = callsData.Skip(offset).Take(pageSize).ToList();

            return new SearchResult() { TotalCalls = recordsTotal, CallsPage = data };
        }
    }

    public class EFUserRepository : IUsersRepository
    {
        private List<UserDbContext> contexts;

        public EFUserRepository(IDbContextFactoryService dbFactory)
        {
            contexts = dbFactory.CreateDbs<UserDbContext>("asterisk");           
            Users = contexts.SelectMany(c => c.Users).ToList();
        }

        public List<User> Users { get; }

        public User GetUser(string ext)
        {
            return Users.FirstOrDefault(u => u.Id == ext);
        }
    }
}
