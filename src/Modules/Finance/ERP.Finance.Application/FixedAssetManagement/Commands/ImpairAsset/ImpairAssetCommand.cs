using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.ImpairAsset;

public class ImpairAssetCommand : IRequest<Result>
{
    public Guid AssetId { get; set; }
    public DateTime ImpairmentDate { get; set; }
    public Money ImpairmentLossAmount { get; set; }
    public Guid ImpairmentLossAccountId { get; set; } // GL Account for Impairment Loss
}