using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.Encumbrance.Aggregates;
using MediatR;

namespace ERP.Finance.Application.Encumbrance.Commands.CancelEncumbrance;

public class CancelEncumbranceCommandHandler(
    IEncumbranceRepository encumbranceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<CancelEncumbranceCommand, Result>
{
    public async Task<Result> Handle(CancelEncumbranceCommand command, CancellationToken cancellationToken)
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

        encumbrance.Cancel();

        await encumbranceRepository.UpdateAsync(encumbrance, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}