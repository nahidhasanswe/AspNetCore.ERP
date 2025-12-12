using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Finance.Domain.AccountsPayable.Aggregates;

namespace ERP.Finance.Application.AccountsPayable.Queries.AccountPayable;

public class GetAccountsPayableAgingQueryHandler(
    IVendorInvoiceRepository repository
    ) : IRequestCommandHandler<GetAccountsPayableAgingQuery, List<VendorAgingDto>>
{
    public async Task<Result<List<VendorAgingDto>>> Handle(GetAccountsPayableAgingQuery query, CancellationToken cancellationToken)
    {
        var asOfDate = query.AsOfDate.Date;
        
        var unpaidInvoices = await repository.ListAllUnpaidAsync(asOfDate, cancellationToken);
        if (!unpaidInvoices.Any())
        {
            return Result.Success(new List<VendorAgingDto>());
        }
        
        // TODO: Convert all the calculation into database side
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
            .ToList();
        
        return Result.Success(result);
    }
}