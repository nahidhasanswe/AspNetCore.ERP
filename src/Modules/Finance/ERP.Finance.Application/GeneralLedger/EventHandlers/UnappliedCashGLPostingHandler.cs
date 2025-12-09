using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Events;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Service;
using ERP.Finance.Domain.GeneralLedger.Services;
using ERP.Finance.Domain.Shared.Currency;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class UnappliedCashGlPostingHandler(
    IJournalEntryRepository journalEntryRepository,
    IUnitOfWorkManager unitOfWork,
    ICurrencyConversionService currencyConverter,
    IGlConfigurationService glConfig, // Service to get the Unapplied Cash/Clearing GL Account ID
    IFiscalPeriodRepository fiscalPeriodRepository,
    IAccountValidationService accountValidator
    ) : INotificationHandler<UnappliedCashCreatedEvent>
{
    private const string SystemBaseCurrency = "USD";

    public async Task Handle(UnappliedCashCreatedEvent notification, CancellationToken cancellationToken)
    {
        var fiscalPeriod = await fiscalPeriodRepository.GetPeriodByDateAsync(notification.OccurredOn, cancellationToken);
        if (fiscalPeriod is null)
        {
            // Log error: Cannot post unapplied cash GL as no open fiscal period was found.
            return;
        }

        var receivedAmount = notification.TotalReceivedAmount;
        
        // Create Journal Entry
        var entry = new JournalEntry(
            $"Cash Received (Unapplied) from Customer {notification.CustomerId}", 
            notification.TransactionReference,
            notification.BusinessUnitId
        );
        
        // Resolve Unapplied Cash GL Account ID
        Guid unappliedCashLiabilityId = await glConfig.GetUnappliedCashClearingAccountId(receivedAmount.Currency);

        // Convert to Base Currency
        var baseAmount = await currencyConverter.ConvertAsync(
            source: receivedAmount, 
            targetCurrency: SystemBaseCurrency,
            conversionDate: notification.OccurredOn // Use the event date for FX rate
        );
        
        // DEBIT: Cash/Bank Account (Asset Increase)
        entry.AddLine(new LedgerLine(
            entry.Id, 
            notification.CashAccountId, // Specific GL bank account ID (DEBITED)
            receivedAmount,
            baseAmount, 
            isDebit: true, 
            description: "Cash received - deposit in transit", 
            costCenterId: null
        )); 
        
        // CREDIT: Unapplied Cash/Cash Clearing Account (Liability/Contra-Asset Increase)
        entry.AddLine(new LedgerLine(
            entry.Id, 
            unappliedCashLiabilityId, // The GL account that holds unapplied cash (CREDITED)
            receivedAmount, 
            baseAmount, 
            isDebit: false, 
            description: "Cash placed in unapplied status awaiting invoice matching", 
            costCenterId: null
        ));

        // Visualize the entry: 
        
        entry.Post(fiscalPeriod, accountValidator);

        // Persist the Journal Entry
        using var scope = unitOfWork.Begin();
        
        await journalEntryRepository.AddAsync(entry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
    }
}