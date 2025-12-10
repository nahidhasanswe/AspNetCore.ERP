using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.ImpairAsset;

public class ImpairAssetCommandHandler(IFixedAssetRepository fixedAssetRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<ImpairAssetCommand, Result>
{
    public async Task<Result> Handle(ImpairAssetCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var fixedAsset = await fixedAssetRepository.GetByIdAsync(command.AssetId, cancellationToken);
        if (fixedAsset == null)
        {
            return Result.Failure("Fixed Asset not found.");
        }

        if (fixedAsset.BusinessUnitId != command.BusinessUnitId)
        {
            return Result.Failure("Fixed Asset does not belong to the specified Business Unit.");
        }

        fixedAsset.Impair(
            command.ImpairmentDate,
            command.ImpairmentLossAmount,
            command.ImpairmentLossAccountId
        );

        await fixedAssetRepository.UpdateAsync(fixedAsset, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}