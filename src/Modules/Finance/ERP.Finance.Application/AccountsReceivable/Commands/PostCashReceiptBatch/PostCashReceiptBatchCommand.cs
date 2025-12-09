using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.PostCashReceiptBatch;

public class PostCashReceiptBatchCommand : IRequest<Result>
{
    public Guid BatchId { get; set; }
}