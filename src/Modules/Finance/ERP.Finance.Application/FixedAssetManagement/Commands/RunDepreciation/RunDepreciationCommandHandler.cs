using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using ERP.Finance.Domain.FixedAssetManagement.Enums;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.RunDepreciation;

public class RunDepreciationCommandHandler(IFixedAssetRepository fixedAssetRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<RunDepreciationCommand, Result>
{
    public async Task<Result> Handle(RunDepreciationCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var activeAssets = (await fixedAssetRepository.ListAllAsync(cancellationToken))
            .Where(asset => asset.Status == FixedAssetStatus.Active || asset.Status == FixedAssetStatus.UnderDepreciation)
            .ToList();

        foreach (var asset in activeAssets)
        {
            try
            {
                asset.Depreciate(command.PeriodDate, out var depreciationAmount);
                // If depreciationAmount is > 0, the asset was updated and needs saving.
                // The Depreciate method already raises the DepreciationPostedEvent.
                if (depreciationAmount.Amount > 0)
                {
                    await fixedAssetRepository.UpdateAsync(asset, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                // Log the error for this specific asset but continue with others
                System.Console.WriteLine($"Error depreciating asset {asset.Id}: {ex.Message}");
            }
        }

        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}