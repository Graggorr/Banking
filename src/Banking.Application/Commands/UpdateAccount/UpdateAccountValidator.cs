using FluentValidation;

namespace Banking.Application.Commands.UpdateAccount
{
    public class UpdateAccountValidator : AbstractValidator<UpdateAccountRequest>
    {
        public UpdateAccountValidator()
        {
            RuleFor(x => x.AccountData.Name).NotEmpty();
            RuleFor(x => x.AccountData.PhoneNumber).NotEmpty();
            RuleFor(x => x.AccountData.Money >= 0).NotEmpty();
        }
    }
}
