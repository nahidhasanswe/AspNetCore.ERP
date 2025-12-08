using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.PlaceInvoiceOnHold;

public record PlaceInvoiceOnHoldCommand(Guid InvoiceId) : IRequestCommand<bool>;