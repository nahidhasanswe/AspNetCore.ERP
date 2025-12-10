using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.TransferAsset;

public class TransferAssetCommand : IRequest<Result>
{
    public Guid BusinessUnitId { get; set; }
    public Guid AssetId { get; set; }
    public Guid NewCostCenterId { get; set; }
    public DateTime TransferDate { get; set; }
}