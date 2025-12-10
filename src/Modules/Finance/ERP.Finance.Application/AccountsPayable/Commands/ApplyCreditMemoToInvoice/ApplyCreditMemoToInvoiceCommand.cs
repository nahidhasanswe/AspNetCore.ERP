using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.ApplyCreditMemoToInvoice;

public class ApplyCreditMemoToInvoiceCommand : IRequest<Result>
{
    public Guid BusinessUnitId { get; set; } // New property
    public Guid InvoiceId { get; set; }
    public Guid CreditMemoId { get; set; }
    public Money AmountToApply { get; set; }
}