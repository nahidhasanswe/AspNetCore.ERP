using ERP.Core;
using ERP.Finance.Domain.Shared.DTOs;
using MediatR;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CreateCustomer;

public class CreateCustomerCommand : IRequest<Result<Guid>>
{
    public Guid BusinessUnitId { get; set; } // New property
    public string Name { get; set; }
    public string ContactEmail { get; set; }
    public AddressDto BillingAddress { get; set; }
    public ContactInfoDto ContactInfo { get; set; }
    public string PaymentTerms { get; set; } // e.g., Net 30, Net 60
    public string DefaultCurrency { get; set; }
    public Guid ARControlAccountId { get; set; } // Default AR Control GL Account
    public Money ApprovedCreditLimit { get; set; } // For CustomerCreditProfile
}