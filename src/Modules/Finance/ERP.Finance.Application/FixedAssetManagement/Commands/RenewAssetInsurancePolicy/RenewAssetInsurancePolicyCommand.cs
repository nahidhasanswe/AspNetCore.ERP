using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.RenewAssetInsurancePolicy;

public class RenewAssetInsurancePolicyCommand : IRequest<Result>
{
    public Guid BusinessUnitId { get; set; }
    public Guid PolicyId { get; set; }
    public DateTime NewEndDate { get; set; }
    public Money NewCoverageAmount { get; set; }
    public Money NewPremiumAmount { get; set; }
}