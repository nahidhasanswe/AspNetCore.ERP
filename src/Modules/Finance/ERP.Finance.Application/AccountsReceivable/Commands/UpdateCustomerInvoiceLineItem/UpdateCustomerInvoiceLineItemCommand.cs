using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.UpdateCustomerInvoiceLineItem;

public class UpdateCustomerInvoiceLineItemCommand : IRequest<Result>
{
    public Guid InvoiceId { get; set; }
    public Guid LineItemId { get; set; }
    public string Description { get; set; }
    public Money LineAmount { get; set; }
    public Guid RevenueAccountId { get; set; }
    public Guid? CostCenterId { get; set; }
}