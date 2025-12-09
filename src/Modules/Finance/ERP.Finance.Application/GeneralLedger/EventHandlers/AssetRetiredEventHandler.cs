using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.FixedAssetManagement.Events;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class AssetRetiredEventHandler : INotificationHandler<AssetRetiredEvent>
{
    private readonly IMediator _mediator;

    public AssetRetiredEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(AssetRetiredEvent notification, CancellationToken cancellationToken)
    {
        // GL Entry for Asset Retirement:
        // Debit: Accumulated Depreciation (to remove it from books)
        // Debit: Loss on Retirement Account (for remaining book value)
        // Credit: Asset Account (to remove original cost)

        var journalEntries = new List<CreateJournalEntryCommand.LedgerLineDto>();

        // Debit Accumulated Depreciation
        journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.AccumulatedDepreciationAccountId,
            Amount = notification.TotalAccumulatedDepreciation,
            IsDebit = true,
            Currency = notification.AcquisitionCost.Currency, // Assuming same currency
            CostCenterId = notification.CostCenterId
        });

        // Calculate remaining book value (Loss on Retirement)
        decimal bookValue = notification.AcquisitionCost.Amount - notification.TotalAccumulatedDepreciation;
        if (bookValue > 0)
        {
            // We need a Loss on Retirement GL Account. For simplicity, using a placeholder.
            Guid lossOnRetirementAccountId = Guid.NewGuid(); // Placeholder
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = lossOnRetirementAccountId,
                Amount = bookValue,
                IsDebit = true,
                Currency = notification.AcquisitionCost.Currency,
                CostCenterId = notification.CostCenterId
            });
        }

        // Credit Asset Account (Original Cost)
        journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.AssetAccountId,
            Amount = notification.AcquisitionCost.Amount,
            IsDebit = false,
            Currency = notification.AcquisitionCost.Currency,
            CostCenterId = notification.CostCenterId
        });

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.RetirementDate,
            Description = $"Journal entry for retirement of asset {notification.AssetId}. Reason: {notification.Reason}",
            Lines = journalEntries
        };

        await _mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}