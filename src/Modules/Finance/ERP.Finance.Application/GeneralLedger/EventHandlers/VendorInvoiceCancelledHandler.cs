using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Events;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Services;
using ERP.Finance.Domain.Shared.Currency;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class VendorInvoiceCancelledHandler(
    IJournalEntryRepository journalEntryRepository,
    IUnitOfWorkManager unitOfWork,
    ICurrencyConversionService currencyConverter,
    IFiscalPeriodRepository fiscalPeriodRepository,
    IAccountValidationService accountValidator
    ) : INotificationHandler<VendorInvoiceCancelledEvent>
{
    private const string SystemBaseCurrency = "USD"; 

    public async Task Handle(VendorInvoiceCancelledEvent notification, CancellationToken cancellationToken)
    {
        var fiscalPeriod = await fiscalPeriodRepository.GetPeriodByDateAsync(notification.CancellationDate, cancellationToken);
        if (fiscalPeriod is null)
        {
            // Log error: Cannot post invoice cancellation GL as no open fiscal period was found.
            return;
        }

        // 1. Create the GL Reversal Aggregate Root
        var entry = new JournalEntry(
            $"Reversal of Cancelled Vendor Invoice {notification.InvoiceId} - Reason: {notification.CancellationReason}", 
            $"CANCEL-{notification.InvoiceId}"
        );
        
        // 2. CRITICAL: Calculate the Base Amount for the entire reversal
        var originalAmount = notification.OriginalTotalAmount;
        var baseAmount = await currencyConverter.ConvertAsync(
            source: originalAmount, 
            targetCurrency: SystemBaseCurrency,
            conversionDate: notification.CancellationDate // Use cancellation date for reversal rate
        );
        
        // 3. Line Creation: Debit Side (Liability Reduction)
        // DEBIT: Accounts Payable Control Account (Liability decreases)
        var debitLine = new LedgerLine(
            entry.Id, 
            notification.OriginalApControlAccountId, 
            originalAmount, 
            baseAmount, 
            isDebit: true, 
            description: "AP Liability Reversal",
            costCenterId: notification.OriginalLineItems.FirstOrDefault()?.CostCenterId // Use a representative CC
        ); 
        entry.AddLine(debitLine);
        
        // 4. Line Creation: Credit Side (Expense/Asset Reduction)
        // CREDIT: Original Expense/Asset Accounts (Original Expense decreases)
        foreach (var line in notification.OriginalLineItems)
        {
            // Calculate base amount for the individual line reversal
            var lineBaseAmount = await currencyConverter.ConvertAsync(
                source: line.LineAmount, 
                targetCurrency: SystemBaseCurrency,
                conversionDate: notification.CancellationDate
            );

            entry.AddLine(new LedgerLine(
                entry.Id, 
                line.ExpenseAccountId, 
                line.LineAmount, 
                lineBaseAmount, 
                isDebit: false, 
                description: $"Reversal: {line.Description}", 
                line.CostCenterId
            )); 
        }

        // 5. Finalize and Post
        entry.Post(fiscalPeriod, accountValidator);

        using var scope = unitOfWork.Begin();
        await journalEntryRepository.AddAsync(entry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
    }
}