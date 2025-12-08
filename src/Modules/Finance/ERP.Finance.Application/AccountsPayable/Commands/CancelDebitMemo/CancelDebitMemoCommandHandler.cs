using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.CancelDebitMemo;

public class CancelDebitMemoCommandHandler(IDebitMemoRepository debitMemoRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CancelDebitMemoCommand, Unit>
{
    public async Task<Result<Unit>> Handle(CancelDebitMemoCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var debitMemo = await debitMemoRepository.GetByIdAsync(command.DebitMemoId, cancellationToken);
        if (debitMemo == null)
        {
            return Result.Failure<Unit>("Debit Memo not found.");
        }

        debitMemo.Cancel(command.Reason);

        await debitMemoRepository.UpdateAsync(debitMemo, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}