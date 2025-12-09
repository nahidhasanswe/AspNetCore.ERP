using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.RevalueAsset;

public class RevalueAssetCommand : IRequest<Result>
{
    public Guid AssetId { get; set; }
    public DateTime RevaluationDate { get; set; }
    public Money NewAcquisitionCost { get; set; }
    public Guid RevaluationGainLossAccountId { get; set; } // GL Account for Revaluation Gain/Loss
}