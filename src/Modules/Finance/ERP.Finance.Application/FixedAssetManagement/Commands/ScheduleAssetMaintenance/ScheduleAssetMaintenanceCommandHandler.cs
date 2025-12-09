using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.ScheduleAssetMaintenance;

public class ScheduleAssetMaintenanceCommandHandler : IRequestCommandHandler<ScheduleAssetMaintenanceCommand, Guid>
{
    private readonly IAssetMaintenanceRecordRepository _maintenanceRepository;
    private readonly IUnitOfWorkManager _unitOfWork;

    public ScheduleAssetMaintenanceCommandHandler(IAssetMaintenanceRecordRepository maintenanceRepository, IUnitOfWorkManager unitOfWork)
    {
        _maintenanceRepository = maintenanceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ScheduleAssetMaintenanceCommand command, CancellationToken cancellationToken)
    {
        using var scope = _unitOfWork.Begin();

        var record = new AssetMaintenanceRecord(
            command.AssetId,
            command.ScheduledDate,
            command.Description,
            command.Cost,
            command.PerformedBy
        );

        await _maintenanceRepository.AddAsync(record, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(record.Id);
    }
}