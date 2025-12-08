using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.ReceiveGoodsOnPO;

public class ReceiveGoodsOnPoCommandHandler(IPurchaseOrderRepository poRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<ReceiveGoodsOnPOCommand, Unit>
{
    public async Task<Result<Unit>> Handle(ReceiveGoodsOnPOCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var purchaseOrder = await poRepository.GetByIdAsync(command.PurchaseOrderId, cancellationToken);
        if (purchaseOrder == null)
        {
            return Result.Failure<Unit>("Purchase Order not found.");
        }

        purchaseOrder.ReceiveGoods(command.PurchaseOrderLineId, command.QuantityReceived);

        await poRepository.UpdateAsync(purchaseOrder, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}