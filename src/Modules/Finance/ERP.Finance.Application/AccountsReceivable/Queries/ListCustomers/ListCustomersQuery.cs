using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using ERP.Finance.Domain.AccountsReceivable.Enums;
using MediatR;
using System.Collections.Generic;

namespace ERP.Finance.Application.AccountsReceivable.Queries.ListCustomers;

public class ListCustomersQuery : IRequest<Result<IEnumerable<CustomerSummaryDto>>>
{
    public string? Name { get; set; }
    public CustomerStatus? Status { get; set; }
}