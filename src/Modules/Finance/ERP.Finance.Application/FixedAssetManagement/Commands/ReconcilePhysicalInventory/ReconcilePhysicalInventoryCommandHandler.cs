using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.ReconcilePhysicalInventory;

public class ReconcilePhysicalInventoryCommandHandler(
    IPhysicalInventoryRecordRepository recordRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<ReconcilePhysicalInventoryCommand, Result>
{
    public async Task<Result> Handle(ReconcilePhysicalInventoryCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var record = await recordRepository.GetByIdAsync(command.RecordId, cancellationToken);
        if (record == null)
        {
            return Result.Failure("Physical Inventory Record not found.");
        }

        if (record.BusinessUnitId != command.BusinessUnitId)
        {
            return Result.Failure("Physical Inventory Record does not belong to the specified Business Unit.");
        }

        record.MarkAsReconciled();

        await recordRepository.UpdateAsync(record, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}