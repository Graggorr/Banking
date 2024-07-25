using Banking.Domain;
using Banking.Infrastructure.Common;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

        public async Task<Result<IEnumerable<Account>>> UpdateAccountRangeAsync(IEnumerable<Account> accounts)
        {
            //to avoid concurrent exception in dbcontext and do not create a separate dbcontext instance
            //in case of have heavy operation. It's been decided to update 2 instance one by one.

            var entities = new List<Account>();

            foreach (var account in accounts)
            {
                var entity = _context.Accounts.Find(account.Id);

                if (entity is null)
                {
                    return Result.Fail($"{account.Id} is not contained.");
                }

                entities.Add(entity);
            }

            foreach (var entity in entities)
            {
                var account = accounts.First(x => x.Id.Equals(entity.Id));
                entity.Name = account.Name;
                entity.MoneyAmount = account.MoneyAmount;
                entity.PhoneNumber = account.PhoneNumber;
            }

            await _context.SaveChangesAsync();

            return entities;
        }

        public async Task<bool> IsPhoneNumberUnique(string phoneNumber) => !await _context.Accounts.AnyAsync(x => x.PhoneNumber.Equals(phoneNumber));
        private static Result<Account> AccountIsNotFoundError() => Result.Fail("Account with the following GUID ID is not found.");
    }
}
