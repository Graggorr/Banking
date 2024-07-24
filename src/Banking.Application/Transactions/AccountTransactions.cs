using FluentResults;
using MediatR;

namespace Banking.Application.Transactions
{
    public record class AddFundsRequest(Guid Id, double AmountOfMoney) : IRequest<Result<double>>;
    public record class WithdrawFundsRequest(Guid Id, double AmountOfMoney) : IRequest<Result<double>>;
    public record class TransferFundsRequest(Guid AccountIdToTakeFunds, Guid AccountIdToSendFunds, double AmountOfMoney) : IRequest<Result<double>>; 
}
