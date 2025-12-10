using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.ApplyCreditMemoToInvoice;

public class ApplyCreditMemoToInvoiceCommandHandler(
    IVendorInvoiceRepository invoiceRepository,
    ICreditMemoRepository creditMemoRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<ApplyCreditMemoToInvoiceCommand, Result>
{
    public async Task<Result> Handle(ApplyCreditMemoToInvoiceCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var invoice = await invoiceRepository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            return Result.Failure("Invoice not found.");
        }

        var creditMemo = await creditMemoRepository.GetByIdAsync(command.CreditMemoId, cancellationToken);
        if (creditMemo == null)
        {
            return Result.Failure<Unit>("Credit Memo not found.");
        }
        
        

        invoice.ApplyCredit(creditMemo, command.AmountToApply);

        await invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await creditMemoRepository.UpdateAsync(creditMemo, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}