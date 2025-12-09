using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.FixedAssetManagement.Events;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class AssetRevaluedEventHandler : INotificationHandler<AssetRevaluedEvent>
{
    private readonly IMediator _mediator;

    public AssetRevaluedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(AssetRevaluedEvent notification, CancellationToken cancellationToken)
    {
        // GL Entry for Asset Revaluation:
        // This is a complex entry. For simplicity, we'll assume:
        // 1. Reverse old accumulated depreciation (Debit Acc. Dep.)
        // 2. Reverse old asset cost (Credit Asset Account)
        // 3. Record new asset cost (Debit Asset Account)
        // 4. Record revaluation gain/loss (Credit/Debit Revaluation Gain/Loss Account)

        var journalEntries = new List<CreateJournalEntryCommand.LedgerLineDto>();

        // 1. Reverse old accumulated depreciation (Debit Acc. Dep.)
        if (notification.OldTotalAccumulatedDepreciation > 0)
        {
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.AccumulatedDepreciationAccountId,
                Amount = notification.OldTotalAccumulatedDepreciation,
                IsDebit = true,
                Currency = notification.OldAcquisitionCost.Currency,
                CostCenterId = notification.CostCenterId
            });
        }

        // 2. Reverse old asset cost (Credit Asset Account)
        journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.AssetAccountId,
            Amount = notification.OldAcquisitionCost.Amount,
            IsDebit = false,
            Currency = notification.OldAcquisitionCost.Currency,
            CostCenterId = notification.CostCenterId
        });

        // 3. Record new asset cost (Debit Asset Account)
        journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.AssetAccountId,
            Amount = notification.NewAcquisitionCost.Amount,
            IsDebit = true,
            Currency = notification.NewAcquisitionCost.Currency,
            CostCenterId = notification.CostCenterId
        });

        // 4. Record revaluation gain/loss
        decimal revaluationDifference = notification.NewAcquisitionCost.Amount - notification.OldAcquisitionCost.Amount;
        if (revaluationDifference > 0) // Revaluation Gain
        {
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.RevaluationGainLossAccountId,
                Amount = revaluationDifference,
                IsDebit = false, // Credit for Gain
                Currency = notification.NewAcquisitionCost.Currency,
                CostCenterId = notification.CostCenterId
            });
        }
        else if (revaluationDifference < 0) // Revaluation Loss
        {
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.RevaluationGainLossAccountId,
                Amount = Math.Abs(revaluationDifference),
                IsDebit = true, // Debit for Loss
                Currency = notification.NewAcquisitionCost.Currency,
                CostCenterId = notification.CostCenterId
            });
        }

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.RevaluationDate,
            Description = $"Journal entry for revaluation of asset {notification.AssetId}",
            Lines = journalEntries
        };

        await _mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}