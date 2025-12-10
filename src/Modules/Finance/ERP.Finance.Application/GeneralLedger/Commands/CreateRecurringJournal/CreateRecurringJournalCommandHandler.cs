using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.Shared.Currency;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.GeneralLedger.Commands.CreateRecurringJournal;

public class CreateRecurringJournalCommandHandler : IRequestCommandHandler<CreateRecurringJournalCommand, Guid>
{
    private readonly IRecurringJournalEntryRepository _recurringJournalRepository;
    private readonly IUnitOfWorkManager _unitOfWork;
    private readonly ICurrencyConversionService _currencyConverter;

    private const string SystemBaseCurrency = "USD";

    public CreateRecurringJournalCommandHandler(
        IRecurringJournalEntryRepository recurringJournalRepository,
        IUnitOfWorkManager unitOfWork,
        ICurrencyConversionService currencyConverter)
    {
        _recurringJournalRepository = recurringJournalRepository;
        _unitOfWork = unitOfWork;
        _currencyConverter = currencyConverter;
    }

    public async Task<Result<Guid>> Handle(CreateRecurringJournalCommand command, CancellationToken cancellationToken)
    {
        using var scope = _unitOfWork.Begin();

        var recurringEntry = new RecurringJournalEntry(
            command.BusinessUnitId,
            command.Description,
            command.ReferenceNumber,
            command.StartDate,
            command.EndDate,
            command.Frequency
        );

        foreach (var lineDto in command.Lines)
        {
            var transactionAmount = new Money(lineDto.Amount, lineDto.Currency);

            var baseAmount = await _currencyConverter.ConvertAsync(
                source: transactionAmount,
                targetCurrency: SystemBaseCurrency,
                conversionDate: DateTime.UtcNow // Use current date for rate
            );

            var line = new LedgerLine(
                command.BusinessUnitId, // Pass BusinessUnitId to LedgerLine
                recurringEntry.Id, // Use recurring entry ID as JournalEntryId for now
                lineDto.AccountId,
                transactionAmount,
                baseAmount,
                lineDto.IsDebit,
                command.Description, // Assuming lineDto has a description
                lineDto.CostCenterId
            );
            recurringEntry.AddLine(line);
        }

        await _recurringJournalRepository.AddAsync(recurringEntry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(recurringEntry.Id);
    }
}