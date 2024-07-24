using FluentResults;
using MediatR;

namespace Banking.Application.Queries
{
    public record class GetAccountRequest(Guid Id) : IRequest<Result<AccountData>>;
    public record class GetAllAccountsRequest() : IRequest<Result<IEnumerable<AccountData>>>;
}
