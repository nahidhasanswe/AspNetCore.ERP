using ERP.Core;
using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.AccountsPayable.DTOs;
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
        var spendData = await invoiceRepository.GetSpendAnalysisListAsync(request.VendorId, null, null, request.StartDate, request.EndDate, request.ExpenseAccountId, cancellationToken);
        return Result.Success(spendData.AsEnumerable());
    }
}