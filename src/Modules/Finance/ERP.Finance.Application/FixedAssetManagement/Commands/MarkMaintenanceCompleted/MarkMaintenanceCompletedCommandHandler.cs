using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.MarkMaintenanceCompleted;

public class MarkMaintenanceCompletedCommandHandler(
    IAssetMaintenanceRecordRepository maintenanceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<MarkMaintenanceCompletedCommand, Result>
{
    public async Task<Result> Handle(MarkMaintenanceCompletedCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var record = await maintenanceRepository.GetByIdAsync(command.MaintenanceRecordId, cancellationToken);
        if (record == null)
        {
            return Result.Failure("Maintenance Record not found.");
        }

        if (record.BusinessUnitId != command.BusinessUnitId)
        {
            return Result.Failure("Maintenance Record does not belong to the specified Business Unit.");
        }

        record.MarkCompleted(command.CompletionDate);

        await maintenanceRepository.UpdateAsync(record, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}