using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.FixedAssetManagement.Commands;

public class RegisterFixedAssetCommand : IRequest<Result<Guid>>
{
    public string TagNumber { get; set; }
    public string Description { get; set; }
    public DateTime AcquisitionDate { get; set; }
    public decimal AcquisitionCost { get; set; }
    public string Currency { get; set; }
    public decimal SalvageValue { get; set; }
    public int UsefulLifeYears { get; set; }
    public string DepreciationMethod { get; set; } // e.g., "StraightLine"
    
    // GL Account IDs needed for posting
    public Guid AssetAccountId { get; set; } 
    public Guid DepreciationExpenseAccountId { get; set; } 
    public Guid AccumulatedDepreciationAccountId { get; set; }
}