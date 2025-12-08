using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Events;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Services;
using ERP.Finance.Domain.Shared.Currency;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class DeductionAppliedGlPostingHandler(
    IJournalEntryRepository journalEntryRepository,
    IUnitOfWorkManager unitOfWork,
    ICurrencyConversionService currencyConverter,
    IFiscalPeriodRepository fiscalPeriodRepository,
    IAccountValidationService accountValidator
    ) : INotificationHandler<DeductionAppliedToInvoiceEvent>
{
    private const string SystemBaseCurrency = "USD";

    public async Task Handle(DeductionAppliedToInvoiceEvent notification, CancellationToken cancellationToken)
    {
        var deductionAmount = notification.DeductionAmount;

        // Fetch the fiscal period for the deduction date. This is now required for posting.
        var fiscalPeriod = await fiscalPeriodRepository.GetPeriodByDateAsync(notification.OccurredOn, cancellationToken);
        if (fiscalPeriod is null)
        {
            // In a real system, you would log this error. An event handler should not throw an exception
            // that stops the process, but this deduction cannot be posted without a valid period.
            // Consider moving this to a dead-letter queue for later processing.
            return; 
        }
        
        var entry = new JournalEntry(
            $"Deduction Applied to Invoice: {notification.InvoiceId}. Reason Code: {notification.DeductionReasonCode}", 
            $"{notification.InvoiceId}-{notification.DeductionReasonCode}-{notification.OccurredOn:yyyyMMdd}"
        );

        var baseAmount = await currencyConverter.ConvertAsync(
            source: deductionAmount, 
            targetCurrency: SystemBaseCurrency,
            conversionDate: notification.OccurredOn 
        );
        
        // Entry: DEBIT Expense (Increase in Expense, Decrease in Equity)
        entry.AddLine(new LedgerLine(
            entry.Id, 
            notification.DeductionExpenseAccountId, // Specific GL Expense Account ID
            deductionAmount,
            baseAmount, 
            isDebit: true, 
            description: $"Deduction ({notification.DeductionReasonCode}) for Invoice {notification.InvoiceId}", 
            costCenterId: null // Cost center determined by GL configuration
        ));
        
        // Entry: CREDIT AR Control (Decrease in Asset)
        entry.AddLine(new LedgerLine(
            entry.Id, 
            notification.ARControlAccountId, // AR Control Account ID
            deductionAmount, 
            baseAmount, 
            isDebit: false, 
            description: $"Clear AR balance via deduction for Invoice {notification.InvoiceId}", 
            costCenterId: null
        ));
        
        // Post the entry using the fetched period and the injected validator.
        entry.Post(fiscalPeriod, accountValidator);

        // Persist the Journal Entry
        using var scope = unitOfWork.Begin();
        
        await journalEntryRepository.AddAsync(entry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
    }
}