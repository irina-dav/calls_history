using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallsHistory.Models;
using CallsHistory.Services;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace CallsHistory.Repositories
{
    public class EFRepository : IRepository
    {
        private List<AppDbContext> contexts;
        private ILogger<EFRepository> logger;
        private DateTimeService dateTimeService;

        public Dictionary<string, int> TimeZoneOffsets { get; set; }

        public EFRepository(IDbContextFactoryService dbFactory, ILogger<EFRepository> logger, DateTimeService dateTimeService)
        {
            this.dateTimeService = dateTimeService;
            contexts = dbFactory.CreateDbs<AppDbContext>("cds");
            foreach (AppDbContext cont in contexts)
            {                
                string serverName = cont.Database.GetDbConnection().ConnectionString.Split(";")[0].Split("=")[1];
                cont.OffsetUTC = dateTimeService.GetServerOffsetUTC(serverName);
            }
            this.logger = logger;
            TimeZoneOffsets = dateTimeService.TimeZonesOffsetUTC;
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
            int viewOffsetUTC = filter.OffsetUTC;

            IQueryable<Call> callsData = Enumerable.Empty<Call>().AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter.SrcCallNumber) && !string.IsNullOrWhiteSpace(filter.DstCallNumber))
                callsData = contexts.SelectMany(x => x.Calls.Where(c => 
                    c.CallDate >= filter.CallDateFrom.AddHours(x.OffsetUTC) && c.CallDate < filter.CallDateTo.AddHours(x.OffsetUTC)
                    && c.Src == filter.SrcCallNumber
                    && c.Dst == filter.DstCallNumber).AsQueryable().ToList().Select(c => { c.CallDateUTC = c.CallDate.AddHours(-x.OffsetUTC + viewOffsetUTC); return c; })).AsQueryable();            
            else if (!string.IsNullOrWhiteSpace(filter.SrcCallNumber))
                callsData = contexts.SelectMany(x => x.Calls.Where(c =>
                    c.CallDate.AddHours(-8) >= filter.CallDateFrom.AddHours(x.OffsetUTC) && c.CallDate < filter.CallDateTo.AddHours(x.OffsetUTC)
                    && c.Src == filter.SrcCallNumber).AsQueryable().ToList().Select(c => { c.CallDateUTC = c.CallDate.AddHours(-x.OffsetUTC + viewOffsetUTC); return c; })).AsQueryable();
            else if (!string.IsNullOrWhiteSpace(filter.DstCallNumber))
                callsData = contexts.SelectMany(x => x.Calls.Where(c =>
                    c.CallDate >= filter.CallDateFrom.AddHours(x.OffsetUTC) && c.CallDate < filter.CallDateTo.AddHours(x.OffsetUTC)
                    && c.Dst == filter.DstCallNumber).AsQueryable().ToList().Select(c => { c.CallDateUTC = c.CallDate.AddHours(-x.OffsetUTC + viewOffsetUTC); return c; })).AsQueryable();

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                callsData = callsData.GroupBy(x => new { x.Src, x.Dst, CallDateUTC = x.CallDateUTC.AddSeconds(-x.CallDateUTC.Second)}).
                    Select(g => g.First()).OrderBy(sortColumn + " " + sortColumnDirection);
            }

            int recordsTotal = callsData.Count();
            var data = callsData.Skip(offset).Take(pageSize).ToList();

            return new SearchResult() { TotalCalls = recordsTotal, CallsPage = data };
        }
    }


    public class EFUserRepository : IUsersRepository
    {
        private List<AsteriskDbContext> contexts;

        public EFUserRepository(IDbContextFactoryService dbFactory)
        {
        
            contexts = dbFactory.CreateDbs<AsteriskDbContext>("asterisk");           
            Users = contexts.SelectMany(c => c.Users).ToList();
        }

        public List<User> Users { get; }

        public User GetUser(string ext)
        {
            return Users.FirstOrDefault(u => u.Id == ext);
        }
    }
}
