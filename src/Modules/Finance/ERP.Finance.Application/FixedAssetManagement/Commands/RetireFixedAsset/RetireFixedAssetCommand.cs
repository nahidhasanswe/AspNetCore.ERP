using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.RetireFixedAsset;

public class RetireFixedAssetCommand : IRequest<Result>
{
    public Guid BusinessUnitId { get; set; }
    public Guid AssetId { get; set; }
    public DateTime RetirementDate { get; set; }
    public string Reason { get; set; }
}