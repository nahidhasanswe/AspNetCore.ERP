using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Queries.GetCustomerStatement;

public class GetCustomerStatementQuery : IRequest<Result<CustomerStatementDto>>
{
    public Guid CustomerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}