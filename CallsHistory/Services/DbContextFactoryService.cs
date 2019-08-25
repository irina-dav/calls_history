using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace CallsHistory.Models
{

    public interface IDbContextFactoryService
    {
        List<T> CreateDbs<T>(string nameTypeDb) where T : DbContext;
    }

    public class DbContextFactoryService : IDbContextFactoryService
    {       
        private ILogger<DbContextFactoryService> logger;

        private ILoggerFactory GetLoggerFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
                   builder.AddDebug()
                          .AddConsole()
                          .AddFilter(DbLoggerCategory.Database.Command.Name,
                                     LogLevel.Debug));
            return serviceCollection.BuildServiceProvider()
                    .GetService<ILoggerFactory>();
        }

        public DbContextFactoryService(Dictionary<string, List<string>> connections, ILogger<DbContextFactoryService> logger)
        {
            this.logger = logger;
            this.connections = connections;
        }

        private  Dictionary<string, List<string>> connections;

        public  void AddListConnectionString(string name, List<string> conns)
        {
            connections.Add(name, conns);
        }

        public  List<T> CreateDbs<T>(string nameTypeDb) where T : DbContext
        {
            List<T> dbs = new List<T>();
            foreach (string conn in connections[nameTypeDb])
            {
                var db = Create<T>(conn);
                try
                {
                    if (db.Database.CanConnect())
                        dbs.Add(db);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error connection to {conn}: {ex}");
                }
            }
            return dbs;
        }

        public T Create<T>(string conn) where T: DbContext
        {
            if (!string.IsNullOrEmpty(conn))
            {               
                var optionsBuilder = new DbContextOptionsBuilder<T>();
                //optionsBuilder.UseLoggerFactory(GetLoggerFactory());
                optionsBuilder.UseMySql(conn);                
                var db = (T)Activator.CreateInstance(typeof(T), (optionsBuilder.Options));
                return db;                              
            }
            else
            {
                throw new ArgumentNullException("Connection failed, connection string is null or empty");
            }
        }
    }
}
