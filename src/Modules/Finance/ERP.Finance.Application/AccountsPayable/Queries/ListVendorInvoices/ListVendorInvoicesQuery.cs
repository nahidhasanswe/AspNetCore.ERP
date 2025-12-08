using ERP.Finance.Domain.Shared.Enums;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Queries.ListVendorInvoices;

public class ListVendorInvoicesQuery : IRequestCommand<IEnumerable<VendorInvoiceSummaryDto>>
{
    public Guid? VendorId { get; set; }
    public InvoiceStatus? Status { get; set; }
}

public record VendorInvoiceSummaryDto(
    Guid Id,
    string InvoiceNumber,
    DateTime InvoiceDate,
    DateTime DueDate,
    decimal TotalAmount,
    decimal OutstandingBalance,
    InvoiceStatus Status
);