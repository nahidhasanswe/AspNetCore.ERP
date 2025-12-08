using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.ApproveVendor;

public class ApproveVendorInvoiceCommand : IRequest<Result<Guid>>
{
    public Guid InvoiceId { get; set; }
    public Guid ApproverId { get; set; }
}