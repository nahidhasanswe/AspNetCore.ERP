using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.AccountsPayable.Commands.RecordVendorPayment;

public class RecordVendorPaymentCommand : IRequest<Result<bool>>
{
    public Guid InvoiceId { get; set; }
    public Money PaymentAmount { get; set; }
    public string TransactionReference { get; set; }
    public DateTime PaymentDate { get; set; }
    public Guid PaymentAccountId { get; set; } // The GL account cash is paid from (e.g., Bank Account)
}