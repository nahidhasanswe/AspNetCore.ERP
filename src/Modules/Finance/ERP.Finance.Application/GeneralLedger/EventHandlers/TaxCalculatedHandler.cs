using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.TaxManagement.Events;
using MediatR;
namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class TaxCalculatedHandler(IMediator mediator) : INotificationHandler<TaxCalculatedEvent>
{
    public async Task Handle(TaxCalculatedEvent notification, CancellationToken cancellationToken)
    {
        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.TransactionDate,
            Description = $"Journal entry for tax calculation (ID: {notification.TaxTransactionId}) for source transaction {notification.SourceTransactionId}",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = new List<CreateJournalEntryCommand.LedgerLineDto>
            {
                // If Sales Tax (IsSalesTransaction = true):
                //   Debit: Source Control Account (e.g., AR Control)
                //   Credit: Tax Payable Account
                // If Purchase Tax (IsSalesTransaction = false):
                //   Debit: Tax Recoverable/Expense Account (e.g., Inventory, Expense)
                //   Credit: Source Control Account (e.g., AP Control)

                // For simplicity, we'll use the notification's logic directly.
                // In a real system, the GL accounts would be resolved dynamically.

                // Debit side
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.IsSalesTransaction ? notification.SourceControlAccountId : notification.TaxPayableAccountId,
                    Amount = notification.TaxAmount.Amount,
                    IsDebit = true,
                    Currency = notification.TaxAmount.Currency,
                    CostCenterId = notification.CostCenterId
                },
                // Credit side
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.IsSalesTransaction ? notification.TaxPayableAccountId : notification.SourceControlAccountId,
                    Amount = notification.TaxAmount.Amount,
                    IsDebit = false,
                    Currency = notification.TaxAmount.Currency,
                    CostCenterId = notification.CostCenterId
                }
            }
        };

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}