using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CancelCashReceipt;

public class CancelCashReceiptCommand : IRequest<Result>
{
    public Guid CashReceiptId { get; set; }
}