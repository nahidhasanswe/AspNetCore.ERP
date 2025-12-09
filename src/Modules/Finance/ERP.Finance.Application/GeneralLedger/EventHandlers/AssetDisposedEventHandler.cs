using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.FixedAssetManagement.Events;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class AssetDisposedEventHandler : INotificationHandler<AssetDisposedEvent>
{
    private readonly IMediator _mediator;

    public AssetDisposedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(AssetDisposedEvent notification, CancellationToken cancellationToken)
    {
        // GL Entry for Asset Disposal:
        // Debit: Cash (for Proceeds)
        // Debit: Accumulated Depreciation (to remove it from books)
        // Debit/Credit: Gain/Loss on Disposal Account (to balance)
        // Credit: Asset Account (to remove original cost)

        var journalEntries = new List<CreateJournalEntryCommand.LedgerLineDto>();

        // Debit Cash (Proceeds)
        if (notification.Proceeds.Amount > 0)
        {
            // Assuming a Cash Account. For simplicity, using a placeholder.
            Guid cashAccountId = Guid.NewGuid(); // Placeholder
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = cashAccountId,
                Amount = notification.Proceeds.Amount,
                IsDebit = true,
                Currency = notification.Proceeds.Currency,
                CostCenterId = notification.CostCenterId
            });
        }

        // Debit Accumulated Depreciation
        journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.AccumulatedDepreciationAccountId,
            Amount = notification.TotalAccumulatedDepreciation,
            IsDebit = true,
            Currency = notification.AcquisitionCost.Currency, // Assuming same currency
            CostCenterId = notification.CostCenterId
        });

        // Credit Asset Account (Original Cost)
        journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.AssetAccountId,
            Amount = notification.AcquisitionCost.Amount,
            IsDebit = false,
            Currency = notification.AcquisitionCost.Currency,
            CostCenterId = notification.CostCenterId
        });

        // Debit/Credit Gain/Loss on Disposal
        if (notification.GainLossAmount > 0) // Gain
        {
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.GainLossAccountId,
                Amount = notification.GainLossAmount,
                IsDebit = false, // Credit for Gain
                Currency = notification.AcquisitionCost.Currency,
                CostCenterId = notification.CostCenterId
            });
        }
        else if (notification.GainLossAmount < 0) // Loss
        {
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.GainLossAccountId,
                Amount = Math.Abs(notification.GainLossAmount),
                IsDebit = true, // Debit for Loss
                Currency = notification.AcquisitionCost.Currency,
                CostCenterId = notification.CostCenterId
            });
        }

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.DisposalDate,
            Description = $"Journal entry for disposal of asset {notification.AssetId}",
            Lines = journalEntries
        };

        await _mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}