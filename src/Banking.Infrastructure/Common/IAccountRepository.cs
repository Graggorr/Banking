using Banking.Domain;
using FluentResults;

namespace Banking.Infrastructure.Common
{
    public interface IAccountRepository
    {
        public Task AddAccountAsync(Account account);
        public Task<Result<Account>> GetAccountAsync(Guid id);
        public Task<Result<Account>> RemoveAccountAsync(Guid id);
        public Task<Result<Account>> UpdateAccountAsync(Account account);
        public Task<Result<IEnumerable<Account>>> GetAllAccountsAsync();
    }
}
