using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.UpdateAssetLocation;

public class UpdateAssetLocationCommand : IRequest<Result>
{
    public Guid BusinessUnitId { get; set; } // New property
    public Guid AssetId { get; set; }
    public string NewLocation { get; set; }
}