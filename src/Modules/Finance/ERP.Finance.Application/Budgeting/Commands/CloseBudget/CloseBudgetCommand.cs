using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.Budgeting.Commands.CloseBudget;

public class CloseBudgetCommand : IRequest<Result>
{
    public Guid BudgetId { get; set; }
}