using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.AccountsPayable.Service;

namespace ERP.Finance.Application.AccountsPayable.Queries.AccountPayable;

public class GetAccountsPayableAgingQueryHandler(
    IVendorInvoiceRepository repository,
    IVendorLookupService vendorServivce
    ) : IRequestCommandHandler<GetAccountsPayableAgingQuery, List<VendorAgingDto>>
{
    // In a real system, this would query a READ MODEL/optimized SQL view
    // to avoid complex calculations on the write-heavy VendorInvoice aggregate.

    public async Task<Result<List<VendorAgingDto>>> Handle(GetAccountsPayableAgingQuery query, CancellationToken cancellationToken)
    {
        var asOfDate = query.AsOfDate.Date;
        
        // 1. Fetch all invoices with an outstanding balance
        // We assume the repository returns a projection or the aggregate root itself.
        // For performance, this should be a Read Model optimized list: List<InvoiceAgingProjection>
        var openInvoices = await repository.ListAllUnpaidAsync(); 

        if (!openInvoices.Any())
        {
            return Result.Success(new List<VendorAgingDto>());
        }

        // 2. Group by Vendor and calculate aging buckets
        var vendorGroups = openInvoices
            .Where(i => i.InvoiceDate.Date <= asOfDate) // Only include invoices created up to the asOfDate
            .GroupBy(i => i.VendorId);
        
        var results = new List<VendorAgingDto>();
        
        // 3. Process each vendor group
        foreach (var group in vendorGroups)
        {
            var dto = new VendorAgingDto { VendorId = group.Key };

            // Fetch name concurrently (or via a pre-loaded dictionary)
            dto.VendorName = await vendorServivce.GetVendorNameAsync(group.Key); 

            // Calculate the outstanding balance for each invoice and slot it into a bucket
            foreach (var invoice in group)
            {
                var outstanding = invoice.OutstandingBalance.Amount;
                var daysOverdue = (int)(asOfDate - invoice.DueDate.Date).TotalDays;

                if (daysOverdue < 0)
                {
                    // Not yet due (Current)
                    dto.Current += outstanding;
                }
                else if (daysOverdue >= 0 && daysOverdue <= 30)
                {
                    // 1 - 30 days overdue (including 0 days overdue if DueDate is today)
                    dto.Days1_30 += outstanding;
                }
                else if (daysOverdue > 30 && daysOverdue <= 60)
                {
                    // 31 - 60 days overdue
                    dto.Days31_60 += outstanding;
                }
                else if (daysOverdue > 60 && daysOverdue <= 90)
                {
                    // 61 - 90 days overdue
                    dto.Days61_90 += outstanding;
                }
                else // daysOverdue > 90
                {
                    // Over 90 days overdue
                    dto.Over90Days += outstanding;
                }
            }
            
            // TotalDue property is automatically calculated by the DTO's getter.
            results.Add(dto);
        }
            
        return Result.Success(results.Where(r => r.TotalDue > 0).ToList());
    }
}