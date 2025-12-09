using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsReceivable.Commands.ReverseCashReceipt;

public class ReverseCashReceiptCommandHandler(
    ICashReceiptRepository cashReceiptRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<ReverseCashReceiptCommand, Result>
{
    public async Task<Result> Handle(ReverseCashReceiptCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var cashReceipt = await cashReceiptRepository.GetByIdAsync(command.CashReceiptId, cancellationToken);
        if (cashReceipt == null)
        {
            return Result.Failure("Cash Receipt not found.");
        }

        cashReceipt.Reverse(command.Reason);

        await cashReceiptRepository.UpdateAsync(cashReceipt, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}