using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.ApplyDebitMemo;

public class ApplyDebitMemoCommandHandler(IDebitMemoRepository debitMemoRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<ApplyDebitMemoCommand, Result>
{
    public async Task<Result> Handle(ApplyDebitMemoCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var debitMemo = await debitMemoRepository.GetByIdAsync(command.DebitMemoId, cancellationToken);
        if (debitMemo == null)
        {
            return Result.Failure("Debit Memo not found.");
        }

        if (debitMemo.BusinessUnitId != command.BusinessUnitId)
        {
            return Result.Failure("Debit Memo does not belong to the specified Business Unit.");
        }

        debitMemo.MarkAsApplied();

        await debitMemoRepository.UpdateAsync(debitMemo, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}