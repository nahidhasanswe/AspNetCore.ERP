using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CreateCashReceipt;

public class CreateCashReceiptCommandHandler(
    ICashReceiptRepository cashReceiptRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreateCashReceiptCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCashReceiptCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var cashReceipt = CashReceipt.Create(
            command.BusinessUnitId,
            command.CustomerId,
            command.ReceiptDate,
            command.ReceivedAmount,
            command.TransactionReference,
            command.CashAccountId
        );

        await cashReceiptRepository.AddAsync(cashReceipt, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(cashReceipt.Id);
    }
}