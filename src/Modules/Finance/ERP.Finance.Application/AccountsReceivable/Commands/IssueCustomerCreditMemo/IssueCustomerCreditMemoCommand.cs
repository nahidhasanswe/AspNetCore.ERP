using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.IssueCustomerCreditMemo;

public class IssueCustomerCreditMemoCommand : IRequest<Result<Guid>>
{
    public Guid CustomerId { get; set; }
    public Money Amount { get; set; }
    public DateTime MemoDate { get; set; }
    public string Reason { get; set; }
    public Guid ARControlAccountId { get; set; } // GL Account for AR Control
    public Guid RevenueAccountId { get; set; } // GL Account for Revenue Adjustment
}