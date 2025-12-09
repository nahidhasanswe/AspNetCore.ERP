using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Queries.ListCustomerInvoices;

public class ListCustomerInvoicesQueryHandler(
    ICustomerInvoiceRepository invoiceRepository,
    ICustomerRepository customerRepository)
    : IRequestHandler<ListCustomerInvoicesQuery, Result<IEnumerable<CustomerInvoiceSummaryDto>>>
{
    public async Task<Result<IEnumerable<CustomerInvoiceSummaryDto>>> Handle(ListCustomerInvoicesQuery request, CancellationToken cancellationToken)
    {
        var allInvoices = await invoiceRepository.ListAllAsync(cancellationToken);

        var filteredInvoices = allInvoices.AsQueryable();

        if (request.CustomerId.HasValue)
        {
            filteredInvoices = filteredInvoices.Where(inv => inv.CustomerId == request.CustomerId.Value);
        }

        if (request.Status.HasValue)
        {
            filteredInvoices = filteredInvoices.Where(inv => inv.Status == request.Status.Value);
        }

        if (request.StartIssueDate.HasValue)
        {
            filteredInvoices = filteredInvoices.Where(inv => inv.IssueDate >= request.StartIssueDate.Value);
        }

        if (request.EndIssueDate.HasValue)
        {
            filteredInvoices = filteredInvoices.Where(inv => inv.IssueDate <= request.EndIssueDate.Value);
        }

        var summaryDtos = new List<CustomerInvoiceSummaryDto>();
        foreach (var inv in filteredInvoices)
        {
            var customerName = await customerRepository.GetNameByIdAsync(inv.CustomerId, cancellationToken);
            summaryDtos.Add(new CustomerInvoiceSummaryDto(
                inv.Id,
                inv.CustomerId,
                customerName ?? "Unknown Customer",
                inv.InvoiceNumber,
                inv.IssueDate,
                inv.DueDate,
                inv.TotalAmount,
                inv.OutstandingBalance,
                inv.Status
            ));
        }

        return Result.Success(summaryDtos.AsEnumerable());
    }
}