using ERP.Core;
using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Queries.GetVendorSpendAnalysis;

public class GetVendorSpendAnalysisQueryHandler(
    IVendorInvoiceRepository invoiceRepository,
    IVendorRepository vendorRepository)
    : IRequestHandler<GetVendorSpendAnalysisQuery, Result<IEnumerable<VendorSpendAnalysisDto>>>
{
    // private readonly IGLAccountRepository _glAccountRepository; // Assuming a GL account repository exists

    /*, IGLAccountRepository glAccountRepository */
    // _glAccountRepository = glAccountRepository;

    public async Task<Result<IEnumerable<VendorSpendAnalysisDto>>> Handle(GetVendorSpendAnalysisQuery request, CancellationToken cancellationToken)
    {
        var filteredInvoices = await invoiceRepository.GetAllListAsync(request.VendorId, null, null, request.StartDate, request.EndDate, cancellationToken);

        var spendData = new List<VendorSpendAnalysisDto>();

        // Group and aggregate spend
        var groupedSpend = filteredInvoices
            .SelectMany(invoice => invoice.LineItems.Select(line => new { invoice.VendorId, invoice.TotalAmount.Currency, line.ExpenseAccountId, line.LineAmount.Amount }))
            .Where(x => !request.ExpenseAccountId.HasValue || x.ExpenseAccountId == request.ExpenseAccountId.Value)
            .GroupBy(x => new { x.VendorId, x.ExpenseAccountId, x.Currency })
            .Select(g => new
            {
                g.Key.VendorId,
                g.Key.ExpenseAccountId,
                g.Key.Currency,
                TotalSpend = g.Sum(x => x.Amount)
            })
            .ToList();

        foreach (var item in groupedSpend)
        {
            var vendorName = await vendorRepository.GetNameByIdAsync(item.VendorId);
            // var expenseAccountName = await _glAccountRepository.GetAccountNameByIdAsync(item.ExpenseAccountId); // Placeholder

            spendData.Add(new VendorSpendAnalysisDto(
                item.VendorId,
                vendorName ?? "Unknown Vendor",
                item.ExpenseAccountId,
                "Unknown Expense Account", // Placeholder
                item.TotalSpend,
                item.Currency
            ));
        }

        return Result.Success(spendData.AsEnumerable());
    }
}