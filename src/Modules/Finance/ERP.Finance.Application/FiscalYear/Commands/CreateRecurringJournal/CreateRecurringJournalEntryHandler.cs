using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using MediatR;

namespace ERP.Finance.Application.FiscalYear.Commands.CreateRecurringJournal;

public class CreateRecurringJournalEntryCommandHandler(
    IRecurringJournalEntryRepository repository,
    IUnitOfWorkManager unitOfWork
) : IRequestHandler<CreateRecurringJournalEntryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRecurringJournalEntryCommand command, CancellationToken cancellationToken)
    {
        var entry = new RecurringJournalEntry(
            command.Description,
            command.ReferenceNumber,
            command.StartDate,
            command.EndDate,
            command.Frequency
        );

        using var scope = unitOfWork.Begin();
        await repository.AddAsync(entry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(entry.Id);
    }
}