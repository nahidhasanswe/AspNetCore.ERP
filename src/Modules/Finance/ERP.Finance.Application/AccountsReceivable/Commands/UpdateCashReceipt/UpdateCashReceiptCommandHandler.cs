using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Commands.UpdateCashReceipt;

public class UpdateCashReceiptCommandHandler(
    ICashReceiptRepository cashReceiptRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UpdateCashReceiptCommand, Result>
{
    public async Task<Result> Handle(UpdateCashReceiptCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var cashReceipt = await cashReceiptRepository.GetByIdAsync(command.CashReceiptId, cancellationToken);
        if (cashReceipt == null)
        {
            return Result.Failure("Cash Receipt not found.");
        }

        // No direct change to BusinessUnitId as it's immutable on the aggregate.
        // Validation for BusinessUnitId (e.g., ensuring the cash receipt belongs to the command's BU)
        // would typically happen here or in a pre-handler.
        if (cashReceipt.BusinessUnitId != command.BusinessUnitId)
        {
            return Result.Failure("Cash Receipt does not belong to the specified Business Unit.");
        }

        cashReceipt.Update(
            command.NewReceiptDate,
            command.NewReceivedAmount,
            command.NewTransactionReference,
            command.NewCashAccountId
        );

        await cashReceiptRepository.UpdateAsync(cashReceipt, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}