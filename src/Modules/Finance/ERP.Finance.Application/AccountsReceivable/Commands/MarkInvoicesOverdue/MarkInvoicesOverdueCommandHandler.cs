using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Domain.Shared.Enums;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Commands.MarkInvoicesOverdue;

public class MarkInvoicesOverdueCommandHandler(
    ICustomerInvoiceRepository invoiceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<MarkInvoicesOverdueCommand, Result>
{
    public async Task<Result> Handle(MarkInvoicesOverdueCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var invoicesToConsider = (await invoiceRepository.ListAllAsync(cancellationToken))
            .Where(inv => (inv.Status == InvoiceStatus.Issued || inv.Status == InvoiceStatus.PartiallyPaid) && inv.DueDate < command.AsOfDate)
            .ToList();

        foreach (var invoice in invoicesToConsider)
        {
            invoice.MarkAsOverdue();
            await invoiceRepository.UpdateAsync(invoice, cancellationToken);
        }

        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}