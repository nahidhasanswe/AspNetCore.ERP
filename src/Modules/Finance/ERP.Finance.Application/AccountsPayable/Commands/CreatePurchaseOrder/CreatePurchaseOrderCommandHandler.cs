using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreatePurchaseOrder;

public class CreatePurchaseOrderCommandHandler(IPurchaseOrderRepository poRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreatePurchaseOrderCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePurchaseOrderCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var poLines = command.Lines.Select(dto => 
            new PurchaseOrderLine(dto.ProductId, dto.Description, dto.Quantity, dto.UnitPrice))
            .ToList();

        var purchaseOrder = new PurchaseOrder(
            command.BusinessUnitId,
            command.VendorId,
            command.OrderDate,
            poLines
        );

        await poRepository.AddAsync(purchaseOrder, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(purchaseOrder.Id);
    }
}