using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.Budgeting.Commands.TransferBudgetFunds;

public class TransferBudgetFundsCommand : IRequest<Result>
{
    public Guid BudgetId { get; set; }
    public Guid FromBudgetItemId { get; set; }
    public Guid ToBudgetItemId { get; set; }
    public Money Amount { get; set; }
}