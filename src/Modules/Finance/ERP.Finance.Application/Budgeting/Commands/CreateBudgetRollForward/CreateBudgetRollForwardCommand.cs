using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.Budgeting.Commands.CreateBudgetRollForward;

public class CreateBudgetRollForwardCommand : IRequest<Result<Guid>>
{
    public Guid SourceBudgetId { get; set; }
    public string NewFiscalPeriod { get; set; }
    public DateTime NewStartDate { get; set; }
    public DateTime NewEndDate { get; set; }
    public decimal AdjustmentFactor { get; set; } = 1.0m;
}