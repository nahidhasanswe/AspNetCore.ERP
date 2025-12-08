using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.VoidCreditMemo;

public class VoidCreditMemoCommandHandler(ICreditMemoRepository creditMemoRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<VoidCreditMemoCommand, Unit>
{
    public async Task<Result<Unit>> Handle(VoidCreditMemoCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var creditMemo = await creditMemoRepository.GetByIdAsync(command.CreditMemoId, cancellationToken);
        if (creditMemo == null)
        {
            return Result.Failure<Unit>("Credit Memo not found.");
        }

        creditMemo.Void(command.Reason);

        await creditMemoRepository.UpdateAsync(creditMemo, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}