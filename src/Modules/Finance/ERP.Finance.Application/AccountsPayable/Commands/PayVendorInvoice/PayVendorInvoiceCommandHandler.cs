using ERP.Core;
using ERP.Core.Exceptions;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Services;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.PayVendorInvoice;

public class PayVendorInvoiceCommandHandler (
        IVendorInvoiceRepository repository,
        IGLConfigurationService glConfigurationService,
        IUnitOfWorkManager unitOfWork
    )
    : IRequestHandler<PayVendorInvoiceCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(PayVendorInvoiceCommand command, CancellationToken cancellationToken)
    {
        var invoice = await repository.GetByIdAsync(command.InvoiceId, cancellationToken);
        
        if (invoice == null) return Result.Failure<bool>($"Invoice not found");

        Guid paymentAccountId = await glConfigurationService.GetPaymentClearingAccountId(invoice.BusinessUnitId, invoice.TotalAmount.Currency, cancellationToken);
        
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