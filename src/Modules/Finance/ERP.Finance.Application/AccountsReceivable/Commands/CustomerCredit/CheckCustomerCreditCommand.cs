using ERP.Core.Behaviors;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CustomerCredit;

public record CheckCustomerCreditCommand(
    Guid CustomerId,
    Guid SalesOrderId,
    Money OrderAmount // The value of the new order
) : IRequestCommand<bool>;