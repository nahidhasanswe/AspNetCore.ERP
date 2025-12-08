using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.AddLineToPO;

public class AddLineToPoCommandHandler(IPurchaseOrderRepository poRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<AddLineToPOCommand, Unit>
{
    public async Task<Result<Unit>> Handle(AddLineToPOCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var purchaseOrder = await poRepository.GetByIdAsync(command.PurchaseOrderId, cancellationToken);
        if (purchaseOrder == null)
        {
            return Result.Failure<Unit>("Purchase Order not found.");
        }

        purchaseOrder.AddLine(command.ProductId, command.Description, command.Quantity, command.UnitPrice);

        await poRepository.UpdateAsync(purchaseOrder, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}