using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Finance.Domain.AccountsPayable.Aggregates;

namespace ERP.Finance.Application.AccountsPayable.Queries.GetVendorAging;

public class GetVendorAgingQueryHandler(IVendorInvoiceRepository invoiceRepository, IVendorRepository vendorRepository)
    : IRequestCommandHandler<GetVendorAgingQuery, IEnumerable<VendorAgingDto>>
{
    public async Task<Result<IEnumerable<VendorAgingDto>>> Handle(GetVendorAgingQuery request, CancellationToken cancellationToken)
    {
        var unpaidInvoices = await invoiceRepository.ListAllUnpaidAsync(request.AsOfDate, cancellationToken);
        if (!unpaidInvoices.Any())
        {
            return Result.Success(Enumerable.Empty<VendorAgingDto>());
        }
        
        var result = unpaidInvoices
            .GroupBy(i => new { i.VendorId, i.VendorName })
            .Select(g => new VendorAgingDto
            {
                VendorId = g.Key.VendorId,
                VendorName = g.Key.VendorName,
                Current = g.Sum(i => i.DaysOverdue < 0 ? i.OutstandingBalance : 0),
                Days1_30 = g.Sum(i => i.DaysOverdue >= 0 && i.DaysOverdue <= 30 ? i.OutstandingBalance : 0),
                Days31_60 = g.Sum(i => i.DaysOverdue > 30 && i.DaysOverdue <= 60 ? i.OutstandingBalance : 0),
                Days61_90 = g.Sum(i => i.DaysOverdue > 60 && i.DaysOverdue <= 90 ? i.OutstandingBalance : 0),
                Over90Days = g.Sum(i => i.DaysOverdue > 90 ? i.OutstandingBalance : 0)
            })
            .Where(x => x.TotalDue > 0)
            .AsEnumerable();
        
        return Result.Success(result);
    }
}