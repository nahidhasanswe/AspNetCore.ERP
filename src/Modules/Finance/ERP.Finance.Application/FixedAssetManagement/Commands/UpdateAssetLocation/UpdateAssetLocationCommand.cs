using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.UpdateAssetLocation;

public class UpdateAssetLocationCommand : IRequest<Result>
{
    public Guid AssetId { get; set; }
    public string NewLocation { get; set; }
}