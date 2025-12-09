using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Domain.Shared.Enums;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Queries.GetRevenueByCustomerReport;

public class GetRevenueByCustomerReportQueryHandler(
    ICustomerInvoiceRepository invoiceRepository,
    ICustomerRepository customerRepository)
    : IRequestHandler<GetRevenueByCustomerReportQuery, Result<IEnumerable<RevenueByCustomerDto>>>
{
    public async Task<Result<IEnumerable<RevenueByCustomerDto>>> Handle(GetRevenueByCustomerReportQuery request, CancellationToken cancellationToken)
    {
        var allInvoices = await invoiceRepository.ListAllAsync(cancellationToken);

        var issuedInvoices = allInvoices.Where(inv => inv.Status == InvoiceStatus.Issued || inv.Status == InvoiceStatus.Paid || inv.Status == InvoiceStatus.PartiallyPaid); // Consider all invoices that contributed to revenue

        if (request.StartDate.HasValue)
        {
            issuedInvoices = issuedInvoices.Where(inv => inv.IssueDate >= request.StartDate.Value);
        }
        if (request.EndDate.HasValue)
        {
            issuedInvoices = issuedInvoices.Where(inv => inv.IssueDate <= request.EndDate.Value);
        }

        var revenueData = new List<RevenueByCustomerDto>();

        var groupedByCustomer = issuedInvoices
            .GroupBy(inv => new { inv.CustomerId, inv.TotalAmount.Currency })
            .Select(g => new
            {
                g.Key.CustomerId,
                g.Key.Currency,
                TotalRevenue = g.Sum(inv => inv.TotalAmount.Amount)
            })
            .ToList();

        foreach (var item in groupedByCustomer)
        {
            var customerName = await customerRepository.GetNameByIdAsync(item.CustomerId, cancellationToken);
            revenueData.Add(new RevenueByCustomerDto(
                item.CustomerId,
                customerName ?? "Unknown Customer",
                item.TotalRevenue,
                item.Currency
            ));
        }

        return Result.Success(revenueData.AsEnumerable());
    }
}