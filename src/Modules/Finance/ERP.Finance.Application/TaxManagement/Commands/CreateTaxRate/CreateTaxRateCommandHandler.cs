using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.TaxManagement.Commands.CreateTaxRate;

public class CreateTaxRateCommandHandler(ITaxRateRepository taxRateRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreateTaxRateCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateTaxRateCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var taxRate = new TaxRate(
            command.JurisdictionId,
            command.Rate,
            command.EffectiveDate
            // Removed: command.TaxPayableAccountId
        );

        await taxRateRepository.AddAsync(taxRate, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(taxRate.Id);
    }
}