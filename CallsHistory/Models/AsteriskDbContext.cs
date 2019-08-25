using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CallsHistory.Models
{
    public class AsteriskDbContext : DbContext
    {
        public AsteriskDbContext(DbContextOptions<AsteriskDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }    
    }
}
