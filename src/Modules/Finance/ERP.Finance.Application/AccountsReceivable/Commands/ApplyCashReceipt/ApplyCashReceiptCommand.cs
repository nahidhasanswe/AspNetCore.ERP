using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.ApplyCashReceipt;

public class ApplyCashReceiptCommand : IRequest<Result>
{
    public Guid CashReceiptId { get; set; }
    public Guid InvoiceId { get; set; }
    public Money AmountToApply { get; set; }
}