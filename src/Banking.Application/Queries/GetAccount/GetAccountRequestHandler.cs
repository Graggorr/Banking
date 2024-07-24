using Banking.Infrastructure.Common;
using FluentResults;
using MediatR;

namespace Banking.Application.Queries.GetAccount
{
    public class GetAccountRequestHandler(IAccountRepository repository) : IRequestHandler<GetAccountRequest, Result<AccountData>>
    {
        private readonly IAccountRepository _repository = repository;

        public async Task<Result<AccountData>> Handle(GetAccountRequest request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAccountAsync(request.Id);

            return result.IsSuccess 
                ? Result.Ok(new AccountData(result.Value.Id, result.Value.Name, result.Value.MoneyAmount, result.Value.PhoneNumber))
                : Result.Fail(result.Errors);
        }
    }
}
