using ERP.Finance.Domain.Shared.Enums;
using ERP.Core.Behaviors;
using ERP.Finance.Domain.AccountsPayable.DTOs;

namespace ERP.Finance.Application.AccountsPayable.Queries.ListVendorInvoices;

public class ListVendorInvoicesQuery : IRequestCommand<IEnumerable<VendorInvoiceSummaryDto>>
{
    public Guid? VendorId { get; set; }
    public InvoiceStatus? Status { get; set; }
}
