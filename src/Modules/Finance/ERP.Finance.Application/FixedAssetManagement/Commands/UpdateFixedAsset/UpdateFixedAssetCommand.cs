using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.UpdateFixedAsset;

public class UpdateFixedAssetCommand : IRequest<Result>
{
    public Guid AssetId { get; set; }
    public Guid BusinessUnitId { get; set; } // New property
    public string Description { get; set; }
    public Guid AssetAccountId { get; set; }
    public Guid DepreciationExpenseAccountId { get; set; }
    public Guid AccumulatedDepreciationAccountId { get; set; }
    public Guid? CostCenterId { get; set; }
    public string Location { get; set; }
}