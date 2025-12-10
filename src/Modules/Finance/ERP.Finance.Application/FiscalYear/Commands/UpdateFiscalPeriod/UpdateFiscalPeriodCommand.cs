using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.FiscalYear.Commands.UpdateFiscalPeriod;

public class UpdateFiscalPeriodCommand : IRequest<Result>
{
    public Guid BusinessUnitId { get; set; } // New property
    public Guid FiscalPeriodId { get; set; }
    public string NewName { get; set; }
    public DateTime NewStartDate { get; set; }
    public DateTime NewEndDate { get; set; }
}