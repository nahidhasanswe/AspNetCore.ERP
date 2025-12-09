using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.TransferAsset;

public class TransferAssetCommand : IRequest<Result>
{
    public Guid AssetId { get; set; }
    public Guid NewCostCenterId { get; set; }
    public DateTime TransferDate { get; set; }
}