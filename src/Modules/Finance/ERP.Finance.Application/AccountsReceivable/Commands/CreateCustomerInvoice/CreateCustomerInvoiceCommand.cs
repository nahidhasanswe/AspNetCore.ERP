using ERP.Core;
using MediatR;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CreateCustomerInvoice;

public record CreateCustomerInvoiceCommand(
    Guid CustomerId,
    // CRITICAL: Invoice must be broken down by line item, not just total amount
    IEnumerable<InvoiceLineItemDto> LineItems, 
    string InvoiceNumber,
    DateTime InvoiceDate,
    DateTime DueDate,
    // CRITICAL: The GL account ID where the asset (AR) will be posted
    Guid ARControlAccountId 
) : IRequest<Result<Guid>>;

public record InvoiceLineItemDto(
    Money LineAmount, // Includes Currency
    string Description,
    Guid RevenueAccountId, // The specific GL revenue account for this item
    Guid? CostCenterId
);