using FluentResults;
using System.Text.RegularExpressions;

namespace Banking.Application
{
    internal class BusinessValidation
    {
        public static Result ValidateAccount(AccountData account)
        {
            const string phoneNumberRegex = "^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";

            return Regex.IsMatch(account.PhoneNumber, phoneNumberRegex)
                ? Result.Ok()
                : Result.Fail($"{account.PhoneNumber} is not valid. Valid example: 1234567890");
        }
    }
}
