using ERP.Core;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Queries.ListVendorInvoices;

public class ListVendorInvoicesQueryHandler(IVendorInvoiceRepository invoiceRepository)
    : IRequestHandler<ListVendorInvoicesQuery, Result<IEnumerable<VendorInvoiceSummaryDto>>>
{
    public async Task<Result<IEnumerable<VendorInvoiceSummaryDto>>> Handle(ListVendorInvoicesQuery request, CancellationToken cancellationToken)
    {
        var allInvoices = await invoiceRepository.GetAllListAsync(request.VendorId, request.Status, null, cancellationToken);
        
        var summaryDtos = allInvoices.Select(inv => new VendorInvoiceSummaryDto(
            inv.Id,
            inv.InvoiceNumber,
            inv.InvoiceDate,
            inv.DueDate,
            inv.TotalAmount.Amount,
            inv.OutstandingBalance.Amount,
            inv.Status
        )).ToList();

        return Result.Success(summaryDtos.AsEnumerable());
    }
}