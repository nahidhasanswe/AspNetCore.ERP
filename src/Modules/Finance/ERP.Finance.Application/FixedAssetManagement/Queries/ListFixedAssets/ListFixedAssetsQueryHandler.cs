using ERP.Core;
using ERP.Finance.Application.FixedAssetManagement.DTOs;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Queries.ListFixedAssets;

public class ListFixedAssetsQueryHandler(IFixedAssetRepository fixedAssetRepository)
    : IRequestHandler<ListFixedAssetsQuery, Result<IEnumerable<AssetSummaryDto>>>
{
    public async Task<Result<IEnumerable<AssetSummaryDto>>> Handle(ListFixedAssetsQuery request, CancellationToken cancellationToken)
    {
        var allAssets = await fixedAssetRepository.ListAllAsync(cancellationToken);

        var filteredAssets = allAssets.AsQueryable();

        if (request.Status.HasValue)
        {
            filteredAssets = filteredAssets.Where(asset => asset.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.TagNumber))
        {
            filteredAssets = filteredAssets.Where(asset => asset.TagNumber.Contains(request.TagNumber));
        }

        var summaryDtos = filteredAssets.Select(asset => new AssetSummaryDto(
            asset.Id,
            asset.TagNumber,
            asset.Description,
            asset.AcquisitionDate,
            asset.AcquisitionCost,
            asset.TotalAccumulatedDepreciation,
            asset.Status
        )).ToList();

        return Result.Success(summaryDtos.AsEnumerable());
    }
}