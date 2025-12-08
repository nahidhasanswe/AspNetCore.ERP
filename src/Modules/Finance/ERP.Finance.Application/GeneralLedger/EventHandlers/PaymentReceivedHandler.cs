using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Events;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.Shared.Currency;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class PaymentReceivedHandler(
    IJournalEntryRepository glRepository,
    IUnitOfWorkManager unitOfWork,
    ICurrencyConversionService currencyConverter
    ) : INotificationHandler<PaymentReceivedEvent>
{
    private const string SystemBaseCurrency = "USD";
    
    public async Task Handle(PaymentReceivedEvent notification, CancellationToken cancellationToken)
    {
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

        entry.Post(); 
        
        using var scope = unitOfWork.Begin();
        
        await glRepository.AddAsync(entry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
    }
}