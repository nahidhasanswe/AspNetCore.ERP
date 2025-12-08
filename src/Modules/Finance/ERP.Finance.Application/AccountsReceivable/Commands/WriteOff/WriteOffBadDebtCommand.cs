using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsReceivable.Commands.WriteOff;

public record WriteOffBadDebtCommand(
    Guid InvoiceId,
    Guid UserId, // User authorizing the write-off
    DateTime WriteOffDate,
    string Reason // e.g., "Bankruptcy," "Statute of limitations"
) : IRequestCommand<bool>;