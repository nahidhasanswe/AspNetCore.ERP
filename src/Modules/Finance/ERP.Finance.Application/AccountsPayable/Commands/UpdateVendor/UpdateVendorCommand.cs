using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Finance.Domain.Shared.DTOs;
using MediatR;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.UpdateVendor;

public class UpdateVendorCommand : IRequestCommand<Unit>
{
    public Guid VendorId { get; set; }
    public string Name { get; set; }
    public string TaxId { get; set; }
    public AddressDto Address { get; set; }
    public ContactInfoDto ContactInfo { get; set; }
    public string PaymentTerms { get; set; }
    public string DefaultCurrency { get; set; }
    public VendorBankDetailsDto BankDetails { get; set; }
    public bool IsActive { get; set; }
}