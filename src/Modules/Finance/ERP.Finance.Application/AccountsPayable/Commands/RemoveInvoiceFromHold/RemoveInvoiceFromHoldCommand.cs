using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.RemoveInvoiceFromHold;

public record RemoveInvoiceFromHoldCommand(Guid InvoiceId) : IRequestCommand<Guid>;