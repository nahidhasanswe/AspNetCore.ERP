using ERP.Core;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using ERP.Finance.Domain.Shared.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.CreateFixedAsset;

public class CreateFixedAssetCommand : IRequest<Result<Guid>>
{
    public string TagNumber { get; set; }
    public string Description { get; set; }
    public DateTime AcquisitionDate { get; set; }
    public Money AcquisitionCost { get; set; }
    public Guid AssetAccountId { get; set; }
    public Guid DepreciationExpenseAccountId { get; set; }
    public Guid AccumulatedDepreciationAccountId { get; set; }
    public DepreciationMethod DepreciationMethod { get; set; }
    public int UsefulLifeYears { get; set; }
    public decimal SalvageValue { get; set; }
    public Guid? CostCenterId { get; set; }
}