using Dapper;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Banking.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private const string DEFAULT_ROUTE = "http://localhost:8080/api/banking/accounts";
        private const string CONNECTION_STRING = "Server=localhost;User ID=sa;Password=Pass@word;Database=banking;TrustServerCertificate=true;MultipleActiveResultSets=true";
        private readonly HttpClient _httpClient;

        public IntegrationTests()
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                connection.Execute($"DELETE FROM Account");
                connection.Close();
            }

            _httpClient = new HttpClient();
        }

        [TestMethod]
        public async Task TestAddAccount()
        {
            var accountData = new PostAccountRequest("Name", "1234567890", 10.0);

            var request = CreateRequestMessage(HttpMethod.Post, JsonSerializer.Serialize(accountData));
            var result = await _httpClient.SendAsync(request);

            Assert.IsTrue(result.StatusCode is HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task TestGetAccount()
        {
            var accountData = new PostAccountRequest("Name", "1234567890", 10.0);

            var postRequest = CreateRequestMessage(HttpMethod.Post, JsonSerializer.Serialize(accountData));

            var postResponse = await _httpClient.SendAsync(postRequest);
            var getRequestString = await postResponse.Content.ReadAsStringAsync();
            var getRequest = CreateRequestMessage(HttpMethod.Get, JsonSerializer.Deserialize<GetAccountRequest>(getRequestString).Id.ToString());

            var result = await _httpClient.SendAsync(getRequest);

            Assert.IsTrue(result.StatusCode is HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task TestGetAllAccounts()
        {
            var accountData1 = new PostAccountRequest("Name", "1234567890", 10.0);
            var accountData2 = new PostAccountRequest("Surname", "1334567890", 20.0);
            var accountData3 = new PostAccountRequest("NameSurname", "1434567890", 30.0);

            var postRequest1 = CreateRequestMessage(HttpMethod.Post, JsonSerializer.Serialize(accountData1));
            var postRequest2 = CreateRequestMessage(HttpMethod.Post, JsonSerializer.Serialize(accountData2));
            var postRequest3 = CreateRequestMessage(HttpMethod.Post, JsonSerializer.Serialize(accountData3));

            Task.WaitAll(_httpClient.SendAsync(postRequest1), _httpClient.SendAsync(postRequest2), _httpClient.SendAsync(postRequest3));

            var getAllRequest = CreateRequestMessage(HttpMethod.Get, string.Empty, null);
            var result = await _httpClient.SendAsync(getAllRequest);

            var content = await result.Content.ReadAsStringAsync();
            var accounts = JsonSerializer.Deserialize<GetAllAccountsResponse>(content);
            var count = accounts.Accounts.Count();

            Assert.IsTrue(result.IsSuccessStatusCode && count is 3);
        }

        [TestMethod]
        public async Task TestUpdateAccount()
        {
            var accountData1 = new PostAccountRequest("Name", "1234567890", 10);
            var accountData2 = new PostAccountRequest("Surname", "1334567890", 20);

            var postRequest = CreateRequestMessage(HttpMethod.Post, JsonSerializer.Serialize(accountData1));
            var postResponse = await _httpClient.SendAsync(postRequest);

            var content = await postResponse.Content.ReadAsStringAsync();
            var getAccountRequest = JsonSerializer.Deserialize<GetAccountRequest>(content);

            var putRequest = CreateRequestMessage(HttpMethod.Put, JsonSerializer.Serialize(accountData2), $"/{getAccountRequest.Id}");
            var result = await _httpClient.SendAsync(putRequest);

            content = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<GetAccountResponse>(content);
            var account = response.AccountData;

            Assert.IsTrue(result.IsSuccessStatusCode && account.Id.Equals(getAccountRequest.Id)
                && account.Name.Equals(accountData2.Name) && account.PhoneNumber.Equals(accountData2.PhoneNumber)
                && account.Money.Equals(accountData2.MoneyAmount));
        }

        [TestMethod]
        public async Task TestWithdrawFunds()
        {
            var accountData = new PostAccountRequest("Name", "1234567890", 20);

            var postRequest = CreateRequestMessage(HttpMethod.Post, JsonSerializer.Serialize(accountData));
            var postResponse = await _httpClient.SendAsync(postRequest);

            var content = await postResponse.Content.ReadAsStringAsync();
            var getAccountRequest = JsonSerializer.Deserialize<GetAccountRequest>(content);

            var putRequest = CreateRequestMessage(HttpMethod.Put, 10.ToString(), $"/transactions/withdraw/{getAccountRequest.Id}");
            var result = await _httpClient.SendAsync(putRequest);
            content = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<TransactionFundsRequest>(content);

            Assert.IsTrue(result.IsSuccessStatusCode && response.FundsAmount is 10);
        }

        [TestMethod]
        public async Task TestAddFunds()
        {
            var accountData = new PostAccountRequest("Name", "1234567890", 20);

            var postRequest = CreateRequestMessage(HttpMethod.Post, JsonSerializer.Serialize(accountData));
            var postResponse = await _httpClient.SendAsync(postRequest);

            var content = await postResponse.Content.ReadAsStringAsync();
            var getAccountRequest = JsonSerializer.Deserialize<GetAccountRequest>(content);

            var putRequest = CreateRequestMessage(HttpMethod.Put, 10.ToString(), $"/transactions/add/{getAccountRequest.Id}");
            var result = await _httpClient.SendAsync(putRequest);
            content = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<TransactionFundsRequest>(content);

            Assert.IsTrue(result.IsSuccessStatusCode && response.FundsAmount is 30);
        }

        [TestMethod]
        public async Task TestTransferFunds()
        {
            var accountData1 = new PostAccountRequest("Name", "1234567890", 10.0);
            var accountData2 = new PostAccountRequest("Surname", "1334567890", 20.0);

            var postRequest1 = CreateRequestMessage(HttpMethod.Post, JsonSerializer.Serialize(accountData1));
            var postRequest2 = CreateRequestMessage(HttpMethod.Post, JsonSerializer.Serialize(accountData2));

            var task1 = Task.Run(async () =>
            {
                var response = await _httpClient.SendAsync(postRequest1);
                var content = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<GetAccountRequest>(content);
            });
            var task2 = Task.Run(async () =>
            {
                var response = await _httpClient.SendAsync(postRequest2);
                var content = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<GetAccountRequest>(content);
            });

            Task.WaitAll(task1, task2);

            var accountData1Id = task1.Result.Id;
            var accountData2Id = task2.Result.Id;

            var transferFundsRequest = CreateRequestMessage(HttpMethod.Put,
                JsonSerializer.Serialize(new TransferFundsRequest(accountData2Id, 10)), $"/transactions/transfer/{accountData1Id}");

            var response = await _httpClient.SendAsync(transferFundsRequest);
            var content = await response.Content.ReadAsStringAsync();
            var leftAmount = JsonSerializer.Deserialize<TransactionFundsRequest>(content).FundsAmount;

            Assert.IsTrue(response.IsSuccessStatusCode && leftAmount is 0);
        }

        private static HttpRequestMessage CreateRequestMessage(HttpMethod httpMethod, string content, string additionalPath = null) =>
            new()
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json"),
                Method = httpMethod,
                RequestUri = new Uri(DEFAULT_ROUTE + additionalPath)
            };
    }
}