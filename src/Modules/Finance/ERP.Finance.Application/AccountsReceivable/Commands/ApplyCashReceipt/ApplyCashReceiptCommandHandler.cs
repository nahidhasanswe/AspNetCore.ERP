using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Commands.ApplyCashReceipt;

public class ApplyCashReceiptCommandHandler(
    ICashReceiptRepository cashReceiptRepository,
    ICustomerInvoiceRepository invoiceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<ApplyCashReceiptCommand, Result>
{
    public async Task<Result> Handle(ApplyCashReceiptCommand command, CancellationToken cancellationToken)
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

        // Apply cash to the receipt
        var applyCashResult = cashReceipt.ApplyCash(command.AmountToApply);
        if (applyCashResult.IsFailure)
        {
            return applyCashResult;
        }

        // Record payment on the invoice
        // Note: The RecordPayment method on CustomerInvoice needs cashAccountId and paymentDate.
        // For simplicity, we'll use the CashReceipt's CashAccountId and ReceiptDate.
        invoice.RecordPayment(
            cashReceipt.TransactionReference,
            command.AmountToApply,
            cashReceipt.CashAccountId,
            cashReceipt.ReceiptDate
        );

        await cashReceiptRepository.UpdateAsync(cashReceipt, cancellationToken);
        await invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}