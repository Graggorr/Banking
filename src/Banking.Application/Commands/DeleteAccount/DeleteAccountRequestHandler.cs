using Banking.Infrastructure.Common;
using FluentResults;
using MediatR;

namespace Banking.Application.Commands.DeleteAccount
{
    public class DeleteAccountRequestHandler(IAccountRepository repository) : IRequestHandler<DeleteAccountRequest, Result<AccountData>>
    {
        private readonly IAccountRepository _repository = repository;

        public async Task<Result<AccountData>> Handle(DeleteAccountRequest request, CancellationToken cancellationToken)
        {
            var result = await _repository.RemoveAccountAsync(request.Id);

            if (result.IsSuccess)
            {
                var value = result.Value;

                return Result.Ok(new AccountData(value.Id, value.Name, value.MoneyAmount, value.PhoneNumber));
            }

            return Result.Fail(result.Errors);
        }
    }
}
