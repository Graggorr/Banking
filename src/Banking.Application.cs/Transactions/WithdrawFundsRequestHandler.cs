using Banking.Infrastructure.Common;
using FluentResults;
using MediatR;

namespace Banking.Application.Transactions
{
    public class WithdrawFundsRequestHandler(IAccountRepository repository) : IRequestHandler<WithdrawFundsRequest, Result<double>>
    {
        private readonly IAccountRepository _repository = repository;

        public async Task<Result<double>> Handle(WithdrawFundsRequest request, CancellationToken cancellationToken)
        {
            var getAccountResult = await _repository.GetAccountAsync(request.Id);

            if (getAccountResult.IsFailed)
            {
                return Result.Fail(getAccountResult.Errors);
            }

            var account = getAccountResult.Value;
            double withdrawResult;

            try
            {
                checked
                {
                    withdrawResult = account.MoneyAmount - request.AmountOfMoney;
                }
            }

            catch (Exception exception)
            {
                return Result.Fail($"System cannot withdraw funds. Exception: {exception.Message}");
            }

            if (withdrawResult < 0)
            {
                return Result.Fail($"You can't withdraw ${request.AmountOfMoney}. Your balance is ${account.MoneyAmount}. Try to withdraw less money.");
            }

            var updateResult = await _repository.UpdateAccountAsync(getAccountResult.Value);

            return updateResult.ToResult();
        }
    }
}
