using ERP.Core;
using ERP.Finance.Application.FixedAssetManagement.DTOs;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Queries.GetFixedAssetById;

public class GetFixedAssetByIdQueryHandler(
    IFixedAssetRepository fixedAssetRepository,
    IAccountRepository glAccountRepository)
    : IRequestHandler<GetFixedAssetByIdQuery, Result<AssetDetailsDto>>
{
    public async Task<Result<AssetDetailsDto>> Handle(GetFixedAssetByIdQuery request, CancellationToken cancellationToken)
    {
        var fixedAsset = await fixedAssetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (fixedAsset == null)
        {
            return Result.Failure<AssetDetailsDto>("Fixed Asset not found.");
        }

        var assetAccountName = await glAccountRepository.GetAccountNameAsync(fixedAsset.AssetAccountId, cancellationToken);
        var depreciationExpenseAccountName = await glAccountRepository.GetAccountNameAsync(fixedAsset.DepreciationExpenseAccountId, cancellationToken);
        var accumulatedDepreciationAccountName = await glAccountRepository.GetAccountNameAsync(fixedAsset.AccumulatedDepreciationAccountId, cancellationToken); // Corrected method name

        var currentBookValue = fixedAsset.AcquisitionCost.Amount - fixedAsset.TotalAccumulatedDepreciation;
        var isFullyDepreciated = currentBookValue <= fixedAsset.Schedule.SalvageValue;

        var dto = new AssetDetailsDto(
            fixedAsset.Id,
            fixedAsset.TagNumber,
            fixedAsset.Description,
            fixedAsset.AcquisitionDate,
            fixedAsset.AcquisitionCost,
            currentBookValue,
            fixedAsset.TotalAccumulatedDepreciation,
            isFullyDepreciated,
            fixedAsset.Status,
            fixedAsset.AssetAccountId,
            assetAccountName ?? "Unknown Asset Account",
            fixedAsset.DepreciationExpenseAccountId,
            depreciationExpenseAccountName ?? "Unknown Depreciation Expense Account",
            fixedAsset.AccumulatedDepreciationAccountId,
            accumulatedDepreciationAccountName ?? "Unknown Accumulated Depreciation Account",
            fixedAsset.Schedule.Method,
            fixedAsset.Schedule.UsefulLifeYears,
            fixedAsset.Schedule.SalvageValue,
            fixedAsset.CostCenterId
        );

        return Result.Success(dto);
    }
}