using ERP.Core;
using ERP.Finance.Application.FiscalYear.DTOs;
using ERP.Finance.Domain.FiscalYear.Enums;
using MediatR;

namespace ERP.Finance.Application.FiscalYear.Queries.ListFiscalPeriods;

public class ListFiscalPeriodsQuery : IRequest<Result<IEnumerable<FiscalPeriodDto>>>
{
    public Guid? BusinessUnitId { get; set; }
    public int? Year { get; set; }
    public PeriodStatus? Status { get; set; }
}