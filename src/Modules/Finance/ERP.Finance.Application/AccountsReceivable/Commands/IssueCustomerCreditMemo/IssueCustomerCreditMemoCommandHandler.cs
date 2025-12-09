using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Domain.AccountsReceivable.Events; // For CreditMemoIssuedEvent
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsReceivable.Commands.IssueCustomerCreditMemo;

public class IssueCustomerCreditMemoCommandHandler : IRequestCommandHandler<IssueCustomerCreditMemoCommand, Guid>
{
    private readonly ICustomerCreditMemoRepository _creditMemoRepository;
    private readonly IUnitOfWorkManager _unitOfWork;

    public IssueCustomerCreditMemoCommandHandler(ICustomerCreditMemoRepository creditMemoRepository, IUnitOfWorkManager unitOfWork)
    {
        _creditMemoRepository = creditMemoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(IssueCustomerCreditMemoCommand command, CancellationToken cancellationToken)
    {
        using var scope = _unitOfWork.Begin();

        var creditMemo = new CustomerCreditMemo(command.CustomerId, command.Amount, command.MemoDate, command.Reason);
        
        // Raise event for GL posting (Debit Revenue Adjustment, Credit AR Control)
        creditMemo.AddDomainEvent(new CustomerCreditMemoIssuedEvent(
            creditMemo.Id,
            creditMemo.CustomerId,
            creditMemo.OriginalAmount,
            creditMemo.MemoDate,
            command.ARControlAccountId,
            command.RevenueAccountId, // The GL account to debit (e.g., Sales Returns, Revenue Adjustment)
            creditMemo.Reason
        ));

        await _creditMemoRepository.AddAsync(creditMemo, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(creditMemo.Id);
    }
}