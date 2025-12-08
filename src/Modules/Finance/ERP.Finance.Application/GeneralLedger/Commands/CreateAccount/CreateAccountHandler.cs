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
        // Add validation to prevent duplicate account codes.
        var existingAccount = await repository.GetByCodeAsync(command.AccountCode, cancellationToken);
        if (existingAccount is not null)
        {
            return Result.Failure<Guid>($"An account with code '{command.AccountCode}' already exists.");
        }

        // The Account constructor now accepts the AccountType enum directly,
        // along with optional parameters for hierarchy and summary flags.
        var entry = new Account(
            command.AccountCode, 
            command.Name, 
            command.Type, // Pass the enum directly
            command.ParentId, 
            command.IsSummary);

        using var scope = unitOfWork.Begin();
        await repository.AddAsync(entry, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(entry.Id);
    }
}