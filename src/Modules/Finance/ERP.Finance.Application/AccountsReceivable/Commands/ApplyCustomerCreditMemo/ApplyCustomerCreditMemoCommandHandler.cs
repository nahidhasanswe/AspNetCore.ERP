using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Commands.ApplyCustomerCreditMemo;

public class ApplyCustomerCreditMemoCommandHandler(
    ICustomerCreditMemoRepository creditMemoRepository,
    ICustomerInvoiceRepository invoiceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<ApplyCustomerCreditMemoCommand, Result>
{
    public async Task<Result> Handle(ApplyCustomerCreditMemoCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var creditMemo = await creditMemoRepository.GetByIdAsync(command.CreditMemoId, cancellationToken);
        if (creditMemo == null)
        {
            return Result.Failure("Customer Credit Memo not found.");
        }

        var invoice = await invoiceRepository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            return Result.Failure("Customer Invoice not found.");
        }

        // Apply credit to the invoice
        invoice.ApplyCreditMemo(creditMemo, command.AmountToApply); // This method needs to be added to CustomerInvoice

        // Apply credit to the credit memo (updates its available amount and status)
        creditMemo.Apply(command.AmountToApply);

        await creditMemoRepository.UpdateAsync(creditMemo, cancellationToken);
        await invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}