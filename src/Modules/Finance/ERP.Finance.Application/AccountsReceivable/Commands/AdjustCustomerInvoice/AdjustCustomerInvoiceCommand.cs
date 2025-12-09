using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.AdjustCustomerInvoice;

public class AdjustCustomerInvoiceCommand : IRequest<Result>
{
    public Guid InvoiceId { get; set; }
    public Money AdjustmentAmount { get; set; } // Positive for increase, negative for decrease
    public string Reason { get; set; }
    public Guid AdjustmentAccountId { get; set; } // The GL account for the adjustment
    public Guid? CostCenterId { get; set; }
}