using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsPayable.Commands.CreateDebitMemo;

public class CreateDebitMemoCommandHandler : IRequestCommandHandler<CreateDebitMemoCommand, Guid>
{
    private readonly IDebitMemoRepository _debitMemoRepository;
    private readonly IUnitOfWorkManager _unitOfWork;

    public CreateDebitMemoCommandHandler(IDebitMemoRepository debitMemoRepository, IUnitOfWorkManager unitOfWork)
    {
        _debitMemoRepository = debitMemoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateDebitMemoCommand command, CancellationToken cancellationToken)
    {
        using var scope = _unitOfWork.Begin();

        var debitMemo = new DebitMemo(command.VendorId, command.Amount, command.MemoDate, command.Reason, command.APControlAccountId);

        await _debitMemoRepository.AddAsync(debitMemo, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(debitMemo.Id);
    }
}