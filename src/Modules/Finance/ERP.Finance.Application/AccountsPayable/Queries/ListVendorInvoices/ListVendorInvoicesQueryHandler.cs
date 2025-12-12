using ERP.Core;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.AccountsPayable.DTOs;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Queries.ListVendorInvoices;

public class ListVendorInvoicesQueryHandler(IVendorInvoiceRepository invoiceRepository)
    : IRequestHandler<ListVendorInvoicesQuery, Result<IEnumerable<VendorInvoiceSummaryDto>>>
{
    public async Task<Result<IEnumerable<VendorInvoiceSummaryDto>>> Handle(ListVendorInvoicesQuery request, CancellationToken cancellationToken)
    {
        
        var summaryDtos = await invoiceRepository.GetInvoiceSummaryAsync(request.VendorId, request.Status, cancellationToken);
        return Result.Success(summaryDtos.AsEnumerable());
    }
}