using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Domain.AccountsReceivable.Events;

namespace ERP.Finance.Application.AccountsReceivable.Commands.IssueCustomerCreditMemo;

public class IssueCustomerCreditMemoCommandHandler(
    ICustomerCreditMemoRepository creditMemoRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<IssueCustomerCreditMemoCommand, Guid>
{
    public async Task<Result<Guid>> Handle(IssueCustomerCreditMemoCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var creditMemo = new CustomerCreditMemo(
            command.BusinessUnitId,
            command.CustomerId,
            command.Amount,
            command.MemoDate,
            command.Reason
        );
        
        // Raise event for GL posting (Debit Revenue Adjustment, Credit AR Control)
        creditMemo.AddDomainEvent(new CustomerCreditMemoIssuedEvent(
            creditMemo.Id,
            creditMemo.CustomerId,
            creditMemo.BusinessUnitId,
            creditMemo.OriginalAmount,
            creditMemo.MemoDate,
            command.ARControlAccountId,
            command.RevenueAccountId, // The GL account to debit (e.g., Sales Returns, Revenue Adjustment)
            creditMemo.Reason
        ));

        await creditMemoRepository.AddAsync(creditMemo, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(creditMemo.Id);
    }
}