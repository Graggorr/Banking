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
            var getAccountToTakeFundsTask = _repository.GetAccountAsync(request.AccountIdToTakeFunds);
            var getAccountToSendFundsTask = _repository.GetAccountAsync(request.AccountIdToSendFunds);
            var tasks = new Task<Result<Account>>[] { getAccountToTakeFundsTask, getAccountToSendFundsTask };
            Task.WaitAll(tasks, cancellationToken);
            var tasksResult = VerifyTaskResults(tasks);

            if (tasksResult.IsFailed)
            {
                return tasksResult;
            }

            var accountToTakeFunds = getAccountToTakeFundsTask.Result.Value;
            var accountToSendFunds = getAccountToSendFundsTask.Result.Value;

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

                    accountToSendFunds.MoneyAmount += request.AmountOfMoney;
                }
            }
            catch (Exception exception)
            {
                return Result.Fail($"System cannot transfer funds. Exception: {exception.Message}");
            }

            using var transaction = await _repository.BeginTransactionAsync();
            var updateAccountToTakeFundsTask = _repository.UpdateAccountAsync(accountToTakeFunds);
            var updateAccountToSendFundsTask = _repository.UpdateAccountAsync(accountToSendFunds);
            tasks = [updateAccountToTakeFundsTask, updateAccountToSendFundsTask];
            Task.WaitAll(tasks, cancellationToken);
            tasksResult = VerifyTaskResults(tasks);

            if (tasksResult.IsFailed)
            {
                transaction.Rollback();

                return tasksResult;
            }

            transaction.Commit();

            return Result.Ok(accountToTakeFunds.MoneyAmount);
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
