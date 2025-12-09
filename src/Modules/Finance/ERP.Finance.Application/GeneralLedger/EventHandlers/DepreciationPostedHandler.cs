using ERP.Finance.Domain.FixedAssetManagement.Events;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Services;
using ERP.Finance.Domain.Shared.Currency;
using MediatR;
using ERP.Core.Uow;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class DepreciationPostedHandler(
    IJournalEntryRepository journalEntryRepository,
    IUnitOfWorkManager unitOfWork,
    ICurrencyConversionService currencyConverter,
    IFiscalPeriodRepository fiscalPeriodRepository,
    IAccountValidationService accountValidator)
    : INotificationHandler<DepreciationPostedEvent>
{
    private const string SystemBaseCurrency = "USD";

    public async Task Handle(DepreciationPostedEvent notification, CancellationToken cancellationToken)
    {
        var fiscalPeriod = await fiscalPeriodRepository.GetPeriodByDateAsync(notification.PeriodDate, cancellationToken);
        if (fiscalPeriod is null)
        {
            // Log error: Cannot post depreciation as no open fiscal period was found.
            return;
        }

        var entry = new JournalEntry(
            $"Monthly Depreciation for Asset {notification.AssetId}", 
            $"DEP-{notification.PeriodDate.Year}-{notification.PeriodDate.Month}",
            notification.BusinessUnitId // Pass BusinessUnitId
        );
        
        var baseAmount = await currencyConverter.ConvertAsync(
            source: notification.Amount, // The Money object from the event
            targetCurrency: SystemBaseCurrency,
            conversionDate: notification.PeriodDate
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
        entry.Post(fiscalPeriod, accountValidator);

        using var scope = unitOfWork.Begin();

        await journalEntryRepository.AddAsync(entry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
    }
}