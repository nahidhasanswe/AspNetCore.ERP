using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.FiscalYear.Commands.CreateFiscalPeriod;

public class CreateFiscalPeriodCommand : IRequest<Result<Guid>>
{
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}