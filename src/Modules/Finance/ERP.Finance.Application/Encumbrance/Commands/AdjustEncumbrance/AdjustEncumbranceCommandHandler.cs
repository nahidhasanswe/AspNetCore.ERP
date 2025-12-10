using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.Encumbrance.Aggregates;
using MediatR;

namespace ERP.Finance.Application.Encumbrance.Commands.AdjustEncumbrance;

public class AdjustEncumbranceCommandHandler(
    IEncumbranceRepository encumbranceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<AdjustEncumbranceCommand, Result>
{
    public async Task<Result> Handle(AdjustEncumbranceCommand command, CancellationToken cancellationToken)
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

        encumbrance.Adjust(command.NewAmount);

        await encumbranceRepository.UpdateAsync(encumbrance, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}