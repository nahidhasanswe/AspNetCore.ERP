using ERP.Core.Behaviors;
using ERP.Finance.Application.FiscalYear.DTOs;

namespace ERP.Finance.Application.FiscalYear.Queries;

public class ListFiscalPeriodsQuery : IResultQuery<IEnumerable<FiscalPeriodDto>>
{
    public int? YearFilter { get; set; } 
}