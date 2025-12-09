using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.AddReceiptToBatch;

public class AddReceiptToBatchCommand : IRequest<Result>
{
    public Guid BatchId { get; set; }
    public Guid ReceiptId { get; set; }
}