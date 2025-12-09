using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreateCreditMemo;

public class CreateCreditMemoCommandHandler(ICreditMemoRepository creditMemoRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreateCreditMemoCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCreditMemoCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var creditMemo = new CreditMemo(command.VendorId, command.BusinessUnitId, command.Amount, command.MemoDate, command.Reason);

        await creditMemoRepository.AddAsync(creditMemo, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(creditMemo.Id);
    }
}