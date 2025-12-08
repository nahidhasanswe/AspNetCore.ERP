using ERP.Core.Uow;
using ERP.Finance.Domain.Encumbrance.Events;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Service;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class EncumbranceGLPostingHandler(
    IJournalEntryRepository glRepository,
    IUnitOfWorkManager unitOfWork,
    IGLConfigurationService glConfig // Service to map GlAccountId to Encumbrance Accounts
    ) : INotificationHandler<EncumbranceCreatedEvent>
{
    private const string SystemBaseCurrency = "USD";

    public async Task Handle(EncumbranceCreatedEvent notification, CancellationToken cancellationToken)
    {
        var amount = notification.Amount;
        
        // 1. Resolve the specific Encumbrance and Appropriation GL Accounts
        // These are non-standard GL accounts used solely for budgetary tracking.
        var encumbrancePostingData = await glConfig.GetEncumbranceGLAccounts(notification.GlAccountId, cancellationToken);
        
        if (encumbrancePostingData is null)
        {
            // Log warning: Cannot post encumbrance GL entry because accounts are not configured.
            return;
        }

        var entry = new JournalEntry(
            $"Encumbrance created for Source: {notification.SourceTransactionId} ({notification.Type})", 
            notification.SourceTransactionId.ToString()
        );
        
        // Note: For simplicity, currency conversion is omitted here, assuming encumbrance is posted in local currency.
        // In a real system, a base currency equivalent is always calculated.

        // DEBIT: Encumbrance Account (Increase in recorded commitment)
        entry.AddLine(new LedgerLine(
            entry.Id, 
            encumbrancePostingData.EncumbranceAccountId, // e.g., 5500 - Encumbrance DR
            amount,
            amount, // Base amount (simplified)
            isDebit: true, 
            description: $"Encumbrance DR for {notification.SourceTransactionId}", 
            notification.CostCenterId 
        ));
        
        // CREDIT: Budgetary Appropriation Account (Decrease in available budget)
        entry.AddLine(new LedgerLine(
            entry.Id, 
            encumbrancePostingData.AppropriationAccountId, // e.g., 5501 - Appropriation CR
            amount, 
            amount, // Base amount (simplified)
            isDebit: false, 
            description: $"Appropriation CR for {notification.SourceTransactionId}", 
            notification.CostCenterId
        ));


        entry.Post();

        // 4. Persist
        using var scope = unitOfWork.Begin();
        await glRepository.AddAsync(entry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
    }
}