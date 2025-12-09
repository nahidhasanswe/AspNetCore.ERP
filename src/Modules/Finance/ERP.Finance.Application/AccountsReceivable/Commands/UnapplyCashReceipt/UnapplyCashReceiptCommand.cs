using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.UnapplyCashReceipt;

public class UnapplyCashReceiptCommand : IRequest<Result>
{
    public Guid CashReceiptId { get; set; }
    public Guid InvoiceId { get; set; }
    public Money AmountToUnapply { get; set; }
}