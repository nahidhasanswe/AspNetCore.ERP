using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.TaxManagement.Commands.CreateTaxRate;

public class CreateTaxRateCommandHandler : IRequestCommandHandler<CreateTaxRateCommand, Guid>
{
    private readonly ITaxRateRepository _taxRateRepository;
    private readonly IUnitOfWorkManager _unitOfWork;

    public CreateTaxRateCommandHandler(ITaxRateRepository taxRateRepository, IUnitOfWorkManager unitOfWork)
    {
        _taxRateRepository = taxRateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateTaxRateCommand command, CancellationToken cancellationToken)
    {
        using var scope = _unitOfWork.Begin();

        var taxRate = new TaxRate(
            command.JurisdictionId,
            command.Rate,
            command.EffectiveDate,
            command.TaxPayableAccountId
        );

        await _taxRateRepository.AddAsync(taxRate, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(taxRate.Id);
    }
}