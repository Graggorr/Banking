using System.Text.Json.Serialization;

namespace Banking.Tests
{
    internal class AccountData(Guid id, string name, string phoneNumber, double money)
    {
        [JsonPropertyName("id")]
        public Guid Id { get; } = id;
        [JsonPropertyName("name")]
        public string Name { get; } = name;
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; } = phoneNumber;
        [JsonPropertyName("money")]
        public double Money { get; } = money;
    }
    internal class PostAccountRequest(string name, string phoneNumber, double money)
    {
        public string Name { get; } = name;
        public string PhoneNumber { get; } = phoneNumber;
        public double MoneyAmount { get; } = money;
    }

    internal class GetAccountRequest(Guid id)
    {
        [JsonPropertyName("id")]
        public Guid Id { get; } = id;
    }
    internal class GetAccountResponse(AccountData accountData)
    {
        [JsonPropertyName("accountData")]
        public AccountData AccountData { get; } = accountData;
    }
    internal class GetAllAccountsResponse(IEnumerable<AccountData> accounts)
    {
        [JsonPropertyName("accountData")]
        public IEnumerable<AccountData> Accounts { get; } = accounts;
    }
    internal class TransactionFundsRequest(double fundsAmount)
    {
        [JsonPropertyName("fundsAmount")]
        public double FundsAmount { get; } = fundsAmount;
    }
    internal class TransferFundsRequest(Guid accountIdToSendFunds, double fundsAmount)
    {
        [JsonPropertyName("accountIdToSendFunds")]
        public Guid AccountIdToSendFunds { get; } = accountIdToSendFunds;
        [JsonPropertyName("fundsAmount")]
        public double FundsAmount { get; } = fundsAmount;
    }
}
