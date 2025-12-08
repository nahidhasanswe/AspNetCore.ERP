using MediatR;
using ERP.Core.Behaviors;

namespace ERP.Finance.Application.AccountsPayable.Commands.ApplyDebitMemo;

public record ApplyDebitMemoCommand(Guid DebitMemoId) : IRequestCommand<Unit>;
