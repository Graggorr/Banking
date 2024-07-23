namespace Banking.Domain
{
    public class Account
    {
        public Guid Id { get; init; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public double MoneyAmount { get; set; }
    }
}
