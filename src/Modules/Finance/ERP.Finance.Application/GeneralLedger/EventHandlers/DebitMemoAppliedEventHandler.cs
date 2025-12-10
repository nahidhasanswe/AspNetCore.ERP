using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsPayable.Events;
using ERP.Finance.Domain.GeneralLedger.Service; // Assuming IGlConfigurationService is here
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class DebitMemoAppliedEventHandler : INotificationHandler<DebitMemoAppliedEvent>
{
    private readonly IMediator _mediator;
    private readonly IGlConfigurationService _glConfig; // Injected

    public DebitMemoAppliedEventHandler(IMediator mediator, IGlConfigurationService glConfig)
    {
        _mediator = mediator;
        _glConfig = glConfig;
    }

    public async Task Handle(DebitMemoAppliedEvent notification, CancellationToken cancellationToken)
    {
        Guid debitMemoClearingAccountId = await _glConfig.DebitMemoClearingAccountIdAsync(notification.Amount.Currency, cancellationToken);

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.AppliedDate,
            Description = $"Debit Memo {notification.DebitMemoId} applied for vendor {notification.VendorId}",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId to Journal Entry
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

        await _mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}