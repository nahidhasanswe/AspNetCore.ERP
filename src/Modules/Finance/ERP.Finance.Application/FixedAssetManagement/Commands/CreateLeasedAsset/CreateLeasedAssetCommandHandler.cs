using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.FixedAssetManagement.Commands.CreateLeasedAsset;

public class CreateLeasedAssetCommandHandler : IRequestCommandHandler<CreateLeasedAssetCommand, Guid>
{
    private readonly ILeasedAssetRepository _leasedAssetRepository;
    private readonly IUnitOfWorkManager _unitOfWork;

    public CreateLeasedAssetCommandHandler(ILeasedAssetRepository leasedAssetRepository, IUnitOfWorkManager unitOfWork)
    {
        _leasedAssetRepository = leasedAssetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateLeasedAssetCommand command, CancellationToken cancellationToken)
    {
        using var scope = _unitOfWork.Begin();

        var leasedAsset = new LeasedAsset(
            command.AssetId,
            command.LeaseAgreementNumber,
            command.Lessor,
            command.Type,
            command.StartDate,
            command.EndDate,
            command.MonthlyPayment
        );

        await _leasedAssetRepository.AddAsync(leasedAsset, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(leasedAsset.Id);
    }
}