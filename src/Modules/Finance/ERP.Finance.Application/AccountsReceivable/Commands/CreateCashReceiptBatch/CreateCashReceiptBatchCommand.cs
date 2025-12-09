using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CreateCashReceiptBatch;

public class CreateCashReceiptBatchCommand : IRequest<Result<Guid>>
{
    public DateTime BatchDate { get; set; }
    public Guid CashAccountId { get; set; }
    public string Reference { get; set; }
}