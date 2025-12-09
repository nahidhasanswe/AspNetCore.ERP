using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using MediatR;

namespace ERP.Finance.Application.TaxManagement.Commands.UpdateTaxRate;

public class UpdateTaxRateCommandHandler(ITaxRateRepository taxRateRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UpdateTaxRateCommand, Result>
{
    public async Task<Result> Handle(UpdateTaxRateCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var taxRate = await taxRateRepository.GetByIdAsync(command.TaxRateId, cancellationToken);
        if (taxRate == null)
        {
            return Result.Failure("Tax Rate not found.");
        }

        taxRate.Update(
            command.NewRate,
            command.NewEffectiveDate,
            command.NewTaxPayableAccountId
        );

        await taxRateRepository.UpdateAsync(taxRate, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}