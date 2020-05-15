using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.Models
{
    public class UsersDBContext : DbContext
    {
        public UsersDBContext(DbContextOptions<UsersDBContext> options)
           : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaypalTransaction> PaypalTransactions { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ProviderChat> Chats { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<PaypalTransaction> Transactions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProviderImage> Images { get; set; }


    }
}
