using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.Encumbrance.Aggregates;
using MediatR;

namespace ERP.Finance.Application.Encumbrance.Commands.CloseEncumbrance;

public class CloseEncumbranceCommandHandler(IEncumbranceRepository encumbranceRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<CloseEncumbranceCommand, Result>
{
    public async Task<Result> Handle(CloseEncumbranceCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var encumbrance = await encumbranceRepository.GetByIdAsync(command.EncumbranceId, cancellationToken);
        if (encumbrance == null)
        {
            return Result.Failure("Encumbrance not found.");
        }

        if (encumbrance.BusinessUnitId != command.BusinessUnitId)
        {
            return Result.Failure("Encumbrance does not belong to the specified Business Unit.");
        }

        encumbrance.Close();

        await encumbranceRepository.UpdateAsync(encumbrance, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}