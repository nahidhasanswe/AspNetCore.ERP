using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.PayVendorInvoice;

public record PayVendorInvoiceCommand(Guid InvoiceId, string PaymentReference, Money PaymentAmount, string PaymentMethodCode, DateTime PaymentDate) : IRequestCommand<bool>;