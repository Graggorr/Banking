using Banking.Application.Commands;
using Banking.Application;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using FluentResults;
using System.Text;

namespace Banking.Service
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapAccounts(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/accounts").WithTags("Accounts");

            group.MapPost(string.Empty, PostAccount);
            group.MapGet("/{id:guid}", GetAccount);
            group.MapGet(string.Empty, GetAllAccounts);
            group.MapDelete("/{id:guid}", DeleteAccount);
            group.MapPut("/{id:guid}", PutAccount);

            //transactions
            group.MapPut("/transactions/withdraw/{id:guid}", WithdrawFunds);
            group.MapPut("/transactions/add/{id:guid}", AddFunds);
            group.MapPut("/transactions/transfer/{accountIdToTakeFunds:guid}", TransferFunds);

            return builder;
        }

        private static async Task<Results<Ok<PostAccountResponse>, BadRequest<string>>> PostAccount(
            [FromBody] PostAccountRequest request,
            [FromServices] IMediator mediator)
        {
            var registerRequest = new RegisterAccountRequest(new AccountData(Guid.NewGuid(), request.Name, request.MoneyAmount, request.PhoneNumber));
            var response = await mediator.Send(registerRequest);

            return response.IsSuccess
                ? TypedResults.Ok(new PostAccountResponse(response.Value))
                : TypedResults.BadRequest(CreateErrorResponse(response.Errors));
        }

        private static async Task<Results<Ok<PutAccountResponse>, BadRequest<string>, NotFound>> PutAccount(
            [AsParameters] PutAccountRequest request,
            [FromServices] IMediator mediator)
        {
            var putRequest = new UpdateAccountRequest(new AccountData(request.Id, request.Body.Name, request.Body.MoneyAmount, request.Body.PhoneNumber));
            var response = await mediator.Send(putRequest);

            if (response.IsFailed)
            {
                if(response.Errors.FirstOrDefault().Message.Contains("is not found."))
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.BadRequest(CreateErrorResponse(response.Errors));
            }

            return TypedResults.Ok(new PutAccountResponse(response.Value));
        }

        private static async Task<Results<Ok<GetAccountResponse>, BadRequest<string>, NotFound>> GetAccount(
            [AsParameters] GetAccountRequest request,
            [FromServices] IMediator mediator)
        {
            var getRequest = new Application.Queries.GetAccountRequest(request.Id);
            var response = await mediator.Send(getRequest);

            if (response.IsFailed)
            {
                if (response.Errors.FirstOrDefault().Message.Contains("is not found"))
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.BadRequest(CreateErrorResponse(response.Errors));
            }

            return TypedResults.Ok(new GetAccountResponse(response.Value));
        }

        private static async Task<Results<Ok<GetAllAccountsResponse>, NotFound>> GetAllAccounts(
            [AsParameters] GetAllAccountsRequest request,
            [FromServices] IMediator mediator)
        {
            var getAllRequest = new Application.Queries.GetAllAccountsRequest();
            var response = await mediator.Send(getAllRequest);

            if (response.IsFailed)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(new GetAllAccountsResponse(response.Value));
        }

        private static async Task<Results<Ok<DeleteAccountResponse>, BadRequest<string>, NotFound>> DeleteAccount(
            [AsParameters] DeleteAccountRequest request,
            [FromServices] IMediator mediator)
        {
            var deleteRequest = new Application.Commands.DeleteAccountRequest(request.Id);
            var response = await mediator.Send(deleteRequest);

            if (response.IsFailed)
            {
                if (response.Errors.FirstOrDefault().Message.Contains("is not found"))
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.BadRequest(CreateErrorResponse(response.Errors));
            }

            return TypedResults.Ok(new DeleteAccountResponse(response.Value));
        }

        private static async Task<Results<Ok<WithdrawFundsResponse>, BadRequest<string>, NotFound>> WithdrawFunds(
            [AsParameters] WithdrawFundsRequest request,
            [FromServices] IMediator mediator)
        {
            var withdrawRequest = new Application.Transactions.WithdrawFundsRequest(request.Id, request.FundsAmount);
            var response = await mediator.Send(withdrawRequest);


            if (response.IsFailed)
            {
                if (response.Errors.FirstOrDefault().Message.Contains("is not found"))
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.BadRequest(CreateErrorResponse(response.Errors));
            }

            return TypedResults.Ok(new WithdrawFundsResponse(response.Value));
        }

        private static async Task<Results<Ok<AddFundsResponse>, BadRequest<string>, NotFound>> AddFunds(
            [AsParameters] AddFundsRequest request,
            [FromServices] IMediator mediator)
        {
            var withdrawRequest = new Application.Transactions.AddFundsRequest(request.Id, request.FundsAmount);
            var response = await mediator.Send(withdrawRequest);


            if (response.IsFailed)
            {
                if (response.Errors.FirstOrDefault().Message.Contains("is not found"))
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.BadRequest(CreateErrorResponse(response.Errors));
            }

            return TypedResults.Ok(new AddFundsResponse(response.Value));
        }

        private static async Task<Results<Ok<TransferFundsResponse>, BadRequest<string>, NotFound>> TransferFunds(
           [AsParameters] TransferFundsRequest request,
           [FromServices] IMediator mediator)
        {
            var withdrawRequest = new Application.Transactions.TransferFundsRequest(request.AccountIdToTakeFunds,
                request.Body.AccountIdToSendFunds, request.Body.FundsAmount);
            var response = await mediator.Send(withdrawRequest);

            if (response.IsFailed)
            {
                if (response.Errors.FirstOrDefault().Message.Contains("is not found"))
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.BadRequest(CreateErrorResponse(response.Errors));
            }

            return TypedResults.Ok(new TransferFundsResponse(response.Value));
        }

        private static string CreateErrorResponse(List<IError> errors)
        {
            var stringBuilder = new StringBuilder();
            errors.ForEach(e => stringBuilder.AppendLine(e.Message));

            return stringBuilder.ToString();
        }
    }
}
