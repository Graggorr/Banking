using Banking.Domain;
using Banking.Infrastructure.Common;
using FluentResults;
using MediatR;

namespace Banking.Application.Commands.UpdateAccount
{
    public class UpdateAccountRequestHandler(IAccountRepository repository) : IRequestHandler<UpdateAccountRequest, Result<AccountData>>
    {
        private readonly IAccountRepository _repository = repository;

        public async Task<Result<AccountData>> Handle(UpdateAccountRequest request, CancellationToken cancellationToken)
        {
            var accountData = request.AccountData;
            var validationResult = BusinessValidation.ValidateAccount(accountData);

            if (validationResult.IsFailed)
            {
                return validationResult;
            }

            var account = new Account { Id = accountData.Id, MoneyAmount = accountData.Money, Name = accountData.Name, PhoneNumber = accountData.PhoneNumber };
            var updateResult = await _repository.UpdateAccountAsync(account);

            return updateResult.IsSuccess ? Result.Ok(request.AccountData) : Result.Fail(updateResult.Errors);
        }
    }
}