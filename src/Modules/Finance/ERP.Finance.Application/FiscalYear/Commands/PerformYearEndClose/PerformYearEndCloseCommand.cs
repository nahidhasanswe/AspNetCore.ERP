using ERP.Core.Behaviors;

namespace ERP.Finance.Application.FiscalYear.Commands.PerformYearEndClose;

public record PerformYearEndCloseCommand(Guid BusinessUnitId, Guid FiscalPeriodToCloseId, Guid RetainedEarningsAccountId) 
    : IRequestCommand<Guid>;