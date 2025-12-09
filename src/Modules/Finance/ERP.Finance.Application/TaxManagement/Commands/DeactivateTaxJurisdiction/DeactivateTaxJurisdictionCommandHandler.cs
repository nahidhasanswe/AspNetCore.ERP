using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.TaxManagement.Commands.DeactivateTaxJurisdiction;

public class DeactivateTaxJurisdictionCommandHandler(
    ITaxJurisdictionRepository jurisdictionRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<DeactivateTaxJurisdictionCommand, Result>
{
    public async Task<Result> Handle(DeactivateTaxJurisdictionCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var jurisdiction = await jurisdictionRepository.GetByIdAsync(command.JurisdictionId, cancellationToken);
        if (jurisdiction == null)
        {
            return Result.Failure("Tax Jurisdiction not found.");
        }

        jurisdiction.Deactivate();

        await jurisdictionRepository.UpdateAsync(jurisdiction, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}