using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.FiscalYear.Service;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.Shared.Currency;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;

public class CreateJournalEntryCommandHandler(
        IJournalEntryRepository repository,
        IUnitOfWorkManager unitOfWork,
        ICurrencyConversionService currencyConverter, 
        IFiscalPeriodCheckService periodChecker
    ) : IRequestHandler<CreateJournalEntryCommand, Result<Guid>>
{
    
    private const string SystemBaseCurrency = "USD";

    public async Task<Result<Guid>> Handle(CreateJournalEntryCommand command, CancellationToken cancellationToken)
    {
        // CRITICAL 1: Check Fiscal Period BEFORE creating the entry (Fail fast)
        await periodChecker.EnsurePeriodIsOpenForPosting(command.PostingDate); // Assuming command has PostingDate

        // 1. Create the Aggregate Root
        var entry = new JournalEntry(command.Description, command.ReferenceNumber);
    
        // 2. Map DTOs to Domain Objects and add to the Aggregate
        foreach (var lineDto in command.Lines)
        {
            var transactionAmount = new Money(lineDto.Amount, lineDto.Currency); 

            // CRITICAL 2: Calculate Base Amount using the injected service
            var baseAmount = await currencyConverter.ConvertAsync(
                source: transactionAmount,
                targetCurrency: SystemBaseCurrency,
                conversionDate: command.PostingDate // Use the transaction date for rate
            );

            // CRITICAL 3: Incorporate ALL new LedgerLine constructor parameters
            var line = new LedgerLine(
                // 1. JournalEntryId: Set here (essential for EF Core mapping)
                entry.Id, 
                // 2. AccountId, Amount, IsDebit, Description (existing)
                lineDto.AccountId, 
                transactionAmount, 
                // 3. BaseAmount (NEW)
                baseAmount, 
                lineDto.IsDebit, 
                command.Description, // Can use lineDto.Description if provided
                // 4. CostCenterId (NEW)
                lineDto.CostCenterId 
            );
        
            entry.AddLine(line);
        }

        // 3. Execute the Core Domain Logic (Invariants Check)
        entry.Post(); // This checks that Debits == Credits for both Amount and BaseAmount.

        using var scope = unitOfWork.Begin();

        // 4. Persist the Aggregate State
        await repository.AddAsync(entry, cancellationToken);

        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(entry.Id);
    }
}