using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.RefundOverpayment;

public class RefundOverpaymentCommand : IRequest<Result>
{
    public Guid CashReceiptId { get; set; }
    public Money RefundAmount { get; set; }
    public Guid RefundCashAccountId { get; set; } // The GL account from which the refund is made
    public string RefundReference { get; set; }
}