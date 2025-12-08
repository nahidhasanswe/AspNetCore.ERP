using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Commands.RecordPayment;

public record RecordPaymentCommand(Guid InvoiceId, decimal Amount, string Currency, string CashMethodCode, DateTime PaymentDate, string Reference) : IRequest<Result<bool>>;