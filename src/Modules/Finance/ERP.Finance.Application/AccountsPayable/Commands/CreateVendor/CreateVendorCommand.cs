using ERP.Core.Behaviors;
using ERP.Finance.Domain.AccountsPayable.ValueObjects;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreateVendor;

public class CreateVendorCommand : IRequestCommand<Guid>
{
    public string Name { get; set; }
    public string TaxId { get; set; }
    public Address Address { get; set; }
    public ContactInfo ContactInfo { get; set; }
    public string PaymentTerms { get; set; }
    public string DefaultCurrency { get; set; }
    public VendorBankDetails BankDetails { get; set; }
}