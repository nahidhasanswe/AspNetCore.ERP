using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.TaxManagement.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.TaxManagement.Commands.CreateTaxJurisdiction;

public class CreateTaxJurisdictionCommandHandler : IRequestCommandHandler<CreateTaxJurisdictionCommand, Guid>
{
    private readonly ITaxJurisdictionRepository _jurisdictionRepository; // Need to define this
    private readonly IUnitOfWorkManager _unitOfWork;

    public CreateTaxJurisdictionCommandHandler(ITaxJurisdictionRepository jurisdictionRepository, IUnitOfWorkManager unitOfWork)
    {
        _jurisdictionRepository = jurisdictionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateTaxJurisdictionCommand command, CancellationToken cancellationToken)
    {
        using var scope = _unitOfWork.Begin();

        var jurisdiction = new TaxJurisdiction(command.Name, command.RegionCode);

        await _jurisdictionRepository.AddAsync(jurisdiction, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(jurisdiction.Id);
    }
}