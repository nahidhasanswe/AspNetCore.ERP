using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsReceivable.Commands.CreateCashReceipt;

public class CreateCashReceiptCommandHandler : IRequestCommandHandler<CreateCashReceiptCommand, Guid>
{
    private readonly ICashReceiptRepository _cashReceiptRepository;
    private readonly IUnitOfWorkManager _unitOfWork;

    public CreateCashReceiptCommandHandler(ICashReceiptRepository cashReceiptRepository, IUnitOfWorkManager unitOfWork)
    {
        _cashReceiptRepository = cashReceiptRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCashReceiptCommand command, CancellationToken cancellationToken)
    {
        using var scope = _unitOfWork.Begin();

        var cashReceipt = CashReceipt.Create(
            command.CustomerId,
            command.ReceiptDate,
            command.ReceivedAmount,
            command.TransactionReference,
            command.CashAccountId
        );

        await _cashReceiptRepository.AddAsync(cashReceipt, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(cashReceipt.Id);
    }
}