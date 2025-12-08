using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsPayable.Events;
using ERP.Finance.Domain.GeneralLedger.Service;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class DebitMemoAppliedEventHandler(
    IMediator mediator,
    IGlConfigurationService glConfig) : INotificationHandler<DebitMemoAppliedEvent>
{
    public async Task Handle(DebitMemoAppliedEvent notification, CancellationToken cancellationToken)
    {
        Guid debitMemoClearingAccountId =
            await glConfig.DebitMemoClearingAccountIdAsync(notification.Amount.Currency, cancellationToken);

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.AppliedDate,
            Description = $"Debit Memo {notification.DebitMemoId} applied for vendor {notification.VendorId}",
            Lines = new List<CreateJournalEntryCommand.LedgerLineDto>
            {
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = debitMemoClearingAccountId,
                    Amount = notification.Amount.Amount,
                    IsDebit = true,
                    Currency = notification.Amount.Currency
                },
              
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.APControlAccountId,
                    Amount = notification.Amount.Amount,
                    IsDebit = false,
                    Currency = notification.Amount.Currency
                }
            }
        };

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}