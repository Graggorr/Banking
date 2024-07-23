using Banking.Application;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Service
{
    //post
    public record class PostAccountRequest(string Name, string PhoneNumber, double MoneyAmount);
    public record class PostAccountResponse(Guid Id);

    //put
    public record class PutAccountRequest([FromRoute] Guid Id, [FromBody] PutAccountRequestBody Body);
    public record class PutAccountRequestBody(string Name, string PhoneNumber, double MoneyAmount);
    public record class PutAccountResponse(AccountData AccountData);

    //get 
    public record class GetAccountRequest([FromRoute] Guid Id);
    public record class GetAccountResponse(AccountData AccountData);
    public record class GetAllAccountsRequest();
    public record class GetAllAccountsResponse(IEnumerable<AccountData> AccountData);

    //delete
    public record class DeleteAccountRequest([FromRoute] Guid Id);
    public record class DeleteAccountResponse(AccountData AccountData);

    //transactions
    public record class AddFundsRequest([FromRoute] Guid Id, [FromBody] double FundsAmount);
    public record class AddFundsResponse(double LeftFundsAmount);
    public record class WithdrawFundsRequest([FromRoute] Guid Id, [FromBody] double FundsAmount);
    public record class WithdrawFundsResponse(double FundsAmount);
    public record class TransferFundsRequest([FromRoute] Guid AccountIdToTakeFunds, [FromBody] TransferFundsRequestBody Body);
    public record class TransferFundsRequestBody(Guid AccountIdToSendFunds, double FundsAmount);
    public record class TransferFundsResponse(double FundsAmount);
}
