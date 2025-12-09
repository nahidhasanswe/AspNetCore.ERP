using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.PlaceCustomerCreditHold;

public class PlaceCustomerCreditHoldCommand : IRequest<Result>
{
    public Guid CustomerId { get; set; }
    public string Reason { get; set; }
}