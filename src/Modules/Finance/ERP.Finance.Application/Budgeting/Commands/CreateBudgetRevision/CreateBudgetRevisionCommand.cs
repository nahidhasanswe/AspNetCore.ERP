using ERP.Core.Behaviors;

namespace ERP.Finance.Application.Budgeting.Commands.CreateBudgetRevision;

public class CreateBudgetRevisionCommand : IRequestCommand<Guid>
{
    public Guid BusinessUnitId { get; set; }
    public Guid OriginalBudgetId { get; set; }
    public string NewNameSuffix { get; set; } = " - Revision";
}