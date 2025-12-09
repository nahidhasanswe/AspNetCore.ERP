using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Events;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Services;
using ERP.Finance.Domain.Shared.Currency;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class BadDebtGeneralLedgerPostingHandler(
    IJournalEntryRepository journalEntryRepository,
    IUnitOfWorkManager unitOfWork,
    ICurrencyConversionService currencyConverter,
    IFiscalPeriodRepository fiscalPeriodRepository,
    IAccountValidationService accountValidator
    ) : INotificationHandler<BadDebtWrittenOffEvent>
{
    private const string SystemBaseCurrency = "USD";

    public async Task Handle(BadDebtWrittenOffEvent notification, CancellationToken cancellationToken)
    {
        var writeOffAmount = notification.WriteOffAmount;
        
        // Fetch the fiscal period for the write-off date. This is now required for posting.
        var fiscalPeriod = await fiscalPeriodRepository.GetPeriodByDateAsync(notification.WriteOffDate, cancellationToken);
        if (fiscalPeriod is null)
        {
            // In a real system, you would log this error. An event handler should not throw an exception
            // that stops the process, but this write-off cannot be posted without a valid period.
            return; 
        }
        
        // 1. Create Journal Entry
        var entry = new JournalEntry(
            $"Bad Debt Write-Off for Invoice: {notification.InvoiceId}. Reason: {notification.Reason}", 
            $"{notification.InvoiceId}-WO-{notification.WriteOffDate:yyyyMMdd}",
            notification.BusinessUnitId
        );
        
        // 2. Convert to Base Currency
        // Use the WriteOffDate for the historical exchange rate.
        var baseAmount = await currencyConverter.ConvertAsync(
            source: writeOffAmount, 
            targetCurrency: SystemBaseCurrency,
            conversionDate: notification.WriteOffDate 
        );
        
        // 3. Debit: Bad Debt Expense Account (Recognizes the loss)
        // Entry: DEBIT Bad Debt Expense (Increases Expense, Decreases Equity)
        entry.AddLine(new LedgerLine(
            entry.Id, 
            notification.BadDebtExpenseAccountId, // Expense Account ID from the event payload
            writeOffAmount,
            baseAmount, 
            isDebit: true, 
            description: $"Bad Debt Expense for Invoice {notification.InvoiceId}", 
            notification.CostCenterId
        ));
        
        // 4. Credit: AR Control Account (Clears the asset liability on the GL)
        // Entry: CREDIT AR Control (Decreases Asset)
        entry.AddLine(new LedgerLine(
            entry.Id, 
            notification.ARControlAccountId, // AR Control Account ID from the event payload
            writeOffAmount, 
            baseAmount, 
            isDebit: false, 
            description: $"Clear AR Asset from Write-Off (Invoice {notification.InvoiceId})", 
            notification.CostCenterId
        ));

        // Post the entry using the fetched period and the injected validator.
        entry.Post(fiscalPeriod, accountValidator);

        // 5. Persist the Journal Entry
        using var scope = unitOfWork.Begin();
        
        await journalEntryRepository.AddAsync(entry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
    }
}