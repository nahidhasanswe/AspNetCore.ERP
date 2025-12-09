using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CreateCustomerInvoice;

public class CreateCustomerInvoiceCommand : IRequest<Result<Guid>>
{
    public Guid CustomerId { get; set; }
    public string InvoiceNumber { get; set; }
    public Guid ARControlAccountId { get; set; } // GL Account for Accounts Receivable
    public DateTime DueDate { get; set; }
    public Guid? CostCenterId { get; set; }
    public List<CustomerInvoiceLineItemDto> LineItems { get; set; } = new();

    public class CustomerInvoiceLineItemDto
    {
        public string Description { get; set; }
        public Money LineAmount { get; set; }
        public Guid RevenueAccountId { get; set; } // GL Account for Revenue
        public Guid? CostCenterId { get; set; }
    }
}