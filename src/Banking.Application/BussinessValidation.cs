using Banking.Infrastructure.Common;
using FluentResults;
using System.Text.RegularExpressions;

namespace Banking.Application
{
    internal class BusinessValidation
    {
        public static Result ValidateAccount(AccountData account, IAccountRepository accountRepository)
        {
            const string phoneNumberRegex = "^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";

            var task1 = Task.Run(() =>
            {
                return Regex.IsMatch(account.PhoneNumber, phoneNumberRegex)
                ? Result.Ok()
                : Result.Fail($"{account.PhoneNumber} is not valid. Valid example: 1234567890");
            });
            var task2 = accountRepository.IsPhoneNumberUnique(account.PhoneNumber);

            Task.WaitAll(task1, task2);

            if (task1.Result.IsFailed)
            {
                return task1.Result;
            }

            if (!task2.Result)
            {
                return Result.Fail($"{account.PhoneNumber} is already occupied.");
            }

            return Result.Ok();

        }
    }
}
