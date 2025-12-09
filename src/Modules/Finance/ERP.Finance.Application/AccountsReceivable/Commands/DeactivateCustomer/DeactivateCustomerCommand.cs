using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Commands.DeactivateCustomer;

public class DeactivateCustomerCommand : IRequest<Result>
{
    public Guid CustomerId { get; set; }
}