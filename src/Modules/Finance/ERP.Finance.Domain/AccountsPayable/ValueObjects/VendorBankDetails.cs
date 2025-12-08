using ERP.Core.ValueObjects;

namespace ERP.Finance.Domain.AccountsPayable.ValueObjects;

public class VendorBankDetails : ValueObject
{
    public string AccountNumber { get; init; }
    public string RoutingNumber { get; init; }
    public string BankName { get; init; }
    public string AccountName { get; init; }

    // Private constructor for EF Core
    private VendorBankDetails() { }

    public VendorBankDetails(string accountNumber, string routingNumber, string bankName, string accountName)
    {
        if (string.IsNullOrWhiteSpace(accountNumber)) throw new ArgumentException("Account number cannot be empty.", nameof(accountNumber));
        if (string.IsNullOrWhiteSpace(bankName)) throw new ArgumentException("Bank name cannot be empty.", nameof(bankName));
        if (string.IsNullOrWhiteSpace(accountName)) throw new ArgumentException("Account name cannot be empty.", nameof(accountName));

        AccountNumber = accountNumber;
        RoutingNumber = routingNumber;
        BankName = bankName;
        AccountName = accountName;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AccountNumber;
        yield return RoutingNumber;
        yield return BankName;
        yield return AccountName;
    }
}