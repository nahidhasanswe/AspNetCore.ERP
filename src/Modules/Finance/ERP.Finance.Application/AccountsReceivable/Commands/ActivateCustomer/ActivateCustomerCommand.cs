using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.ActivateCustomer;

public class ActivateCustomerCommand : IRequest<Result>
{
    public Guid CustomerId { get; set; }
}