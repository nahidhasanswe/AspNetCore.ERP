using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsReceivable.Events;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class CashReceiptRefundedEventHandler(IMediator mediator) : INotificationHandler<CashReceiptRefundedEvent>
{
    // In a real system, you'd need the AR Control Account.

    public async Task Handle(CashReceiptRefundedEvent notification, CancellationToken cancellationToken)
    {
        // GL Entry for a refund:
        // Debit: AR Control Account (to reverse the original credit from cash receipt)
        // Credit: Cash Account (from which the refund is made)

        // We need the AR Control Account. For now, using a placeholder.
        Guid arControlAccountId = Guid.NewGuid(); // Placeholder for AR Control Account

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.RefundDate,
            Description = $"Journal entry for refund of cash receipt {notification.ReceiptId}. Reference: {notification.RefundReference}",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = new List<CreateJournalEntryCommand.LedgerLineDto>
            {
                // Debit AR Control Account (increases AR liability, reversing original credit)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = arControlAccountId, // This needs to be the AR Control Account
                    Amount = notification.RefundAmount.Amount,
                    IsDebit = true,
                    Currency = notification.RefundAmount.Currency
                },
                // Credit Cash Account (from which refund is made)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.RefundCashAccountId,
                    Amount = notification.RefundAmount.Amount,
                    IsDebit = false,
                    Currency = notification.RefundAmount.Currency
                }
            }
        };

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}