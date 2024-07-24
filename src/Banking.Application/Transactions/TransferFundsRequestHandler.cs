using Banking.Domain;
using Banking.Infrastructure.Common;
using FluentResults;
using MediatR;

namespace Banking.Application.Transactions
{
    public class TransferFundsRequestHandler(IAccountRepository repository) : IRequestHandler<TransferFundsRequest, Result<double>>
    {
        private readonly IAccountRepository _repository = repository;

        public async Task<Result<double>> Handle(TransferFundsRequest request, CancellationToken cancellationToken)
        {
            var getAccountToTakeFundsResult = await _repository.GetAccountAsync(request.AccountIdToTakeFunds);
            var getAccountToSendFundsResult = await _repository.GetAccountAsync(request.AccountIdToSendFunds);

            if (getAccountToSendFundsResult.IsFailed)
            {
                return Result.Fail(getAccountToSendFundsResult.Errors);
            }

            if (getAccountToTakeFundsResult.IsFailed)
            {
                return Result.Fail(getAccountToTakeFundsResult.Errors);
            }

            var accountToTakeFunds = getAccountToTakeFundsResult.Value;
            var accountToSendFunds = getAccountToSendFundsResult.Value;

            try
            {
                checked
                {
                    var withdrawFundsResult = accountToTakeFunds.MoneyAmount - request.AmountOfMoney;

                    if (withdrawFundsResult < 0)
                    {
                        return Result.Fail($"You can't transfer ${request.AmountOfMoney}. Your balance is ${accountToTakeFunds.MoneyAmount}." +
                            $" Try to transfer less money.");
                    }

                    accountToTakeFunds.MoneyAmount = withdrawFundsResult;
                    accountToSendFunds.MoneyAmount += request.AmountOfMoney;
                }
            }
            catch (Exception exception)
            {
                return Result.Fail($"System cannot transfer funds. Exception: {exception.Message}");
            }

            var result = await _repository.UpdateAccountRangeAsync([accountToSendFunds, accountToTakeFunds]);

            return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok(accountToTakeFunds.MoneyAmount);
        }

        private static Result VerifyTaskResults(Task<Result<Account>>[] tasks)
        {
            var tasksResult = tasks.FirstOrDefault(x => x.Result.IsFailed).Result;

            if (tasksResult is not null)
            {
                return Result.Fail(tasksResult.Errors);
            }

            return Result.Ok();
        }
    }
}
