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
        var unpaidInvoices = await invoiceRepository.ListAllUnpaidAsync();
        if (!unpaidInvoices.Any())
        {
            return Result.Success(Enumerable.Empty<VendorAgingDto>());
        }

        var today = DateTime.UtcNow.Date;

        var agingData = unpaidInvoices
            .GroupBy(invoice => invoice.VendorId)
            .Select(async vendorGroup =>
            {
                var vendorName = await vendorRepository.GetNameByIdAsync(vendorGroup.Key);
                var totalDue = vendorGroup.Sum(inv => inv.OutstandingBalance.Amount);

                decimal current = 0;
                decimal days1_30 = 0;
                decimal days31_60 = 0;
                decimal days61_90 = 0;
                decimal over90 = 0;

                foreach (var invoice in vendorGroup)
                {
                    var overdueDays = (today - invoice.DueDate.Date).Days;
                    var balance = invoice.OutstandingBalance.Amount;

                    if (overdueDays <= 0)
                        current += balance;
                    else if (overdueDays <= 30)
                        days1_30 += balance;
                    else if (overdueDays <= 60)
                        days31_60 += balance;
                    else if (overdueDays <= 90)
                        days61_90 += balance;
                    else
                        over90 += balance;
                }

                return new VendorAgingDto{
                    VendorId= vendorGroup.Key,
                    VendorName = vendorName ?? "Unknown Vendor",
                    Current = current,
                    Days1_30 = days1_30,
                    Days31_60 = days31_60,
                    Days61_90 = days61_90,
                    Over90Days = over90
                };
            });

        var results = await Task.WhenAll(agingData);
        return Result.Success(results.AsEnumerable());
    }
}