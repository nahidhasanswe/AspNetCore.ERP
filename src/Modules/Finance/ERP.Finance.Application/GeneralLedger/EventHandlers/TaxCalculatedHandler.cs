using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Services;
using ERP.Finance.Domain.Shared.Currency;
using ERP.Finance.Domain.TaxManagement.Events;
using MediatR;
using ERP.Core.Uow;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class TaxCalculatedHandler(
    IJournalEntryRepository journalEntryRepository,
    IUnitOfWorkManager unitOfWork,
    ICurrencyConversionService currencyConverter,
    IFiscalPeriodRepository fiscalPeriodRepository,
    IAccountValidationService accountValidator)
    : INotificationHandler<TaxCalculatedEvent>
{
    private const string SystemBaseCurrency = "USD";

    public async Task Handle(TaxCalculatedEvent notification, CancellationToken cancellationToken)
    {
        var fiscalPeriod = await fiscalPeriodRepository.GetPeriodByDateAsync(notification.TransactionDate, cancellationToken);
        if (fiscalPeriod is null)
        {
            // Log error: Cannot post tax GL as no open fiscal period was found.
            return;
        }

        var taxAmount = notification.TaxAmount;
        var payableAccountId = notification.TaxPayableAccountId;

        var entry = new JournalEntry(
            $"Tax liability from Source: {notification.SourceTransactionId}", 
            notification.Reference,
            notification.BusinessUnitId
        );

        Guid arApControlAccountId = notification.SourceControlAccountId;
        
        var baseTaxAmount = await currencyConverter.ConvertAsync(
            source: taxAmount, 
            targetCurrency: SystemBaseCurrency,
            conversionDate: notification.TransactionDate 
        );
        
        // **GL Posting Logic based on Transaction Type**

        if (notification.IsSalesTransaction)
        {
            // AR Tax: Sales Tax is collected from customer (Liability increase)
            // Debit: Accounts Receivable (Asset increase, recorded with initial invoice event)
            // Credit: Tax Payable (Liability increase)
            
            // This event is usually triggered *after* the initial AR invoice posting.
            // When tax is calculated:
            // 1. Debit the AR Control Account (or the AR account on the invoice)
            // 2. Credit the Tax Payable Account
            
            // For simplicity, we assume the base amount was already posted. This event posts the tax portion.
            
            // 3. Debit: AR Control Account (Tax portion of the customer's total debt)
            entry.AddLine(new LedgerLine(
                entry.Id, 
                arApControlAccountId, 
                taxAmount,
                baseTaxAmount, 
                isDebit: true, 
                description: "Tax portion of Accounts Receivable", 
                notification.CostCenterId
            )); 
            
            // 4. Credit: Tax Payable (Liability increase)
            entry.AddLine(new LedgerLine(
                entry.Id, 
                payableAccountId, 
                taxAmount, 
                baseTaxAmount, 
                isDebit: false, 
                description: "Sales Tax Payable Liability", 
                notification.CostCenterId
            ));
        }
        else // Purchase (AP Tax)
        {
            // AP Tax: Input VAT/Tax Paid (Asset increase)
            // Debit: Tax Recoverable/Expense Account (Asset/Expense increase)
            // Credit: Accounts Payable (Liability increase, recorded with initial AP invoice event)
            
            // Debit: Inventory/Expense Account (Often handled on invoice initial posting)
            // Credit: AP Control Account
            
            // Purchase Tax (AP): DEBIT Tax Recoverable (Asset) | CREDIT AP Control (Liability)
            
            // 3. Debit: Tax Recoverable/Input Tax (Asset increase)
            entry.AddLine(new LedgerLine(
                entry.Id, 
                payableAccountId, 
                taxAmount, 
                baseTaxAmount, 
                isDebit: true, 
                description: "Tax Recoverable/Input Tax Asset", 
                notification.CostCenterId
            ));
            
            // 4. Credit: AP Control Account (Tax portion of the total liability to vendor)
            entry.AddLine(new LedgerLine(
                entry.Id, 
                arApControlAccountId, 
                taxAmount, 
                baseTaxAmount, 
                isDebit: false, 
                description: "Tax portion of Accounts Payable", 
                notification.CostCenterId
            ));
        }

        entry.Post(fiscalPeriod, accountValidator);

        using var scope = unitOfWork.Begin();
        
        await journalEntryRepository.AddAsync(entry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
    }
}