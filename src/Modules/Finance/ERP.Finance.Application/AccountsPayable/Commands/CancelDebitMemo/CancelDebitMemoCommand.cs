using MediatR;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.CancelDebitMemo;

public class CancelDebitMemoCommand : IRequestCommand<Unit>
{
    public Guid DebitMemoId { get; set; }
    public string Reason { get; set; }
}