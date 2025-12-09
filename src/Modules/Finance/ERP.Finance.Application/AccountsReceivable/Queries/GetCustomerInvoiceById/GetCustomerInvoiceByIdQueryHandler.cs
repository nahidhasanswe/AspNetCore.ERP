using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Queries.GetCustomerInvoiceById;

public class GetCustomerInvoiceByIdQueryHandler(
    ICustomerInvoiceRepository invoiceRepository,
    ICustomerRepository customerRepository,
    IAccountRepository glAccountRepository)
    : IRequestHandler<GetCustomerInvoiceByIdQuery, Result<CustomerInvoiceDetailsDto>>
{
    public async Task<Result<CustomerInvoiceDetailsDto>> Handle(GetCustomerInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            return Result.Failure<CustomerInvoiceDetailsDto>("Customer Invoice not found.");
        }

        var customerName = await customerRepository.GetNameByIdAsync(invoice.CustomerId, cancellationToken); // Assuming GetNameByIdAsync exists

        var lineItemDetailsTasks = invoice.LineItems.Select(async item =>
        {
            var revenueAccountName = await glAccountRepository.GetAccountNameAsync(item.RevenueAccountId, cancellationToken);
            return new CustomerInvoiceLineItemDto(
                item.Id,
                item.Description,
                item.LineAmount,
                item.RevenueAccountId,
                revenueAccountName ?? "Unknown Revenue Account",
                item.CostCenterId
            );
        }).ToList();

        var lineItemDetails = await Task.WhenAll(lineItemDetailsTasks);

        var invoiceDetails = new CustomerInvoiceDetailsDto(
            invoice.Id,
            invoice.CustomerId,
            customerName ?? "Unknown Customer",
            invoice.IssueDate,
            invoice.DueDate,
            invoice.TotalAmount,
            invoice.OutstandingBalance,
            invoice.Status,
            invoice.InvoiceNumber,
            invoice.ARControlAccountId,
            invoice.CostCenterId,
            invoice.TotalPaymentsReceived,
            invoice.TotalAmountWrittenOff,
            invoice.TotalCreditsApplied,
            lineItemDetails
        );

        return Result.Success(invoiceDetails);
    }
}