using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.FixedAssetManagement.Events;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class AssetAcquiredEventHandler : INotificationHandler<AssetAcquiredEvent>
{
    private readonly IMediator _mediator;
    // In a real system, you'd need a Cash/AP Control Account.

    public AssetAcquiredEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(AssetAcquiredEvent notification, CancellationToken cancellationToken)
    {
        // GL Entry for Asset Acquisition:
        // Debit: Asset Account (e.g., Computer Equipment)
        // Credit: Cash / Accounts Payable Control Account

        // We need the Cash/AP Control Account. For now, using a placeholder.
        Guid cashOrAPControlAccountId = Guid.NewGuid(); // Placeholder

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.AcquisitionDate,
            Description = $"Journal entry for acquisition of asset {notification.TagNumber} (ID: {notification.AssetId})",
            Lines = new List<CreateJournalEntryCommand.LedgerLineDto>
            {
                // Debit Asset Account
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.AssetAccountId,
                    Amount = notification.AcquisitionCost.Amount,
                    IsDebit = true,
                    Currency = notification.AcquisitionCost.Currency,
                    CostCenterId = notification.CostCenterId
                },
                // Credit Cash / Accounts Payable Control Account
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = cashOrAPControlAccountId, // This would be dynamic based on how the asset was acquired
                    Amount = notification.AcquisitionCost.Amount,
                    IsDebit = false,
                    Currency = notification.AcquisitionCost.Currency,
                    CostCenterId = notification.CostCenterId
                }
            }
        };

        await _mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}