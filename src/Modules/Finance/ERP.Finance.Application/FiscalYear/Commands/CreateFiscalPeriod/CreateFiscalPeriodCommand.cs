using ERP.Core.Behaviors;

namespace ERP.Finance.Application.FiscalYear.Commands.CreateFiscalPeriod;

public class CreateFiscalPeriodCommand : IRequestCommand<Guid>
{
    public int Year { get; set; }
    public int Month { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}