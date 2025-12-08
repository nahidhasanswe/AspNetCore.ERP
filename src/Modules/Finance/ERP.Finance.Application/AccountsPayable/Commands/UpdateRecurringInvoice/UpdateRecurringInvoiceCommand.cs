using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.UpdateRecurringInvoice;

public class UpdateRecurringInvoiceCommand : IRequestCommand<Unit>
{
    public Guid RecurringInvoiceId { get; set; }
    public RecurrenceInterval Interval { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<RecurringInvoiceLineDto> Lines { get; set; } = new();

    public class RecurringInvoiceLineDto
    {
        public string Description { get; set; }
        public Money LineAmount { get; set; }
        public Guid ExpenseAccountId { get; set; }
        public Guid? CostCenterId { get; set; }
    }
}