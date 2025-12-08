using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreateVendor;

public class CreateVendorCommand : IRequest<Result<Guid>>
{
    public string Name { get; set; }
    public string TaxId { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhone { get; set; }
    public string PaymentTerms { get; set; }
}