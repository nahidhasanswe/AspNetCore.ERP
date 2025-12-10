using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsReceivable.Events;
using ERP.Finance.Domain.GeneralLedger.Services;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class UnappliedCashGlPostingHandler(IMediator mediator, IGLConfigurationService glConfig)
    : INotificationHandler<UnappliedCashCreatedEvent>
{
    // Injected

    public async Task Handle(UnappliedCashCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Resolve Unapplied Cash GL Account ID
        Guid unappliedCashLiabilityId = await glConfig.GetUnappliedCashClearingAccountId(notification.TotalReceivedAmount.Currency, cancellationToken);

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.OccurredOn, // Assuming OccurredOn is the posting date
            Description = $"Cash Received (Unapplied) from Customer {notification.CustomerId}",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = new List<CreateJournalEntryCommand.LedgerLineDto>
            {
                // DEBIT: Cash/Bank Account (Asset Increase)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.CashAccountId, // Specific GL bank account ID (DEBITED)
                    Amount = notification.TotalReceivedAmount.Amount,
                    IsDebit = true,
                    Currency = notification.TotalReceivedAmount.Currency
                },
                // CREDIT: Unapplied Cash/Cash Clearing Account (Liability/Contra-Asset Increase)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = unappliedCashLiabilityId, // The GL account that holds unapplied cash (CREDITED)
                    Amount = notification.TotalReceivedAmount.Amount,
                    IsDebit = false,
                    Currency = notification.TotalReceivedAmount.Currency
                }
            }
        };

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}