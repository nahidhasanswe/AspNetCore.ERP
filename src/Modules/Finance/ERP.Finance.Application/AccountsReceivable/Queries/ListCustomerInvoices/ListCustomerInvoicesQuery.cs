using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using ERP.Finance.Domain.Shared.Enums;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.AccountsReceivable.Queries.ListCustomerInvoices;

public class ListCustomerInvoicesQuery : IRequest<Result<IEnumerable<CustomerInvoiceSummaryDto>>>
{
    public Guid? CustomerId { get; set; }
    public InvoiceStatus? Status { get; set; }
    public DateTime? StartIssueDate { get; set; }
    public DateTime? EndIssueDate { get; set; }
}