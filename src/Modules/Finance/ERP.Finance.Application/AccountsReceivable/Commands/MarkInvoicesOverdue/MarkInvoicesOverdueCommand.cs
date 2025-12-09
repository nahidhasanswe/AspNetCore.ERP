using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.MarkInvoicesOverdue;

public class MarkInvoicesOverdueCommand : IRequest<Result>
{
    public DateTime AsOfDate { get; set; } = DateTime.UtcNow;
}