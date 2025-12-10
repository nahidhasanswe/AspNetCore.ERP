using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.RecordPhysicalInventory;

public class RecordPhysicalInventoryCommandHandler(
    IPhysicalInventoryRecordRepository recordRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<RecordPhysicalInventoryCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RecordPhysicalInventoryCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var record = new PhysicalInventoryRecord(
            command.BusinessUnitId, // Pass BusinessUnitId
            command.AssetId,
            command.CountDate,
            command.CountedLocation,
            command.CountedBy,
            command.Notes
        );

        await recordRepository.AddAsync(record, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(record.Id);
    }
}