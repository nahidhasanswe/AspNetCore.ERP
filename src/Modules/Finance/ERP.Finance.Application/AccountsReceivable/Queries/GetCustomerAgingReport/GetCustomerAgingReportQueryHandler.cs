using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Queries.GetCustomerAgingReport;

public class GetCustomerAgingReportQueryHandler(
    ICustomerInvoiceRepository invoiceRepository,
    ICustomerRepository customerRepository)
    : IRequestHandler<GetCustomerAgingReportQuery, Result<IEnumerable<CustomerAgingDto>>>
{
    public async Task<Result<IEnumerable<CustomerAgingDto>>> Handle(GetCustomerAgingReportQuery request, CancellationToken cancellationToken)
    {
        // Fetch all invoices that are not fully paid or written off
        var allInvoices = await invoiceRepository.ListAllAsync(cancellationToken);
        var outstandingInvoices = allInvoices.Where(inv => inv.OutstandingBalance.Amount > 0).ToList();

        var agingData = new List<CustomerAgingDto>();

        var groupedByCustomer = outstandingInvoices.GroupBy(inv => inv.CustomerId);

        foreach (var customerGroup in groupedByCustomer)
        {
            var customerId = customerGroup.Key;
            var customerName = await customerRepository.GetNameByIdAsync(customerId, cancellationToken);

            decimal current = 0;        // Not yet due
            decimal days1_30 = 0;       // 1-30 days overdue
            decimal days31_60 = 0;      // 31-60 days overdue
            decimal days61_90 = 0;      // 61-90 days overdue
            decimal over90Days = 0;     // Over 90 days overdue

            foreach (var invoice in customerGroup)
            {
                var daysOverdue = (request.AsOfDate.Date - invoice.DueDate.Date).Days;
                var outstandingAmount = invoice.OutstandingBalance.Amount;

                if (daysOverdue <= 0) // Not yet due or due today
                {
                    current += outstandingAmount;
                }
                else if (daysOverdue <= 30) // 1-30 days overdue
                {
                    days1_30 += outstandingAmount;
                }
                else if (daysOverdue <= 60) // 31-60 days overdue
                {
                    days31_60 += outstandingAmount;
                }
                else if (daysOverdue <= 90) // 61-90 days overdue
                {
                    days61_90 += outstandingAmount;
                }
                else // Over 90 days overdue
                {
                    over90Days += outstandingAmount;
                }
            }

            agingData.Add(new CustomerAgingDto(
                customerId,
                customerName ?? "Unknown Customer",
                current,
                days1_30,
                days31_60,
                days61_90,
                over90Days
            ));
        }

        return Result.Success(agingData.AsEnumerable());
    }
}