using Banking.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;
using Banking.Extensions;

namespace Banking.Service
{
    internal class DatabaseInitializer(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<BankingDbContext>()
                .UseSqlServer(_configuration.GetSqlConnectionString("ASPNETCORE_ENVIRONMENT")).Options;
            using var context = new BankingDbContext(options);

            context.Database.EnsureCreated();
        }
    }
}
