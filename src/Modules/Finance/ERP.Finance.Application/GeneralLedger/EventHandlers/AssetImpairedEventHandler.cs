using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.FixedAssetManagement.Events;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class AssetImpairedEventHandler : INotificationHandler<AssetImpairedEvent>
{
    private readonly IMediator _mediator;

    public AssetImpairedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(AssetImpairedEvent notification, CancellationToken cancellationToken)
    {
        // GL Entry for Asset Impairment:
        // Debit: Impairment Loss Account
        // Credit: Accumulated Depreciation (or directly to Asset Account, depending on policy)

        var journalEntries = new List<CreateJournalEntryCommand.LedgerLineDto>();

        // Debit Impairment Loss Account
        journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.ImpairmentLossAccountId,
            Amount = notification.ImpairmentLossAmount.Amount,
            IsDebit = true,
            Currency = notification.ImpairmentLossAmount.Currency,
            CostCenterId = notification.CostCenterId
        });

        // Credit Asset Account (reducing the asset's book value)
        journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.AssetAccountId,
            Amount = notification.ImpairmentLossAmount.Amount,
            IsDebit = false,
            Currency = notification.ImpairmentLossAmount.Currency,
            CostCenterId = notification.CostCenterId
        });

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.ImpairmentDate,
            Description = $"Journal entry for impairment of asset {notification.AssetId}",
            Lines = journalEntries
        };

        await _mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}