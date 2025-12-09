using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.RetireFixedAsset;

public class RetireFixedAssetCommand : IRequest<Result>
{
    public Guid AssetId { get; set; }
    public DateTime RetirementDate { get; set; }
    public string Reason { get; set; }
}