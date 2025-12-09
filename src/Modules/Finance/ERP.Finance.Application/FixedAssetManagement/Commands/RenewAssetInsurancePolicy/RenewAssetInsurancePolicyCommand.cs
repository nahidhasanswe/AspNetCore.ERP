using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.RenewAssetInsurancePolicy;

public class RenewAssetInsurancePolicyCommand : IRequest<Result>
{
    public Guid PolicyId { get; set; }
    public DateTime NewEndDate { get; set; }
    public Money NewCoverageAmount { get; set; }
    public Money NewPremiumAmount { get; set; }
}