using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.ApplyDebitMemo;

public class ApplyDebitMemoCommand : IRequest<Result>
{
    public Guid BusinessUnitId { get; set; }
    public Guid DebitMemoId { get; set; }
}