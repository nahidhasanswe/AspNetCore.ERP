using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Domain.AccountsReceivable.Events;

namespace ERP.Finance.Application.AccountsReceivable.Commands.ConvertOverpaymentToCredit;

public class ConvertOverpaymentToCreditCommandHandler(
    ICashReceiptRepository cashReceiptRepository,
    ICustomerCreditMemoRepository creditMemoRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<ConvertOverpaymentToCreditCommand, Guid>
{
    public async Task<Result<Guid>> Handle(ConvertOverpaymentToCreditCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var cashReceipt = await cashReceiptRepository.GetByIdAsync(command.CashReceiptId, cancellationToken);
        if (cashReceipt == null)
        {
            return Result.Failure<Guid>("Cash Receipt not found.");
        }

        if (cashReceipt.BusinessUnitId != command.BusinessUnitId)
        {
            return Result.Failure<Guid>("Cash Receipt does not belong to the specified Business Unit.");
        }

        if (command.AmountToConvert.Amount <= 0)
        {
            return Result.Failure<Guid>("Amount to convert must be positive.");
        }
        if (command.AmountToConvert.Amount > cashReceipt.UnappliedAmount.Amount)
        {
            return Result.Failure<Guid>("Cannot convert more than the unapplied amount.");
        }
        if (command.AmountToConvert.Currency != cashReceipt.UnappliedAmount.Currency)
        {
            return Result.Failure<Guid>("Currency mismatch for amount to convert.");
        }

        // 1. Apply cash to the receipt (reducing unapplied amount)
        var applyResult = cashReceipt.ApplyCash(command.AmountToConvert);
        if (applyResult.IsFailure)
        {
            return Result.Failure<Guid>(applyResult.Error);
        }

        // 2. Create CustomerCreditMemo
        var creditMemo = new CustomerCreditMemo(
            command.BusinessUnitId,
            cashReceipt.CustomerId,
            command.AmountToConvert,
            DateTime.UtcNow,
            command.Reason
        );

        // 3. Raise event for GL posting for the credit memo issuance
        creditMemo.AddDomainEvent(new CustomerCreditMemoIssuedEvent(
            creditMemo.Id,
            creditMemo.CustomerId,
            creditMemo.BusinessUnitId,
            creditMemo.OriginalAmount,
            creditMemo.MemoDate,
            command.ARControlAccountId,
            command.RevenueAdjustmentAccountId,
            creditMemo.Reason
        ));

        await cashReceiptRepository.UpdateAsync(cashReceipt, cancellationToken);
        await creditMemoRepository.AddAsync(creditMemo, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(creditMemo.Id);
    }
}