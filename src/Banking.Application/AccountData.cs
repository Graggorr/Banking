namespace Banking.Application
{
    public record class AccountData(Guid Id, string Name, double Money, string PhoneNumber);
}