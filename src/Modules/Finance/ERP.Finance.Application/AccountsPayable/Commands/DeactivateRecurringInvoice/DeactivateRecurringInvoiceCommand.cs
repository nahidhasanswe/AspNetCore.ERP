using MediatR;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.DeactivateRecurringInvoice;

public record DeactivateRecurringInvoiceCommand(Guid RecurringInvoiceId) : IRequestCommand<Unit>;