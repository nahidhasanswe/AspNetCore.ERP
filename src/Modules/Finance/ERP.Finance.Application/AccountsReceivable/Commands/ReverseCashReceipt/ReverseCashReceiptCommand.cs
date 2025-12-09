using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.ReverseCashReceipt;

public class ReverseCashReceiptCommand : IRequest<Result>
{
    public Guid CashReceiptId { get; set; }
    public string Reason { get; set; }
}