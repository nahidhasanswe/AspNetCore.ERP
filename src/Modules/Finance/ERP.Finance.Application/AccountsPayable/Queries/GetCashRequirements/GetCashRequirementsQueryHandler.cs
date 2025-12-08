using ERP.Core;
using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsPayable.Queries.GetCashRequirements;

public class GetCashRequirementsQueryHandler : IRequestHandler<GetCashRequirementsQuery, Result<IEnumerable<CashRequirementDto>>>
{
    private readonly IVendorInvoiceRepository _invoiceRepository;
    private readonly IVendorRepository _vendorRepository;

    public GetCashRequirementsQueryHandler(IVendorInvoiceRepository invoiceRepository, IVendorRepository vendorRepository)
    {
        _invoiceRepository = invoiceRepository;
        _vendorRepository = vendorRepository;
    }

    public async Task<Result<IEnumerable<CashRequirementDto>>> Handle(GetCashRequirementsQuery request, CancellationToken cancellationToken)
    {
        var invoicesDue = await _invoiceRepository.GetForecastProjectionAsync(request.DueDateCutoff);

        var cashRequirements = new List<CashRequirementDto>();

        foreach (var invoice in invoicesDue)
        {
            var vendorName = await _vendorRepository.GetNameByIdAsync(invoice.VendorId);

            cashRequirements.Add(new CashRequirementDto(
                invoice.Id,
                invoice.InvoiceNumber,
                invoice.VendorId,
                vendorName ?? "Unknown Vendor",
                invoice.DueDate,
                invoice.OutstandingBalance.Amount,
                invoice.OutstandingBalance.Currency,
                invoice.Status.ToString()
            ));
        }

        return Result.Success(cashRequirements.AsEnumerable());
    }
}