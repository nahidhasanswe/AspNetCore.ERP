using ERP.Core;
using ERP.Core.Mapping;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;
using ERP.Finance.Application.AccountsPayable.DTOs;
using ERP.Finance.Domain.Shared.DTOs;

namespace ERP.Finance.Application.AccountsPayable.Queries.GetVendorOnboardingRequestDetails;

public class GetVendorOnboardingRequestDetailsQueryHandler(
    IVendorOnboardingRequestRepository onboardingRepository,
    IObjectMapper mapper
    )
    : IRequestHandler<GetVendorOnboardingRequestDetailsQuery, Result<VendorOnboardingRequestDetailsDto>>
{
    public async Task<Result<VendorOnboardingRequestDetailsDto>> Handle(GetVendorOnboardingRequestDetailsQuery request, CancellationToken cancellationToken)
    {
        var onboardingRequest = await onboardingRepository.GetByIdAsync(request.RequestId, cancellationToken);
        if (onboardingRequest == null)
        {
            return Result.Failure<VendorOnboardingRequestDetailsDto>("Vendor onboarding request not found.");
        }

        var dto = new VendorOnboardingRequestDetailsDto(
            onboardingRequest.Id,
            onboardingRequest.ProposedName,
            onboardingRequest.ProposedTaxId,
            mapper.Map<AddressDto>(onboardingRequest.ProposedAddress),
            mapper.Map<ContactInfoDto>(onboardingRequest.ProposedContactInfo),
            onboardingRequest.ProposedPaymentTerms,
            onboardingRequest.ProposedDefaultCurrency,
            mapper.Map<VendorBankDetailsDto>(onboardingRequest.ProposedBankDetails),
            onboardingRequest.Status,
            onboardingRequest.RejectionReason,
            onboardingRequest.ApprovedVendorId,
            onboardingRequest.CreatedAt
        );

        return Result.Success(dto);
    }
}