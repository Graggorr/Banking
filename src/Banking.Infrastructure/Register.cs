using Banking.Infrastructure.Common;
using Banking.Infrastructure.Core;
using Banking.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Infrastructure
{
    public static class Register
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddDbContext<BankingDbContext>(options => options.UseSqlServer(configuration.GetSqlConnectionString("ASPNETCORE_ENVIRONMENT")));

            return services;
        }
    }
}
