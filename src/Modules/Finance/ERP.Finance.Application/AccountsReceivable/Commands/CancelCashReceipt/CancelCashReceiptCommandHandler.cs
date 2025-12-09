using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CancelCashReceipt;

public class CancelCashReceiptCommandHandler(
    ICashReceiptRepository cashReceiptRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<CancelCashReceiptCommand, Result>
{
    public async Task<Result> Handle(CancelCashReceiptCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var cashReceipt = await cashReceiptRepository.GetByIdAsync(command.CashReceiptId, cancellationToken);
        if (cashReceipt == null)
        {
            return Result.Failure("Cash Receipt not found.");
        }

        cashReceipt.Cancel();

        await cashReceiptRepository.UpdateAsync(cashReceipt, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}