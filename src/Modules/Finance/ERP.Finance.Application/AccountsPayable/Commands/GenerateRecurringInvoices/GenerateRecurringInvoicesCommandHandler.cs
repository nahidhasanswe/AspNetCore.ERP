using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.GenerateRecurringInvoices;

public class GenerateRecurringInvoicesCommandHandler(
    IRecurringInvoiceRepository recurringInvoiceRepository,
    IVendorInvoiceRepository vendorInvoiceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<GenerateRecurringInvoicesCommand, Unit>
{
    public async Task<Result<Unit>> Handle(GenerateRecurringInvoicesCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var today = DateTime.UtcNow.Date;
        var recurringInvoicesDue = await recurringInvoiceRepository.GetActiveRecurringInvoicesDueForGenerationAsync(today);

        foreach (var recurringInvoice in recurringInvoicesDue)
        {
            var newInvoice = recurringInvoice.GenerateInvoice(today);
            if (newInvoice != null)
            {
                await vendorInvoiceRepository.AddAsync(newInvoice, cancellationToken);
                await recurringInvoiceRepository.UpdateAsync(recurringInvoice, cancellationToken); // Update next occurrence date
            }
        }

        await scope.SaveChangesAsync(cancellationToken);
        return Result.Success(Unit.Value);
    }
}