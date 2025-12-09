using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Commands.UnapplyCashReceipt;

public class UnapplyCashReceiptCommandHandler(
    ICashReceiptRepository cashReceiptRepository,
    ICustomerInvoiceRepository invoiceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UnapplyCashReceiptCommand, Result>
{
    public async Task<Result> Handle(UnapplyCashReceiptCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var cashReceipt = await cashReceiptRepository.GetByIdAsync(command.CashReceiptId, cancellationToken);
        if (cashReceipt == null)
        {
            return Result.Failure("Cash Receipt not found.");
        }

        var invoice = await invoiceRepository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            return Result.Failure("Customer Invoice not found.");
        }

        // Unapply cash from the receipt
        var unapplyCashResult = cashReceipt.UnapplyCash(command.AmountToUnapply);
        if (unapplyCashResult.IsFailure)
        {
            return unapplyCashResult;
        }

        // Unapply payment from the invoice
        invoice.UnapplyPayment(command.AmountToUnapply);

        await cashReceiptRepository.UpdateAsync(cashReceipt, cancellationToken);
        await invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}