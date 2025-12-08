using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Commands.RecordPayment;

public class RecordPaymentCommandHandler(
    ICustomerInvoiceRepository repository,
    IUnitOfWorkManager unitOfWork
    ) : IRequestHandler<RecordPaymentCommand, Result<bool>>
{

    public async Task<Result<bool>> Handle(RecordPaymentCommand command, CancellationToken cancellationToken)
    {
        var invoice = await repository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null) 
            return Result.Failure<bool>($"Customer Invoice {command.InvoiceId} not found.");

        Guid cashAccountId;
        try
        {
            // The PaymentMethod VO resolves itself from the code and provides the GL account ID.
            PaymentMethod method = PaymentMethod.FromCode(command.CashMethodCode);
            cashAccountId = method.ClearingAccountId; 
        }
        catch (ArgumentException ex) // Catch the exception thrown by PaymentMethod.FromCode
        {
            // Domain failure due to bad input
            return Result.Failure<bool>($"Invalid payment method code: {ex.Message}");
        }
        
        var paymentAmount = new Money(command.Amount, command.Currency);
        
        // CORE AR DOMAIN LOGIC: Records payment and raises PaymentReceivedEvent
        invoice.RecordPayment(command.Reference, paymentAmount, cashAccountId, command.PaymentDate);

        using var scope = unitOfWork.Begin();
        await repository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken); // Saves state and dispatches events
        
        return Result.Success(true);
    }
}