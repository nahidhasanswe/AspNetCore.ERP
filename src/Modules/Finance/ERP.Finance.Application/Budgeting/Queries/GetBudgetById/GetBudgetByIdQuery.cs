using ERP.Core;
using ERP.Finance.Application.Budgeting.DTOs;
using MediatR;
using System;

namespace ERP.Finance.Application.Budgeting.Queries.GetBudgetById;

public class GetBudgetByIdQuery : IRequest<Result<BudgetDetailsDto>>
{
    public Guid BudgetId { get; set; }
}