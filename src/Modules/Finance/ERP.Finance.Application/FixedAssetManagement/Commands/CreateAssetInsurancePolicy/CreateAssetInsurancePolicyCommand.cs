using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.CreateAssetInsurancePolicy;

public class CreateAssetInsurancePolicyCommand : IRequest<Result<Guid>>
{
    public Guid AssetId { get; set; }
    public string PolicyNumber { get; set; }
    public string Insurer { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Money CoverageAmount { get; set; }
    public Money PremiumAmount { get; set; }
}