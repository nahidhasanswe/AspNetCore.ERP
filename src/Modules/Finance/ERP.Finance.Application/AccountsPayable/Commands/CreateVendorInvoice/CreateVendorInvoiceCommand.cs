using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreateVendorInvoice;

public class CreateVendorInvoiceCommand : IRequest<Result<Guid>>
{
    public Guid BusinessUnitId { get; set; } // New property
    public Guid VendorId { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string JurisdictionCode { get; set; } // Needed for Tax Management

    public Guid? CostCenterId { get; set; }

    public List<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();

    public class InvoiceLine
    {
        public string Description { get; set; }
        public Money LineAmount { get; set; }
        public Guid ExpenseAccountId { get; set; }
        public Guid? CostCenterId { get; set; }
    }
}