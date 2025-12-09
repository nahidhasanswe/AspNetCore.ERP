using ERP.Core;
using ERP.Finance.Application.FixedAssetManagement.DTOs;
using ERP.Finance.Domain.FixedAssetManagement.Enums;
using MediatR;
using System.Collections.Generic;

namespace ERP.Finance.Application.FixedAssetManagement.Queries.ListFixedAssets;

public class ListFixedAssetsQuery : IRequest<Result<IEnumerable<AssetSummaryDto>>>
{
    public FixedAssetStatus? Status { get; set; }
    public string? TagNumber { get; set; }
}