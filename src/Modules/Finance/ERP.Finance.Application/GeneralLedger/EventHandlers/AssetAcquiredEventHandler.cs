using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.FixedAssetManagement.Events;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class AssetAcquiredEventHandler(IMediator mediator) : INotificationHandler<AssetAcquiredEvent>
{
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
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
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

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}