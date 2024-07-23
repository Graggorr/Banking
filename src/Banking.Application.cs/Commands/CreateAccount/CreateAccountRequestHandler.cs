using Banking.Domain;
using Banking.Infrastructure.Common;
using FluentResults;
using MediatR;

namespace Banking.Application.Commands.CreateAccount
{
    public class CreateAccountRequestHandler(IAccountRepository repository) : IRequestHandler<CreateAccountRequest, Result<Guid>>
    {
        private readonly IAccountRepository _repository = repository;

        public async Task<Result<Guid>> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
        {
            var accountData = request.AccountData;
            var result = BusinessValidation.ValidateAccount(accountData);

            if (result.IsFailed)
            {
                return result;
            }

            var account = new Account { Id = accountData.Id, MoneyAmount = accountData.Money, Name = accountData.Name, PhoneNumber = accountData.PhoneNumber };
            await _repository.AddAccountAsync(account);

            return Result.Ok(account.Id);
        }
    }
}
