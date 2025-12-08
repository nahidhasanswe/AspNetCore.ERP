using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Commands.CreateBudget;

public record CreateBudgetCommand(
    Guid BusinessUnitId,
    string FiscalPeriod,
    string Name,
    IEnumerable<BudgetLineItemDto> LineItems
) : IRequest<Result<Guid>>;

public record BudgetLineItemDto(Guid GlAccountId, Guid? CostCenterId, decimal Amount, string Currency);