using ERP.Core.Uow;
using ERP.Finance.Domain.Encumbrance.Aggregates;
using ERP.Finance.Domain.Encumbrance.Events;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Service;
using ERP.Finance.Domain.GeneralLedger.Services;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class EncumbranceCommitmentGlPostingHandler(
    IJournalEntryRepository journalEntryRepository,
    IUnitOfWorkManager unitOfWork,
    IGlConfigurationService glConfig,
    IEncumbranceRepository encumbranceRepository, // To fetch the original reserved amount
    IFiscalPeriodRepository fiscalPeriodRepository,
    IAccountValidationService accountValidator
    ) : INotificationHandler<EncumbranceConvertedToCommitmentEvent>
{
    public async Task Handle(EncumbranceConvertedToCommitmentEvent notification, CancellationToken cancellationToken)
    {
        var fiscalPeriod = await fiscalPeriodRepository.GetPeriodByDateAsync(notification.OccurredOn, cancellationToken);
        if (fiscalPeriod is null)
        {
            // Log error: Cannot post encumbrance commitment as no open fiscal period was found.
            return;
        }

        // 1. Fetch the original Encumbrance record to find the initial reserved amount.
        var originalEncumbrance = await encumbranceRepository.GetByIdAsync(notification.EncumbranceId);
        if (originalEncumbrance == null) return; 

        // CRITICAL: Calculate the difference between the final committed amount and the original reserved amount.
        decimal originalAmount = originalEncumbrance.Amount.Amount; // Assuming Money is tracked in the repository
        decimal finalAmount = notification.FinalCommittedAmount.Amount;
        decimal adjustmentAmount = finalAmount - originalAmount;
        string currency = notification.FinalCommittedAmount.Currency;

        // If adjustmentAmount is 0, no GL entry is needed for the difference.
        if (adjustmentAmount == 0) return;

        // 2. Resolve the GL Accounts
        var encumbrancePostingData = await glConfig.GetEncumbranceGlAccounts(notification.GlAccountId, cancellationToken);
        if (encumbrancePostingData == null) return; 

        var entry = new JournalEntry(
            $"Encumbrance Adjustment/Reclassification: {notification.EncumbranceId}", 
            notification.EncumbranceId.ToString(),
            notification.BusinessUnitId
        );

        // Determine if it's a net increase (Debit) or net decrease (Credit)
        bool isIncrease = adjustmentAmount > 0;
        Money finalAdjustment = new Money(Math.Abs(adjustmentAmount), currency);
        
        // 3. Post Adjustment Entry

        // A. DEBIT/CREDIT: Encumbrance Account
        // If adjustmentAmount > 0 (Increase): DEBIT Encumbrance Account
        // If adjustmentAmount < 0 (Decrease): CREDIT Encumbrance Account
        entry.AddLine(new LedgerLine(
            entry.Id, 
            encumbrancePostingData.EncumbranceAccountId, 
            finalAdjustment,
            finalAdjustment, // Base amount (simplified)
            isDebit: isIncrease, 
            description: $"Adjustment to firm commitment for {notification.EncumbranceId} (DR/CR Commitment)", 
            notification.CostCenterId
        ));
        
        // B. CREDIT/DEBIT: Budgetary Appropriation Account (Balances the entry)
        // If adjustmentAmount > 0 (Increase): CREDIT Appropriation Account
        // If adjustmentAmount < 0 (Decrease): DEBIT Appropriation Account
        entry.AddLine(new LedgerLine(
            entry.Id, 
            encumbrancePostingData.AppropriationAccountId, 
            finalAdjustment, 
            finalAdjustment, // Base amount (simplified)
            isDebit: !isIncrease, 
            description: $"Adjustment to firm commitment for {notification.EncumbranceId} (CR/DR Appropriation)", 
            notification.CostCenterId
        ));

        // Visualize the adjustment entry logic: 

        entry.Post(fiscalPeriod, accountValidator);

        using var scope = unitOfWork.Begin();
        
        await journalEntryRepository.AddAsync(entry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
    }
}