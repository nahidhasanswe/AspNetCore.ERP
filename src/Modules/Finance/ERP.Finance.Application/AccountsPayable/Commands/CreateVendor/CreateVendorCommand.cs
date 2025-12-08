using ERP.Core.Behaviors;
using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Finance.Domain.Shared.DTOs;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreateVendor;

public class CreateVendorCommand : IRequestCommand<Guid>
{
    public string Name { get; set; }
    public string TaxId { get; set; }
    public AddressDto Address { get; set; }
    public ContactInfoDto ContactInfo { get; set; }
    public string PaymentTerms { get; set; }
    public string DefaultCurrency { get; set; }
    public VendorBankDetailsDto BankDetails { get; set; }
}