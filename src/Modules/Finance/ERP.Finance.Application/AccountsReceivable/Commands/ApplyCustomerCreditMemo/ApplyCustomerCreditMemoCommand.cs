using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.ApplyCustomerCreditMemo;

public class ApplyCustomerCreditMemoCommand : IRequest<Result>
{
    public Guid CreditMemoId { get; set; }
    public Guid InvoiceId { get; set; }
    public Money AmountToApply { get; set; }
}