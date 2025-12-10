using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.UpdateCashReceipt;

public class UpdateCashReceiptCommand : IRequest<Result>
{
    public Guid BusinessUnitId { get; set; } // New property
    public Guid CashReceiptId { get; set; }
    public DateTime NewReceiptDate { get; set; }
    public Money NewReceivedAmount { get; set; }
    public string NewTransactionReference { get; set; }
    public Guid NewCashAccountId { get; set; }
}