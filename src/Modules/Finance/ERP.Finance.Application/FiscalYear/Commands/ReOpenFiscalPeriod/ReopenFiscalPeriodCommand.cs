using ERP.Core.Behaviors;

namespace ERP.Finance.Application.FiscalYear.Commands.ReOpenFiscalPeriod;

public class ReopenFiscalPeriodCommand : IRequestCommand<bool>
{
    public Guid PeriodId { get; set; }
}