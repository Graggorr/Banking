﻿using Banking.Domain;
using Banking.Infrastructure.Common;
using FluentResults;
using MediatR;

namespace Banking.Application.Commands.CreateAccount
{
    public class RegisterAccountRequestHandler(IAccountRepository repository) : IRequestHandler<RegisterAccountRequest, Result<Guid>>
    {
        private readonly IAccountRepository _repository = repository;

        public async Task<Result<Guid>> Handle(RegisterAccountRequest request, CancellationToken cancellationToken)
        {
            var accountData = request.AccountData;
            var result = BusinessValidation.ValidateAccount(accountData, _repository);

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
