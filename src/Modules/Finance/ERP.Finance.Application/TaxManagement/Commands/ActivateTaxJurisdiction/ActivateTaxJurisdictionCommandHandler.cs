using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.TaxManagement.Commands.ActivateTaxJurisdiction;

public class ActivateTaxJurisdictionCommandHandler(
    ITaxJurisdictionRepository jurisdictionRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<ActivateTaxJurisdictionCommand, Result>
{
    public async Task<Result> Handle(ActivateTaxJurisdictionCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var jurisdiction = await jurisdictionRepository.GetByIdAsync(command.JurisdictionId, cancellationToken);
        if (jurisdiction == null)
        {
            return Result.Failure("Tax Jurisdiction not found.");
        }

        jurisdiction.Activate();

        await jurisdictionRepository.UpdateAsync(jurisdiction, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}