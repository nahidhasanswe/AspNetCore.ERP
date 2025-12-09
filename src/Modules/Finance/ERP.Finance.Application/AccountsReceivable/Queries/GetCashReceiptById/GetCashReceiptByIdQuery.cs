using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Queries.GetCashReceiptById;

public class GetCashReceiptByIdQuery : IRequest<Result<CashReceiptDetailsDto>>
{
    public Guid ReceiptId { get; set; }
}