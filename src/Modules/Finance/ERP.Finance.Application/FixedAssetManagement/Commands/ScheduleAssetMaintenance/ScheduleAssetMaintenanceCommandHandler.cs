using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.ScheduleAssetMaintenance;

public class ScheduleAssetMaintenanceCommandHandler(
    IAssetMaintenanceRecordRepository maintenanceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<ScheduleAssetMaintenanceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(ScheduleAssetMaintenanceCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var record = new AssetMaintenanceRecord(
            command.BusinessUnitId,
            command.AssetId,
            command.ScheduledDate,
            command.Description,
            command.Cost,
            command.PerformedBy
        );

        await maintenanceRepository.AddAsync(record, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(record.Id);
    }
}