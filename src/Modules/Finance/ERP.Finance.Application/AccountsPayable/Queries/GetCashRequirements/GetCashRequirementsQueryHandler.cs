using ERP.Core;
using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Queries.GetCashRequirements;

public class GetCashRequirementsQueryHandler(
    IVendorInvoiceRepository invoiceRepository)
    : IRequestHandler<GetCashRequirementsQuery, Result<IEnumerable<CashRequirementDto>>>
{
    public async Task<Result<IEnumerable<CashRequirementDto>>> Handle(GetCashRequirementsQuery request, CancellationToken cancellationToken)
    {
        var invoicesDue = await invoiceRepository.GetForecastProjectionAsync(request.DueDateCutoff, request.BusinessUnitId, cancellationToken);
        
        var result = invoicesDue.Select(invoice => new CashRequirementDto
        (
            invoice.Id,
            invoice.InvoiceNumber,
            invoice.VendorId,
            invoice.VendorName ?? "Unknown Vendor",
            invoice.DueDate,
            invoice.OutstandingBalance.Amount,
            invoice.OutstandingBalance.Currency,
            invoice.Status.ToString()));

        return Result.Success(result.AsEnumerable());
    }
}