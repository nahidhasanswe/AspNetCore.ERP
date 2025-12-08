using ERP.Core;
using ERP.Core.Behaviors;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.ApproveVendor;

public record ApproveVendorInvoiceCommand(Guid InvoiceId, Guid ApproverId) : IRequestCommand<Guid>;