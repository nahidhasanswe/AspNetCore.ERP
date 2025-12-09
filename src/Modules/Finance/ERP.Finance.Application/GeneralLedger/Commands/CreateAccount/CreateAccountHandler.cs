using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.GeneralLedger.Aggregates;

namespace ERP.Finance.Application.GeneralLedger.Commands.CreateAccount;

public class CreateAccountHandler(IAccountRepository accountRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreateAccountCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var account = new Account(
            command.BusinessUnitId,
            command.AccountCode,
            command.Name,
            command.Type,
            command.ParentId,
            command.IsSummary
        );

        await accountRepository.AddAsync(account, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(account.Id);
    }
}