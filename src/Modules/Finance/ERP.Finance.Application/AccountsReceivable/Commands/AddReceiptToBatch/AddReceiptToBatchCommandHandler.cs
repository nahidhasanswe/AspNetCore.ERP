using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsReceivable.Commands.AddReceiptToBatch;

public class AddReceiptToBatchCommandHandler(
    ICashReceiptBatchRepository batchRepository,
    ICashReceiptRepository receiptRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<AddReceiptToBatchCommand, Result>
{
    // To get receipt details

    public async Task<Result> Handle(AddReceiptToBatchCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var batch = await batchRepository.GetByIdAsync(command.BatchId, cancellationToken);
        if (batch == null)
        {
            return Result.Failure("Cash Receipt Batch not found.");
        }

        var receipt = await receiptRepository.GetByIdAsync(command.ReceiptId, cancellationToken);
        if (receipt == null)
        {
            return Result.Failure("Cash Receipt not found.");
        }

        batch.AddReceipt(receipt); // Add receipt to batch

        await batchRepository.UpdateAsync(batch, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}