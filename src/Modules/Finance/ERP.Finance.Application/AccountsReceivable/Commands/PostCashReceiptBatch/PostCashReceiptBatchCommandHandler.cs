using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsReceivable.Commands.PostCashReceiptBatch;

public class PostCashReceiptBatchCommandHandler(
    ICashReceiptBatchRepository batchRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<PostCashReceiptBatchCommand, Result>
{
    public async Task<Result> Handle(PostCashReceiptBatchCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var batch = await batchRepository.GetByIdAsync(command.BatchId, cancellationToken);
        if (batch == null)
        {
            return Result.Failure("Cash Receipt Batch not found.");
        }

        batch.Post(); // This will raise CashReceiptBatchPostedEvent

        await batchRepository.UpdateAsync(batch, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}