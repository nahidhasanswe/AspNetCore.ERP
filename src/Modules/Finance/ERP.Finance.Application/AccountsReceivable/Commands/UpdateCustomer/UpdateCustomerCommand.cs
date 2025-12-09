using ERP.Core;
using MediatR;
using ERP.Finance.Domain.Shared.DTOs;

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
}