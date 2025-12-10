using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.FiscalYear.Commands.ReopenFiscalPeriod;

public class ReopenFiscalPeriodCommand : IRequest<Result>
{
    public Guid BusinessUnitId { get; set; } // New property
    public Guid FiscalPeriodId { get; set; }
}