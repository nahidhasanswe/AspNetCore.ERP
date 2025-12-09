using ERP.Core;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.CreateLeasedAsset;

public class CreateLeasedAssetCommand : IRequest<Result<Guid>>
{
    public Guid AssetId { get; set; } // Link to a FixedAsset if tracked there
    public string LeaseAgreementNumber { get; set; }
    public string Lessor { get; set; }
    public LeaseType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Money MonthlyPayment { get; set; }
}