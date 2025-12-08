using ERP.Core.Behaviors;
using ERP.Finance.Domain.FiscalYear.Enums;

namespace ERP.Finance.Application.FiscalYear.Commands.CloseFiscalPeriod;

public class CloseFiscalPeriodCommand : IRequestCommand<bool>
{
    public Guid PeriodId { get; set; }
    public PeriodStatus TargetStatus { get; set; } // Must be SoftClose or HardClose
}