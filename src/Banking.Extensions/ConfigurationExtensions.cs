using Microsoft.Extensions.Configuration;

namespace Banking.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetSqlConnectionString(this IConfiguration configuration, string environmentVariableName)
        {
            return Environment.GetEnvironmentVariable(environmentVariableName).Equals("Development")
                ? configuration.GetConnectionString("BankingDb") : configuration.GetConnectionString("BankingDbDocker");
        }
    }
}
