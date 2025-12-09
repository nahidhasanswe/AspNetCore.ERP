using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.FixedAssetManagement.Events;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class AssetTransferredEventHandler : INotificationHandler<AssetTransferredEvent>
{
    private readonly IMediator _mediator;

    public AssetTransferredEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(AssetTransferredEvent notification, CancellationToken cancellationToken)
    {
        // GL Entry for Asset Transfer (between cost centers):
        // This involves reversing the asset and accumulated depreciation from the old cost center
        // and re-recording them in the new cost center.

        var journalEntries = new List<CreateJournalEntryCommand.LedgerLineDto>();

        // 1. Reverse Asset and Accumulated Depreciation from Old Cost Center
        if (notification.OldCostCenterId.HasValue)
        {
            // Credit Asset Account (to remove from old CC)
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.AssetAccountId,
                Amount = notification.AcquisitionCost.Amount,
                IsDebit = false, // Credit
                Currency = notification.AcquisitionCost.Currency,
                CostCenterId = notification.OldCostCenterId
            });
            // Debit Accumulated Depreciation Account (to remove from old CC)
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.AccumulatedDepreciationAccountId,
                Amount = notification.TotalAccumulatedDepreciation,
                IsDebit = true, // Debit
                Currency = notification.AcquisitionCost.Currency,
                CostCenterId = notification.OldCostCenterId
            });
        }

        // 2. Record Asset and Accumulated Depreciation in New Cost Center
        if (notification.NewCostCenterId.HasValue)
        {
            // Debit Asset Account (to add to new CC)
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.AssetAccountId,
                Amount = notification.AcquisitionCost.Amount,
                IsDebit = true, // Debit
                Currency = notification.AcquisitionCost.Currency,
                CostCenterId = notification.NewCostCenterId
            });
            // Credit Accumulated Depreciation Account (to add to new CC)
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.AccumulatedDepreciationAccountId,
                Amount = notification.TotalAccumulatedDepreciation,
                IsDebit = false, // Credit
                Currency = notification.AcquisitionCost.Currency,
                CostCenterId = notification.NewCostCenterId
            });
        }

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.TransferDate,
            Description = $"Journal entry for transfer of asset {notification.AssetId} from CC {notification.OldCostCenterId} to {notification.NewCostCenterId}",
            Lines = journalEntries
        };

        await _mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}