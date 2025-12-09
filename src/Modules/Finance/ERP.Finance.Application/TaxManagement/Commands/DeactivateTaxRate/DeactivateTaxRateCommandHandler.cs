using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.TaxManagement.Commands.DeactivateTaxRate;

public class DeactivateTaxRateCommandHandler(ITaxRateRepository taxRateRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<DeactivateTaxRateCommand, Result>
{
    public async Task<Result> Handle(DeactivateTaxRateCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var taxRate = await taxRateRepository.GetByIdAsync(command.TaxRateId, cancellationToken);
        if (taxRate == null)
        {
            return Result.Failure("Tax Rate not found.");
        }

        taxRate.Deactivate();

        await taxRateRepository.UpdateAsync(taxRate, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}