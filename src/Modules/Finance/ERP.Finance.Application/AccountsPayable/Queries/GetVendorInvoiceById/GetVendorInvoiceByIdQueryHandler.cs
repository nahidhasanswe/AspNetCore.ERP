using ERP.Core;
using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Queries.GetVendorInvoiceById;

public class GetVendorInvoiceByIdQueryHandler(IVendorInvoiceRepository invoiceRepository)
    : IRequestCommandHandler<GetVendorInvoiceByIdQuery, VendorInvoiceDetailsDto>
{
    public async Task<Result<VendorInvoiceDetailsDto>> Handle(GetVendorInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            return Result.Failure<VendorInvoiceDetailsDto>("Invoice not found.");
        }

        var lineItemsDto = invoice.LineItems.Select(li => new InvoiceLineItemDto(
            li.Id,
            li.Description,
            li.LineAmount,
            li.ExpenseAccountId,
            li.CostCenterId
        )).ToList();

        var invoiceDto = new VendorInvoiceDetailsDto(
            invoice.Id,
            invoice.VendorId,
            invoice.InvoiceNumber,
            invoice.InvoiceDate,
            invoice.DueDate,
            invoice.TotalAmount,
            invoice.OutstandingBalance,
            invoice.Status,
            invoice.IsOnHold,
            lineItemsDto
        );

        return Result.Success(invoiceDto);
    }
}