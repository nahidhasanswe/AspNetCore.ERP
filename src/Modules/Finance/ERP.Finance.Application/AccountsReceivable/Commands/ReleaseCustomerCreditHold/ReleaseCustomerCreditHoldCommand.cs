using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.ReleaseCustomerCreditHold;

public class ReleaseCustomerCreditHoldCommand : IRequest<Result>
{
    public Guid CustomerId { get; set; }
}