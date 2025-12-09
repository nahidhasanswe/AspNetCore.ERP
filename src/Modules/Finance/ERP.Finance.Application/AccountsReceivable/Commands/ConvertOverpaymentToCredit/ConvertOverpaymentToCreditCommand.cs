using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.ConvertOverpaymentToCredit;

public class ConvertOverpaymentToCreditCommand : IRequest<Result<Guid>>
{
    public Guid CashReceiptId { get; set; }
    public Money AmountToConvert { get; set; }
    public string Reason { get; set; }
    public Guid ARControlAccountId { get; set; } // GL Account for AR Control
    public Guid RevenueAdjustmentAccountId { get; set; } // GL Account for Revenue Adjustment
}