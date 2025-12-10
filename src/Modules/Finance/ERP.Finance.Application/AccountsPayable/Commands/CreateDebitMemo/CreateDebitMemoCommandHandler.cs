using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreateDebitMemo;

public class CreateDebitMemoCommandHandler(IDebitMemoRepository debitMemoRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreateDebitMemoCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateDebitMemoCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var debitMemo = new DebitMemo(
            command.BusinessUnitId,
            command.VendorId,
            command.Amount,
            command.MemoDate,
            command.Reason,
            command.APControlAccountId
        );

        await debitMemoRepository.AddAsync(debitMemo, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(debitMemo.Id);
    }
}