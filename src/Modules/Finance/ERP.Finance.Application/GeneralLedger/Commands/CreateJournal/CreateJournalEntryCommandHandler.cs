using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Services;
using ERP.Finance.Domain.Shared.Currency;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;

public class CreateJournalEntryCommandHandler(
        IJournalEntryRepository repository,
        IUnitOfWorkManager unitOfWork,
        ICurrencyConversionService currencyConverter,
        IFiscalPeriodRepository fiscalPeriodRepository,
        IAccountValidationService accountValidator
    ) : IRequestHandler<CreateJournalEntryCommand, Result<Guid>>
{
    
    private const string SystemBaseCurrency = "USD";

    public async Task<Result<Guid>> Handle(CreateJournalEntryCommand command, CancellationToken cancellationToken)
    {
        // Check Fiscal Period BEFORE creating the entry (Fail fast)
        // We now fetch the period to pass it to the Post method.
        var fiscalPeriod = await fiscalPeriodRepository.GetPeriodByDateAsync(command.PostingDate, cancellationToken);

        if (fiscalPeriod is null)
            return Result.Failure<Guid>($"No fiscal period found for date {command.PostingDate.ToShortDateString()}. The Post method will validate if it is open.");
        
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

            // Incorporate ALL new LedgerLine constructor parameters
            var line = new LedgerLine(
                entry.Id, 
                lineDto.AccountId, 
                transactionAmount, 
                baseAmount, 
                lineDto.IsDebit, 
                command.Description, // Prefer line-specific description, fallback to header
                lineDto.CostCenterId 
            );
        
            entry.AddLine(line);
        }

        // Execute the Core Domain Logic (Invariants Check)
        // The Post method now requires the fiscal period and an account validator.
        entry.Post(fiscalPeriod, accountValidator); 

        using var scope = unitOfWork.Begin();

        // Persist the Aggregate State
        await repository.AddAsync(entry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(entry.Id);
    }
}