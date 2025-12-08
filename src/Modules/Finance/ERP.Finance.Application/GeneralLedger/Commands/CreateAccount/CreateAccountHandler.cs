using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.Commands.CreateAccount;

public class CreateAccountHandler(
    IAccountRepository repository,
    IUnitOfWorkManager unitOfWork
) : IRequestHandler<CreateAccountCommand, Result<Guid>>
{

    public async Task<Result<Guid>> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var entry = new Account(command.AccountCode, command.Name, command.Type.ToString());

        using var scope = unitOfWork.Begin();
        await repository.AddAsync(entry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(entry.Id);
    }
}