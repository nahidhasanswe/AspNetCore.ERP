using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CreateCustomer;

public class CreateCustomerCommand : IRequest<Result<Guid>>
{
    public string Name { get; set; }
    public string ContactEmail { get; set; }
}