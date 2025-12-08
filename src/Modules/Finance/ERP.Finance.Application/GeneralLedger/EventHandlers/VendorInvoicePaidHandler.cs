using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Events;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.Shared.Currency;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class VendorInvoicePaidHandler(
        IJournalEntryRepository glRepository,
        IUnitOfWorkManager unitOfWork,
        ICurrencyConversionService currencyConverter
    ) : INotificationHandler<VendorPaymentRecordedEvent>
{
    private const string SystemBaseCurrency = "USD";
    
    public async Task Handle(VendorPaymentRecordedEvent notification, CancellationToken cancellationToken)
    {
        // 1. Create the GL Aggregate Root
        var entry = new JournalEntry(
            $"Payment for Vendor Invoice {notification.InvoiceId}", 
            notification.TransactionReference
        );
        
        Guid cashAccountId = notification.PaymentAccountId; 
        
        Money amount = notification.AmountPaid;
        
        var baseAmount = await currencyConverter.ConvertAsync(
            source: amount, 
            targetCurrency: SystemBaseCurrency,
            conversionDate: notification.OccurredOn // Assumed new property on event
        );
        
        // Action: Debit AP Control Account (Liability Reduction)
        var debitLine = new LedgerLine(
            entry.Id, 
            notification.APControlAccountId, 
            amount, 
            baseAmount, 
            isDebit: true, 
            description: "AP Liability Reduction",
            notification.CostCenterId
        );
        
        // Action: Credit Cash/Bank Account (Asset Reduction)
        var creditLine = new LedgerLine(
            entry.Id, 
            cashAccountId, 
            amount, 
            baseAmount, 
            isDebit: false, 
            description: "Cash/Bank Disbursement",
            notification.CostCenterId
        );
        
        entry.AddLine(debitLine);
        entry.AddLine(creditLine);

        // 5. Post the entry, ensuring the balance invariant is maintained
        entry.Post();

        using var scope = unitOfWork.Begin();
        
        // IMPORTANT: The LedgerLine constructor now requires BaseAmount for currency consistency,
        // so you must inject ICurrencyConversionService here and calculate the BaseAmount
        // before adding LedgerLines, if the payment is in a foreign currency. 
        // For simplicity, we omit that complexity here.

        await glRepository.AddAsync(entry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
    }
}