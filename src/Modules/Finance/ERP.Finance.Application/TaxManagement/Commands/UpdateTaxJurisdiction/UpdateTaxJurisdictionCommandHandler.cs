using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.TaxManagement.Commands.UpdateTaxJurisdiction;

public class UpdateTaxJurisdictionCommandHandler(
    ITaxJurisdictionRepository jurisdictionRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UpdateTaxJurisdictionCommand, Result>
{
    public async Task<Result> Handle(UpdateTaxJurisdictionCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var jurisdiction = await jurisdictionRepository.GetByIdAsync(command.JurisdictionId, cancellationToken);
        if (jurisdiction == null)
        {
            return Result.Failure("Tax Jurisdiction not found.");
        }

        jurisdiction.Update(command.NewName, command.NewRegionCode);

        await jurisdictionRepository.UpdateAsync(jurisdiction, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}