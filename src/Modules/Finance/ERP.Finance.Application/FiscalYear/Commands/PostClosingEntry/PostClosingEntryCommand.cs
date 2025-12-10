using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.FiscalYear.Commands.PostClosingEntry;

public class PostClosingEntryCommand : IRequest<Result>
{
    public Guid BusinessUnitId { get; set; } // New property
    public Guid FiscalPeriodId { get; set; }
}