using ERP.Core.Uow;
using ERP.Finance.Domain.Events;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Services;
using ERP.Finance.Domain.Shared.Currency;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class InvoiceIssuedHandler(
    IJournalEntryRepository journalEntryRepository,
    IUnitOfWorkManager unitOfWork,
    ICurrencyConversionService currencyConverter,
    IFiscalPeriodRepository fiscalPeriodRepository,
    IAccountValidationService accountValidator
    ) : INotificationHandler<InvoiceIssuedEvent>
{

    private const string SystemBaseCurrency = "USD";
    
    public async Task Handle(InvoiceIssuedEvent notification, CancellationToken cancellationToken)
    {
        var fiscalPeriod = await fiscalPeriodRepository.GetPeriodByDateAsync(notification.OccurredOn, cancellationToken);
        if (fiscalPeriod is null)
        {
            // Log error: Cannot post invoice GL as no open fiscal period was found.
            return;
        }

        var entry = new JournalEntry(
            $"Accrual for Customer Invoice {notification.InvoiceId}", 
            notification.InvoiceId.ToString(),
            notification.BusinessUnitId
        );
        
        var baseAmount = await currencyConverter.ConvertAsync(
            source: notification.Amount, // The Money object from the event
            targetCurrency: SystemBaseCurrency,
            conversionDate: notification.OccurredOn // Use the invoice date for the rate
        );
        
        // Debit: Increase Asset (Accounts Receivable)
        var debitLine = new LedgerLine(
            entry.Id, 
            notification.ARControlAccountId, 
            notification.Amount, 
            baseAmount, 
            isDebit: true, 
            description: "Accounts Receivable Control",
            costCenterId: notification.CostCenterId 
        );
        
        entry.AddLine(debitLine);
        
        // 2. CREDIT: Revenue Accounts (Equity Increase)
        foreach (var line in notification.LineItems)
        {
            var baseLineAmount = await currencyConverter.ConvertAsync(
                source: line.LineAmount, 
                targetCurrency: SystemBaseCurrency,
                conversionDate: notification.OccurredOn 
            );

            entry.AddLine(new LedgerLine(
                entry.Id, 
                line.RevenueAccountId, // Specific Revenue GL Account ID to CREDIT
                line.LineAmount, baseLineAmount, isDebit: false, 
                description: $"Revenue for item: {line.Description}", 
                line.CostCenterId
            ));
        }

        entry.Post(fiscalPeriod, accountValidator);

        using var scope = unitOfWork.Begin();
        
        await journalEntryRepository.AddAsync(entry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
    }
}