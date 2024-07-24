using MediatR;
using FluentResults;

namespace Banking.Application.Commands
{
    public record class RegisterAccountRequest(AccountData AccountData) : IRequest<Result<Guid>>;
    public record class UpdateAccountRequest(AccountData AccountData) : IRequest<Result<AccountData>>;
    public record class DeleteAccountRequest(Guid Id) : IRequest<Result<AccountData>>;
}
