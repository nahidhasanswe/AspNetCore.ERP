using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;

namespace ERP.Finance.Application.Encumbrance.Commands.CreateEncumbrance;

public class CreateEncumbranceCommand : IRequest<Result<Guid>>
{
    public Guid SourceTransactionId { get; set; } // e.g., Purchase Requisition ID
    public Guid BusinessUnitId { get; set; } // Needed to find the correct budget
    public Guid GlAccountId { get; set; }
    public Guid? CostCenterId { get; set; }
    public Money Amount { get; set; }
    public string BudgetPeriod { get; set; } // e.g., "JAN-2026", "Q1-2026"
}