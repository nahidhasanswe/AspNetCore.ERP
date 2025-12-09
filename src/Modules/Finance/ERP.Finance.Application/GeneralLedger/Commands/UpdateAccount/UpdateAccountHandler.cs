using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.Commands.UpdateAccount;

public class UpdateAccountHandler(IAccountRepository accountRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UpdateAccountCommand, Result>
{
    public async Task<Result> Handle(UpdateAccountCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var account = await accountRepository.GetByIdAsync(command.AccountId, cancellationToken);
        if (account == null)
        {
            return Result.Failure("Account not found.");
        }

        account.Update(
            command.AccountCode,
            command.Name,
            command.Type,
            command.ParentId,
            command.BusinessUnitId // Pass BusinessUnitId to update method
        );
        // Note: IsSummary and IsActive are updated via separate methods on the aggregate.

        await accountRepository.UpdateAsync(account, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}