using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.RecordPhysicalInventory;

public class RecordPhysicalInventoryCommandHandler : IRequestCommandHandler<RecordPhysicalInventoryCommand, Guid>
{
    private readonly IPhysicalInventoryRecordRepository _recordRepository;
    private readonly IUnitOfWorkManager _unitOfWork;

    public RecordPhysicalInventoryCommandHandler(IPhysicalInventoryRecordRepository recordRepository, IUnitOfWorkManager unitOfWork)
    {
        _recordRepository = recordRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(RecordPhysicalInventoryCommand command, CancellationToken cancellationToken)
    {
        using var scope = _unitOfWork.Begin();

        var record = new PhysicalInventoryRecord(
            command.AssetId,
            command.CountDate,
            command.CountedLocation,
            command.CountedBy,
            command.Notes
        );

        await _recordRepository.AddAsync(record, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(record.Id);
    }
}