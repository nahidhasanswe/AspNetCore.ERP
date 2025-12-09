using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Domain.AccountsReceivable.Events;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsReceivable.Commands.ConvertOverpaymentToCredit;

public class ConvertOverpaymentToCreditCommandHandler : IRequestCommandHandler<ConvertOverpaymentToCreditCommand, Guid>
{
    private readonly ICashReceiptRepository _cashReceiptRepository;
    private readonly ICustomerCreditMemoRepository _creditMemoRepository;
    private readonly IUnitOfWorkManager _unitOfWork;

    public ConvertOverpaymentToCreditCommandHandler(
        ICashReceiptRepository cashReceiptRepository,
        ICustomerCreditMemoRepository creditMemoRepository,
        IUnitOfWorkManager unitOfWork)
    {
        _cashReceiptRepository = cashReceiptRepository;
        _creditMemoRepository = creditMemoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ConvertOverpaymentToCreditCommand command, CancellationToken cancellationToken)
    {
        using var scope = _unitOfWork.Begin();

        var cashReceipt = await _cashReceiptRepository.GetByIdAsync(command.CashReceiptId, cancellationToken);
        if (cashReceipt == null)
        {
            return Result.Failure<Guid>("Cash Receipt not found.");
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
            cashReceipt.CustomerId,
            command.AmountToConvert,
            DateTime.UtcNow,
            command.Reason
        );

        // 3. Raise event for GL posting for the credit memo issuance
        creditMemo.AddDomainEvent(new CustomerCreditMemoIssuedEvent(
            creditMemo.Id,
            creditMemo.CustomerId,
            creditMemo.OriginalAmount,
            creditMemo.MemoDate,
            command.ARControlAccountId,
            command.RevenueAdjustmentAccountId,
            creditMemo.Reason
        ));

        await _cashReceiptRepository.UpdateAsync(cashReceipt, cancellationToken);
        await _creditMemoRepository.AddAsync(creditMemo, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(creditMemo.Id);
    }
}