using ERP.Core;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreateRecurringInvoice;

public class CreateRecurringInvoiceCommand : IRequest<Result<Guid>>
{
    public Guid VendorId { get; set; }
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