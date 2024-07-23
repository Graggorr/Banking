using Banking.Domain;
using Banking.Infrastructure.Common;
using FluentResults;

namespace Banking.Infrastructure.Core
{
    public class AccountRepository(BankingDbContext context) : IAccountRepository
    {
        private readonly BankingDbContext _context = context;

        public Task AddAccountAsync(Account account)
        {
            _context.Accounts.Add(account);

            return _context.SaveChangesAsync();
        }

        public async Task<Result<Account>> GetAccountAsync(Guid id)
        {
            var account = await _context.Accounts.FindAsync(id);

            return account is not null ? Result.Ok(account) : AccountIsNotFoundError();
        }

        public Task<Result<IEnumerable<Account>>> GetAllAccountsAsync()
        {
            if (_context.Accounts.Any())
            {
                return Task.FromResult(Result.Ok(_context.Accounts.AsEnumerable()));
            }

            return Task.FromResult<Result<IEnumerable<Account>>>(Result.Fail("No accounts are contained."));
        }

        public async Task<Result<Account>> RemoveAccountAsync(Guid id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account is not null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();

                return Result.Ok(account);
            }

            return AccountIsNotFoundError();
        }

        public async Task<Result<Account>> UpdateAccountAsync(Account account)
        {
            var entity = await _context.Accounts.FindAsync(account.Id);

            if (entity is null)
            {
                return AccountIsNotFoundError();
            }

            entity.PhoneNumber = account.PhoneNumber;
            entity.MoneyAmount = account.MoneyAmount;
            entity.Name = account.Name;

            await _context.SaveChangesAsync();

            return Result.Ok(entity);
        }

        private static Result<Account> AccountIsNotFoundError() => Result.Fail("Account with the following GUID ID is not found.");
    }
}
