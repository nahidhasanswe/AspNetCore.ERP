using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsPayable.Commands.SubmitVendorOnboardingRequest;

public class SubmitVendorOnboardingRequestCommandHandler : IRequestCommandHandler<SubmitVendorOnboardingRequestCommand, Guid>
{
    private readonly IVendorOnboardingRequestRepository _onboardingRepository;
    private readonly IUnitOfWorkManager _unitOfWork;

    public SubmitVendorOnboardingRequestCommandHandler(IVendorOnboardingRequestRepository onboardingRepository, IUnitOfWorkManager unitOfWork)
    {
        _onboardingRepository = onboardingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(SubmitVendorOnboardingRequestCommand command, CancellationToken cancellationToken)
    {
        using var scope = _unitOfWork.Begin();

        var request = new VendorOnboardingRequest(
            command.ProposedName,
            command.ProposedTaxId,
            command.ProposedAddress,
            command.ProposedContactInfo,
            command.ProposedPaymentTerms,
            command.ProposedDefaultCurrency,
            command.ProposedBankDetails
        );
        request.SubmitForApproval(); // Automatically submit upon creation

        await _onboardingRepository.AddAsync(request, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(request.Id);
    }
}