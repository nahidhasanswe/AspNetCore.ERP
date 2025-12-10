using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.FixedAssetManagement.Events;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class DepreciationPostedHandler(IMediator mediator) : INotificationHandler<DepreciationPostedEvent>
{
    public async Task Handle(DepreciationPostedEvent notification, CancellationToken cancellationToken)
    {
        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.PeriodDate,
            Description = $"Journal entry for depreciation of asset {notification.AssetId} for period {notification.PeriodDate:yyyy-MM}",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = new List<CreateJournalEntryCommand.LedgerLineDto>
            {
                // Debit Depreciation Expense Account
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.DepreciationExpenseAccountId,
                    Amount = notification.Amount.Amount,
                    IsDebit = true,
                    Currency = notification.Amount.Currency,
                    CostCenterId = notification.CostCenterId
                },
                // Credit Accumulated Depreciation Account
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.AccumulatedDepreciationAccountId,
                    Amount = notification.Amount.Amount,
                    IsDebit = false,
                    Currency = notification.Amount.Currency,
                    CostCenterId = notification.CostCenterId
                }
            }
        };

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}