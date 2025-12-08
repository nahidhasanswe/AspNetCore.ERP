using ERP.Core.Behaviors;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.AccountsReceivable.Commands.Deduction;

public record ApplyDeductionCommand(
    Guid InvoiceId,
    Money DeductionAmount, // The amount being short-paid/deducted
    string DeductionReasonCode, // e.g., 'DMG' (Damaged Goods), 'PRC' (Pricing Error)
    Guid UserId // User authorizing the deduction
) : IRequestCommand<bool>;