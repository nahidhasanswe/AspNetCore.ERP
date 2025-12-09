using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsReceivable.Commands.RefundOverpayment;

public class RefundOverpaymentCommandHandler(
    ICashReceiptRepository cashReceiptRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<RefundOverpaymentCommand, Result>
{
    public async Task<Result> Handle(RefundOverpaymentCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var cashReceipt = await cashReceiptRepository.GetByIdAsync(command.CashReceiptId, cancellationToken);
        if (cashReceipt == null)
        {
            return Result.Failure("Cash Receipt not found.");
        }

        cashReceipt.Refund(
            command.RefundAmount,
            command.RefundCashAccountId,
            command.RefundReference
        );

        await cashReceiptRepository.UpdateAsync(cashReceipt, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}