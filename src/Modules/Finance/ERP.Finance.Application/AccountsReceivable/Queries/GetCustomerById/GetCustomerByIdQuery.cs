using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Queries.GetCustomerById;

public class GetCustomerByIdQuery : IRequest<Result<CustomerDetailsDto>>
{
    public Guid CustomerId { get; set; }
}