using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Events;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.Shared.Currency;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class DepreciationPostedHandler(
    IJournalEntryRepository glRepository,
    IUnitOfWorkManager unitOfWork,
    ICurrencyConversionService currencyConverter
    ) : INotificationHandler<DepreciationPostedEvent>
{
    private const string SystemBaseCurrency = "USD";
    
    public async Task Handle(DepreciationPostedEvent notification, CancellationToken cancellationToken)
    {
        var entry = new JournalEntry(
            $"Monthly Depreciation for Asset {notification.AssetId}", 
            $"DEP-{notification.PeriodDate.Year}-{notification.PeriodDate.Month}"
        );
        
        var baseAmount = await currencyConverter.ConvertAsync(
            source: notification.Amount, // The Money object from the event
            targetCurrency: SystemBaseCurrency,
            conversionDate: notification.OccurredOn
        );
        
        // Debit: Increase Expense (Depreciation Expense)
        var debitLine = new LedgerLine(
            entry.Id, 
            notification.DepreciationExpenseAccountId, 
            notification.Amount, 
            baseAmount, 
            isDebit: true, 
            description: "Monthly Depreciation Expense",
            notification.CostCenterId
        );
        
        // Credit: Increase Contra-Asset (Accumulated Depreciation)
        var creditLine = new LedgerLine(
            entry.Id, 
            notification.AccumulatedDepreciationAccountId, 
            notification.Amount, 
            baseAmount, 
            isDebit: false, 
            description: "Monthly Accumulated Depreciation",
            notification.CostCenterId
        );
        
        entry.AddLine(debitLine);
        entry.AddLine(creditLine);

        // Core Invariant Check
        entry.Post();

        using var scope = unitOfWork.Begin();

        await glRepository.AddAsync(entry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
    }
}