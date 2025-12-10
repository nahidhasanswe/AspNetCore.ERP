using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CreateCashReceipt;

public class CreateCashReceiptCommand : IRequest<Result<Guid>>
{
    public Guid BusinessUnitId { get; set; } // New property
    public Guid CustomerId { get; set; }
    public DateTime ReceiptDate { get; set; }
    public Money ReceivedAmount { get; set; }
    public string TransactionReference { get; set; }
    public Guid CashAccountId { get; set; } // The GL account that was DEBITED (Cash in Bank)
}