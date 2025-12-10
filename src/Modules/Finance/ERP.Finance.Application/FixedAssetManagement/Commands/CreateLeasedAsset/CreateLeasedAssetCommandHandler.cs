using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.CreateLeasedAsset;

public class CreateLeasedAssetCommandHandler(
    ILeasedAssetRepository leasedAssetRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreateLeasedAssetCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateLeasedAssetCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var leasedAsset = new LeasedAsset(
            command.BusinessUnitId,
            command.AssetId,
            command.LeaseAgreementNumber,
            command.Lessor,
            command.Type,
            command.StartDate,
            command.EndDate,
            command.MonthlyPayment
        );

        await leasedAssetRepository.AddAsync(leasedAsset, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(leasedAsset.Id);
    }
}