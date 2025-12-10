using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CreateCashReceiptBatch;

public class CreateCashReceiptBatchCommandHandler(
    ICashReceiptBatchRepository batchRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreateCashReceiptBatchCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCashReceiptBatchCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var batch = new CashReceiptBatch(
            command.BusinessUnitId,
            command.BatchDate,
            command.CashAccountId,
            command.Reference
        );

        await batchRepository.AddAsync(batch, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(batch.Id);
    }
}