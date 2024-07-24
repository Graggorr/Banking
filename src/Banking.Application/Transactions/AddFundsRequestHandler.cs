using Banking.Infrastructure.Common;
using FluentResults;
using MediatR;

namespace Banking.Application.Transactions
{
    public class AddFundsRequestHandler(IAccountRepository repository) : IRequestHandler<AddFundsRequest, Result<double>>
    {
        private readonly IAccountRepository _repository = repository;

        public async Task<Result<double>> Handle(AddFundsRequest request, CancellationToken cancellationToken)
        {
            var getAccountResult = await _repository.GetAccountAsync(request.Id);

            if (getAccountResult.IsFailed)
            {
                return Result.Fail(getAccountResult.Errors);
            }

            var account = getAccountResult.Value;

            try
            {
                checked
                {
                    account.MoneyAmount += request.AmountOfMoney;
                }
            }
            catch (Exception exception)
            {
                return Result.Fail($"System cannot add funds. Exception: {exception.Message}");
            }

            var updateResult = await _repository.UpdateAccountAsync(account);

            return updateResult.IsSuccess ? Result.Ok(updateResult.Value.MoneyAmount) : Result.Fail(updateResult.Errors);
        }
    }
}
