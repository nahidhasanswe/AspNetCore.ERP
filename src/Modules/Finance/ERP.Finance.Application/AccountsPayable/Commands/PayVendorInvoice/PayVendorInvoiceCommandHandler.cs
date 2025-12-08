using ERP.Core;
using ERP.Core.Exceptions;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.PayVendorInvoice;

public class PayVendorInvoiceCommandHandler (
        IVendorInvoiceRepository repository,
        IUnitOfWorkManager unitOfWork
    )
    : IRequestHandler<PayVendorInvoiceCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(PayVendorInvoiceCommand command, CancellationToken cancellationToken)
    {
        var invoice = await repository.GetByIdAsync(command.InvoiceId, cancellationToken);
        
        if (invoice == null) return Result.Success(false);
        
        PaymentMethod paymentMethod;
        try
        {
            // Use the Value Object to determine the GL Clearing Account ID
            paymentMethod = PaymentMethod.FromCode(command.PaymentMethodCode);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<bool>($"Invalid payment method: {ex.Message}");
        }
        
        // The GL account the payment is drawn from (the Credit side of the entry)
        Guid paymentAccountId = paymentMethod.ClearingAccountId;
        
        // CORE AP DOMAIN LOGIC
        try
        {
            invoice.RecordPayment(command.PaymentAmount, command.PaymentReference, command.PaymentDate, paymentAccountId, invoice.APControlAccountId);
        }
        catch (DomainException ex)
        {
            return Result.Failure<bool>(ex.Message);
        }
        
        using var scope = unitOfWork.Begin();
        await repository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
        
        return Result.Success(true);
    }
}