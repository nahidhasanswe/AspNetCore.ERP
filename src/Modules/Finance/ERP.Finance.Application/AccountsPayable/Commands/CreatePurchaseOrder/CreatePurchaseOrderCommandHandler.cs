using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreatePurchaseOrder;

public class CreatePurchaseOrderCommandHandler : IRequestCommandHandler<CreatePurchaseOrderCommand, Guid>
{
    private readonly IPurchaseOrderRepository _poRepository;
    private readonly IUnitOfWorkManager _unitOfWork;

    public CreatePurchaseOrderCommandHandler(IPurchaseOrderRepository poRepository, IUnitOfWorkManager unitOfWork)
    {
        _poRepository = poRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreatePurchaseOrderCommand command, CancellationToken cancellationToken)
    {
        using var scope = _unitOfWork.Begin();

        var poLines = command.Lines.Select(dto => 
            new PurchaseOrderLine(dto.ProductId, dto.Description, dto.Quantity, dto.UnitPrice))
            .ToList();

        var purchaseOrder = new PurchaseOrder(command.VendorId, command.OrderDate, poLines);

        await _poRepository.AddAsync(purchaseOrder, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(purchaseOrder.Id);
    }
}