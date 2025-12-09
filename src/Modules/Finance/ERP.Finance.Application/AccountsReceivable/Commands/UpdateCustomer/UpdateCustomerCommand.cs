using ERP.Core;
using ERP.Finance.Domain.Shared.DTOs;
using MediatR;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.AccountsReceivable.Commands.UpdateCustomer;

public class UpdateCustomerCommand : IRequest<Result>
{
    public Guid CustomerId { get; set; }
    public string Name { get; set; }
    public string ContactEmail { get; set; }
    public AddressDto BillingAddress { get; set; }
    public ContactInfoDto ContactInfo { get; set; }
    public string PaymentTerms { get; set; }
    public string DefaultCurrency { get; set; }
    public Guid ARControlAccountId { get; set; }
    public Money ApprovedCreditLimit { get; set; } // Added for CustomerCreditProfile
}