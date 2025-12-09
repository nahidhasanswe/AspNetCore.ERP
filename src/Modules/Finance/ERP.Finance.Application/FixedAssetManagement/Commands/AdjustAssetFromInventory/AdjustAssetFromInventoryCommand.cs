using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.AdjustAssetFromInventory;

public class AdjustAssetFromInventoryCommand : IRequest<Result>
{
    public Guid RecordId { get; set; } // The physical inventory record
    public Guid AssetId { get; set; } // The asset to adjust
    public string NewLocation { get; set; } // The location found during count
}