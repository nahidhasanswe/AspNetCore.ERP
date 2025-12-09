using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.CreateFixedAsset;

public class CreateFixedAssetCommandHandler(IFixedAssetRepository fixedAssetRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreateFixedAssetCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateFixedAssetCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var schedule = new DepreciationSchedule(
            command.DepreciationMethod,
            command.UsefulLifeYears,
            command.SalvageValue
        );

        var fixedAsset = new FixedAsset(
            command.TagNumber,
            command.Description,
            command.AcquisitionCost,
            command.AcquisitionDate,
            command.AssetAccountId,
            command.DepreciationExpenseAccountId,
            command.AccumulatedDepreciationAccountId,
            schedule,
            command.CostCenterId,
            command.Location
        );

        await fixedAssetRepository.AddAsync(fixedAsset, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(fixedAsset.Id);
    }
}