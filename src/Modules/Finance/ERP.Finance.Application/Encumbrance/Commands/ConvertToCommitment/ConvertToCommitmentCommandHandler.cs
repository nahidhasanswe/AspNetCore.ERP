using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.Encumbrance.Aggregates;
using MediatR;

namespace ERP.Finance.Application.Encumbrance.Commands.ConvertToCommitment;

public class ConvertToCommitmentCommandHandler(
    IEncumbranceRepository encumbranceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<ConvertToCommitmentCommand, Result>
{
    public async Task<Result> Handle(ConvertToCommitmentCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var encumbrance = await encumbranceRepository.GetByIdAsync(command.EncumbranceId, cancellationToken);
        if (encumbrance == null)
        {
            return Result.Failure("Encumbrance not found.");
        }

        var result = encumbrance.ConvertToCommitment(command.NewAmount);
        if (result.IsFailure)
        {
            return result;
        }

        await encumbranceRepository.UpdateAsync(encumbrance, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}