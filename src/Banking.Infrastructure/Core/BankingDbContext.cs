using Banking.Domain;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Core
{
    public class BankingDbContext(DbContextOptions<BankingDbContext> options) : DbContext(options)
    {
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
