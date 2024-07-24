using Banking.Infrastructure.Common;
using FluentResults;
using MediatR;

namespace Banking.Application.Queries.GetAllAccounts
{
    public class GetAllAccountsRequestHandler(IAccountRepository repository) : IRequestHandler<GetAllAccountsRequest, Result<IEnumerable<AccountData>>>
    {
        private readonly IAccountRepository _repository = repository;

        public async Task<Result<IEnumerable<AccountData>>> Handle(GetAllAccountsRequest request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllAccountsAsync();

            if (result.IsFailed)
            {
                return Result.Fail(result.Errors);
            }

            var accounts = result.Value;
            var enumerable = new List<AccountData>();

            foreach(var account in accounts)
            {
                enumerable.Add(new AccountData(account.Id, account.Name, account.MoneyAmount, account.PhoneNumber));
            }

            return Result.Ok(enumerable.AsEnumerable());
        }
    }
}
