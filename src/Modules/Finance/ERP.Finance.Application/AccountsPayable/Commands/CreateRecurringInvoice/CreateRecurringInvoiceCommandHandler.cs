using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreateRecurringInvoice;

public class CreateRecurringInvoiceCommandHandler(
    IRecurringInvoiceRepository recurringInvoiceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreateRecurringInvoiceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateRecurringInvoiceCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var lines = command.Lines.Select(dto => new RecurringInvoiceLine
        (
            dto.Description,
            dto.LineAmount,
            dto.ExpenseAccountId,
            dto.CostCenterId
        )).ToList();

        var recurringInvoice = RecurringInvoice.Create(
            command.VendorId,
            command.Interval,
            command.StartDate,
            command.EndDate,
            lines
        );

        await recurringInvoiceRepository.AddAsync(recurringInvoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(recurringInvoice.Id);
    }
}