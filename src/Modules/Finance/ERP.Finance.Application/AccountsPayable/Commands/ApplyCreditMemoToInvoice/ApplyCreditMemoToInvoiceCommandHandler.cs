using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.ApplyCreditMemoToInvoice;

public class ApplyCreditMemoToInvoiceCommandHandler(
    IVendorInvoiceRepository invoiceRepository,
    ICreditMemoRepository creditMemoRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<ApplyCreditMemoToInvoiceCommand, Unit>
{
    public async Task<Result<Unit>> Handle(ApplyCreditMemoToInvoiceCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var invoice = await invoiceRepository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            return Result.Failure<Unit>("Invoice not found.");
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

        return Result.Success(Unit.Value);
    }
}