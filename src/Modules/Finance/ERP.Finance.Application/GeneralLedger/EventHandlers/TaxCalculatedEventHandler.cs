using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.TaxManagement.Events;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class TaxCalculatedEventHandler(IMediator mediator) : INotificationHandler<TaxCalculatedEvent>
{
    public async Task Handle(TaxCalculatedEvent notification, CancellationToken cancellationToken)
    {
        // GL Entry for Tax Calculation:
        // If Sales Tax (IsSalesTransaction = true):
        //   Debit: Source Control Account (e.g., AR Control) - if tax is part of AR
        //   Credit: Tax Payable Account
        // If Purchase Tax (IsSalesTransaction = false):
        //   Debit: Expense/Asset Account (e.g., Inventory, Expense) - if tax is part of cost
        //   Credit: Source Control Account (e.g., AP Control) - if tax is part of AP

        var journalEntries = new List<CreateJournalEntryCommand.LedgerLineDto>();

        if (notification.IsSalesTransaction)
        {
            // Sales Tax: Debit AR Control, Credit Tax Payable
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.SourceControlAccountId, // e.g., AR Control Account
                Amount = notification.TaxAmount.Amount,
                IsDebit = true,
                Currency = notification.TaxAmount.Currency,
                CostCenterId = notification.CostCenterId
            });
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.TaxPayableAccountId,
                Amount = notification.TaxAmount.Amount,
                IsDebit = false,
                Currency = notification.TaxAmount.Currency,
                CostCenterId = notification.CostCenterId
            });
        }
        else
        {
            // Purchase Tax: Debit Expense/Asset, Credit AP Control
            // This assumes the BaseAmount already includes the tax or the tax is an additional expense.
            // For simplicity, we'll debit a generic expense/asset account for the tax amount.
            // In a real system, the expense account would be derived from the source transaction's line items.
            Guid taxExpenseAssetAccountId = Guid.NewGuid(); // Placeholder for tax expense/asset account

            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = taxExpenseAssetAccountId, // e.g., Inventory, Expense Account
                Amount = notification.TaxAmount.Amount,
                IsDebit = true,
                Currency = notification.TaxAmount.Currency,
                CostCenterId = notification.CostCenterId
            });
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.SourceControlAccountId, // e.g., AP Control Account
                Amount = notification.TaxAmount.Amount,
                IsDebit = false,
                Currency = notification.TaxAmount.Currency,
                CostCenterId = notification.CostCenterId
            });
        }

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.TransactionDate,
            Description = $"Journal entry for tax calculation (ID: {notification.TaxTransactionId}) for source transaction {notification.SourceTransactionId}",
            Lines = journalEntries
        };

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}