using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.Queries.GetIncomeStatement;

public class GetIncomeStatementQuery : IRequest<Result<IncomeStatementDto>>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}