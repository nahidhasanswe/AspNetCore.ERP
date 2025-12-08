using ERP.Core;
using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Queries.GetVendorSpendAnalysis;

public class GetVendorSpendAnalysisQueryHandler(
    IVendorInvoiceRepository invoiceRepository,
    IVendorRepository vendorRepository,
    IAccountRepository accountRepository)
    : IRequestHandler<GetVendorSpendAnalysisQuery, Result<IEnumerable<VendorSpendAnalysisDto>>>
{
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
            var expenseAccountName = await accountRepository.GetAccountNameAsync(item.ExpenseAccountId, cancellationToken);

            spendData.Add(new VendorSpendAnalysisDto(
                item.VendorId,
                vendorName ?? "Unknown Vendor",
                item.ExpenseAccountId,
                expenseAccountName ?? "Unknown Expense Account",
                item.TotalSpend,
                item.Currency
            ));
        }

        return Result.Success(spendData.AsEnumerable());
    }
}