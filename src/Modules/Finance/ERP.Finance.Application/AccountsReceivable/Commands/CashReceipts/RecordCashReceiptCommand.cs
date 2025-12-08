using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CashReceipts;

public record RecordCashReceiptCommand(
    Guid CustomerId,
    decimal Amount,
    string Currency,
    DateTime ReceiptDate,
    string TransactionReference,
    string CashMethodCode // Used to determine the receiving GL account
) : IRequestCommand<Guid>;