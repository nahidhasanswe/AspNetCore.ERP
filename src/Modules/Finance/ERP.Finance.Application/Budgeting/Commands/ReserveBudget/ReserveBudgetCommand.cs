using ERP.Core.Behaviors;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.Budgeting.Commands.ReserveBudget;

public record ReserveBudgetCommand(
    Guid SourceTransactionId, // e.g., Purchase Requisition ID
    Money Amount,
    Guid GlAccountId,
    Guid? CostCenterId
) : IRequestCommand<Guid>;