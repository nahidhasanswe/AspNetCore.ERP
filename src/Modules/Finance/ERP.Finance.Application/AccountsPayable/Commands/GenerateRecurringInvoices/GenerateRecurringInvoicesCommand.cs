using ERP.Core.Behaviors;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.GenerateRecurringInvoices;

public record GenerateRecurringInvoicesCommand() : IRequestCommand<Unit>;