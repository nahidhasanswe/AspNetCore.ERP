using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CashReceipts;

public class RecordCashReceiptCommandHandler(
    ICashReceiptRepository repository,
    IUnitOfWorkManager unitOfWork
) : IRequestCommandHandler<RecordCashReceiptCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RecordCashReceiptCommand command, CancellationToken cancellationToken)
    {
        // 1. Resolve GL CASH ACCOUNT ID using PaymentMethod Value Object
        Guid cashAccountId;
        try
        {
            // The PaymentMethod VO provides the GL account that was DEBITED by the bank
            cashAccountId = PaymentMethod.FromCode(command.CashMethodCode).ClearingAccountId; 
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>($"Invalid payment method code: {ex.Message}");
        }

        var receivedAmount = new Money(command.Amount, command.Currency);

        // 2. Execute Domain Logic: Create the CashReceipt Aggregate
        var receipt = CashReceipt.Create(
            command.BusinessUnitId, // Pass BusinessUnitId
            command.CustomerId, 
            command.ReceiptDate, 
            receivedAmount, 
            command.TransactionReference,
            cashAccountId
        );
        // The receipt constructor raises the UnappliedCashCreatedEvent.

        // 3. Persist and Dispatch Event
        using var scope = unitOfWork.Begin();
        await repository.AddAsync(receipt, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
        
        return Result.Success(receipt.Id);
    }
}