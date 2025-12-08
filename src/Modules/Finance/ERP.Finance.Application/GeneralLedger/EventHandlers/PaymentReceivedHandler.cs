using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Events;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Services;
using ERP.Finance.Domain.Shared.Currency;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class PaymentReceivedHandler(
    IJournalEntryRepository journalEntryRepository,
    IUnitOfWorkManager unitOfWork,
    ICurrencyConversionService currencyConverter,
    IFiscalPeriodRepository fiscalPeriodRepository,
    IAccountValidationService accountValidator
    ) : INotificationHandler<PaymentReceivedEvent>
{
    private const string SystemBaseCurrency = "USD";
    
    public async Task Handle(PaymentReceivedEvent notification, CancellationToken cancellationToken)
    {
        var fiscalPeriod = await fiscalPeriodRepository.GetPeriodByDateAsync(notification.PaymentDate, cancellationToken);
        if (fiscalPeriod is null)
        {
            // Log error: Cannot post payment GL as no open fiscal period was found.
            return;
        }

        var entry = new JournalEntry(
            $"Cash Receipt for Invoice {notification.InvoiceId}", 
            notification.Reference
        );
        
        var baseAmount = await currencyConverter.ConvertAsync(
            source: notification.AmountReceived, // The Money object from the event
            targetCurrency: SystemBaseCurrency,
            conversionDate: notification.PaymentDate // Use the date the payment was recorded
        );
        
        // 2. Debit: Increase Asset (Cash/Bank Account)
        var debitLine = new LedgerLine(
            entry.Id, 
            notification.CashAccountId, 
            notification.AmountReceived, 
            baseAmount, 
            isDebit: true, 
            description: "Cash/Bank Deposit",
            costCenterId: notification.CostCenterId 
        ); 
        
        // 3. Credit: Decrease Asset (Accounts Receivable - Clearing the liability)
        var creditLine = new LedgerLine(
            entry.Id, 
            notification.ArControlAccountId, 
            notification.AmountReceived,
            baseAmount, 
            isDebit: false, 
            description: "AR Balance Reduction",
            costCenterId: notification.CostCenterId
        );
        
        entry.AddLine(debitLine);
        entry.AddLine(creditLine);

        entry.Post(fiscalPeriod, accountValidator); 
        
        using var scope = unitOfWork.Begin();
        
        await journalEntryRepository.AddAsync(entry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
    }
}