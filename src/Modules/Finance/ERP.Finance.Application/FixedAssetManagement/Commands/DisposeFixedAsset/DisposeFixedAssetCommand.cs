using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.DisposeFixedAsset;

public class DisposeFixedAssetCommand : IRequest<Result>
{
    public Guid AssetId { get; set; }
    public DateTime DisposalDate { get; set; }
    public Money Proceeds { get; set; }
    public Guid GainLossAccountId { get; set; } // GL Account for Gain/Loss on Disposal
}