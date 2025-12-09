using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CreateCashReceiptBatch;

public class CreateCashReceiptBatchCommandHandler : IRequestCommandHandler<CreateCashReceiptBatchCommand, Guid>
{
    private readonly ICashReceiptBatchRepository _batchRepository;
    private readonly IUnitOfWorkManager _unitOfWork;

    public CreateCashReceiptBatchCommandHandler(ICashReceiptBatchRepository batchRepository, IUnitOfWorkManager unitOfWork)
    {
        _batchRepository = batchRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCashReceiptBatchCommand command, CancellationToken cancellationToken)
    {
        using var scope = _unitOfWork.Begin();

        var batch = new CashReceiptBatch(command.BatchDate, command.CashAccountId, command.Reference);

        await _batchRepository.AddAsync(batch, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(batch.Id);
    }
}