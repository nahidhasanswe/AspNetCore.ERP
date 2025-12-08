using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.AccountsPayable.Services;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsPayable.Commands.ApproveVendor;

public class ApproveVendorInvoiceCommandHandler(
    IVendorInvoiceRepository invoiceRepository,
    IApprovalService approvalService,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<ApproveVendorInvoiceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(ApproveVendorInvoiceCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var invoice = await invoiceRepository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            return Result.Failure<Guid>("Invoice not found.");
        }

        await invoice.Approve(command.ApproverId, approvalService);

        await invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(invoice.Id);
    }
}