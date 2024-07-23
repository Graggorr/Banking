using FluentValidation;

namespace Banking.Application.Commands.CreateAccount
{
    public class CreateAccountValidator: AbstractValidator<CreateAccountRequest>
    {
        public CreateAccountValidator()
        {
            RuleFor(x => x.AccountData.Name).NotEmpty();
            RuleFor(x => x.AccountData.PhoneNumber).NotEmpty();
            RuleFor(x => x.AccountData.Money >= 0).NotEmpty();
        }
    }
}
