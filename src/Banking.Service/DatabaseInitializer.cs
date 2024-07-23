using Banking.Infrastructure.Core;

namespace Banking.Service
{
    internal class DatabaseInitializer(BankingDbContext context)
    {
        private readonly BankingDbContext _context = context;

        public void Initialize() => _context.Database.EnsureCreated();
    }
}
