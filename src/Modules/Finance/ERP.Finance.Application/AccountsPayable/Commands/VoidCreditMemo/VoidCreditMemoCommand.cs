using MediatR;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.VoidCreditMemo;

public class VoidCreditMemoCommand : IRequestCommand<Unit>
{
    public Guid CreditMemoId { get; set; }
    public string Reason { get; set; }
}