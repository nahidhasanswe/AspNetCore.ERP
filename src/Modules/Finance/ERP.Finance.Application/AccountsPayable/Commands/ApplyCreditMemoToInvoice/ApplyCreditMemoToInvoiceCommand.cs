using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.ApplyCreditMemoToInvoice;

public class ApplyCreditMemoToInvoiceCommand : IRequestCommand<Unit>
{
    public Guid InvoiceId { get; set; }
    public Guid CreditMemoId { get; set; }
    public Money AmountToApply { get; set; }
}